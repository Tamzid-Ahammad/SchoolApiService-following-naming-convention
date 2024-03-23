using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FeePaymentsController : ControllerBase
	{
		private readonly SchoolDbContext _context;

		public FeePaymentsController(SchoolDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetFeePayments()
		{
			try
			{
				var feePayments = await _context.FeePayments
					.Include(fp => fp.FeePaymentDetails)
					.ToListAsync();

				return Ok(feePayments);
			}
			catch (Exception ex)
			{
				// Log the exception for debugging purposes
				Console.WriteLine($"Exception: {ex}");

				return StatusCode(500, $"Internal Server Error: {ex.Message}");
			}
		}


		// GET: api/FeePayments/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetFeePaymentById(int id)
		{
			try
			{
				var feePayment = await _context.FeePayments
					.Include(fp => fp.FeePaymentDetails)
					.FirstOrDefaultAsync(fp => fp.FeePaymentId == id);

				if (feePayment == null)
				{
					return NotFound($"FeePayment with ID {id} not found");
				}

				return Ok(feePayment);
			}
			catch (Exception ex)
			{
				// Log the exception for debugging purposes
				Console.WriteLine($"Exception: {ex}");

				return StatusCode(500, $"Internal Server Error: {ex.Message}");
			}
		}


		// PUT: api/FeePayments/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutFeePayment(int id, FeePayment feePayment)
		{
			if (id != feePayment.FeePaymentId)
			{
				return BadRequest();
			}

			_context.Entry(feePayment).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FeePaymentExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		[HttpPost]
		public async Task<IActionResult> CreateFeePayment([FromBody] FeePayment feePayment)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					await AttachFeeStructureAsync(feePayment);
					await CalculateFeePaymentFieldsAsync(feePayment);

					UpdateDueBalance(feePayment);

					_context.FeePayments.Add(feePayment);
					await _context.SaveChangesAsync();

					// Save FeePaymentDetails
					SaveFeePaymentDetails(feePayment);

					transaction.Commit();

					return Ok(feePayment);
				}
				catch (Exception ex)
				{
					// Log the exception for debugging purposes
					Console.WriteLine($"Exception: {ex}");

					transaction.Rollback();
					return StatusCode(500, $"Internal Server Error: {ex.Message}");
				}
			}
		}

		private async Task AttachFeeStructureAsync(FeePayment feePayment)
		{
			if (feePayment.FeeStructures != null && feePayment.FeeStructures.Any())
			{
				feePayment.FeeStructures = await _context.FeeStructures
					.Where(fs => feePayment.FeeStructures.Select(f => f.FeeStructureId).Contains(fs.FeeStructureId))
					.ToListAsync();
			}
		}

		private async Task CalculateFeePaymentFieldsAsync(FeePayment feePayment)
		{
			var studentId = feePayment.StudentId;

			feePayment.TotalFeeAmount = feePayment.FeeStructures?.Sum(fs => fs.FeeAmount) ?? 0;
			feePayment.AmountAfterDiscount = feePayment.TotalFeeAmount - (feePayment.TotalFeeAmount * feePayment.Discount / 100);

			var previousDue = await _context.DueBalances
				.Where(b => b.StudentId == studentId)
				.Select(b => b.DueBalanceAmount)
				.FirstOrDefaultAsync();

			feePayment.PreviousDue = previousDue ?? 0;
			feePayment.TotalAmount = feePayment.AmountAfterDiscount + feePayment.PreviousDue;
			feePayment.AmountRemaining = feePayment.TotalAmount - feePayment.AmountPaid;
		}

		private void UpdateDueBalance(FeePayment feePayment)
		{
			var dueBalance = _context.DueBalances
				.Where(db => db.StudentId == feePayment.StudentId)
				.FirstOrDefault();

			if (dueBalance != null)
			{
				dueBalance.DueBalanceAmount = feePayment.AmountRemaining;
				dueBalance.LastUpdate = DateTime.Now; // Update LastUpdate timestamp
			}
			else
			{
				_context.DueBalances.Add(new DueBalance
				{
					StudentId = feePayment.StudentId,
					DueBalanceAmount = feePayment.AmountRemaining,
					LastUpdate = DateTime.Now // Set LastUpdate timestamp for a new record
				});
			}
		}

		private void SaveFeePaymentDetails(FeePayment feePayment)
		{
			if (feePayment.FeeStructures != null && feePayment.FeeStructures.Any())
			{
				foreach (var feeStructure in feePayment.FeeStructures)
				{
					var feePaymentDetail = new FeePaymentDetail
					{
						FeePaymentId = feePayment.FeePaymentId,
						FeeTypeId = feeStructure.FeeTypeId, // Assuming you have FeeTypeId in FeeStructure
						FeeAmount = feeStructure.FeeAmount
					};

					_context.FeePaymentDetails.Add(feePaymentDetail);
				}

				_context.SaveChanges();
			}
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFeePayment(int id)
		{
			var feePayment = await _context.FeePayments.FindAsync(id);
			if (feePayment == null)
			{
				return NotFound();
			}

			_context.FeePayments.Remove(feePayment);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool FeePaymentExists(int id)
		{
			return _context.FeePayments.Any(e => e.FeePaymentId == id);
		}
	}
}
