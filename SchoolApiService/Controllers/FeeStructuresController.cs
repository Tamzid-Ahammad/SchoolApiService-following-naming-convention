using System;
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
	public class FeeStructuresController : ControllerBase
	{
		private readonly SchoolDbContext _context;

		public FeeStructuresController(SchoolDbContext context)
		{
			_context = context;
		}

		// GET: api/FeeStructures
		[HttpGet]
		public async Task<ActionResult<IEnumerable<FeeStructure>>> GetFeeStructures()
		{
			return await _context.FeeStructures.ToListAsync();
		}

		// GET: api/FeeStructures/5
		[HttpGet("{id}")]
		public async Task<ActionResult<FeeStructure>> GetFeeStructure(int id)
		{
			var feeStructure = await _context.FeeStructures.FindAsync(id);

			if (feeStructure == null)
			{
				return NotFound();
			}

			return feeStructure;
		}

		// PUT: api/FeeStructures/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutFeeStructure(int id, FeeStructure feeStructure)
		{
			if (id != feeStructure.FeeStructureId)
			{
				return BadRequest();
			}

			_context.Entry(feeStructure).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FeeStructureExists(id))
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

		// POST: api/FeeStructures
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<FeeStructure>> PostFeeStructure(FeeStructure feeStructure)
		{
			_context.FeeStructures.Add(feeStructure);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetFeeStructure", new { id = feeStructure.FeeStructureId }, feeStructure);
		}

		// DELETE: api/FeeStructures/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFeeStructure(int id)
		{
			var feeStructure = await _context.FeeStructures.FindAsync(id);
			if (feeStructure == null)
			{
				return NotFound();
			}

			_context.FeeStructures.Remove(feeStructure);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool FeeStructureExists(int id)
		{
			return _context.FeeStructures.Any(e => e.FeeStructureId == id);
		}
	}
}
