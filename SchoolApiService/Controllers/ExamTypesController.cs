using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApp.Models.ViewModels;

namespace SchoolApiService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExamTypesController : ControllerBase
	{
		private readonly SchoolDbContext _context;

		public ExamTypesController(SchoolDbContext context)
		{
			_context = context;
		}

		// GET: api/ExamType
		[HttpGet]
		public async Task<IEnumerable<ExamType>> GetdbsExamType()
		{
			return await _context.ExamTypes.ToListAsync();
		}

		// GET: api/ExamType/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ExamType>> GetExamType(int id)
		{
			var examType = await _context.ExamTypes.FindAsync(id);

			if (examType == null)
			{
				return NotFound();
			}

			return examType;
		}

		// PUT: api/ExamType/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutExamType(int id, ExamType examType)
		{
			if (id != examType.ExamTypeId)
			{
				return BadRequest();
			}

			_context.Entry(examType).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ExamTypeExists(id))
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

		// POST: api/ExamType
		[HttpPost]
		public async Task PostExamType(SaveExamTypeVM examType)
		{
			_context.ExamTypes.Add(new ExamType
			{
				ExamTypeName = examType.ExamTypeName,
			});
			await _context.SaveChangesAsync();
		}

		// DELETE: api/ExamType/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteExamType(int id)
		{
			var examType = await _context.ExamTypes.FindAsync(id);
			if (examType == null)
			{
				return NotFound();
			}

			_context.ExamTypes.Remove(examType);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ExamTypeExists(int id)
		{
			return _context.ExamTypes.Any(e => e.ExamTypeId == id);
		}
	}
}
