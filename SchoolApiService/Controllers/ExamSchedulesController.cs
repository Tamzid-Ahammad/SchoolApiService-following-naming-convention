using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApp.Models.ViewModels;

namespace SchoolApiService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExamSchedulesController : ControllerBase
	{
		private readonly SchoolDbContext _context;

		public ExamSchedulesController(SchoolDbContext context)
		{
			_context = context;
		}

		// GET: api/ExamSchedules
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ExamSchedule>>> GetExamSchedules()
		{
			return await _context.ExamSchedules
				.Include(es => es.ExamType)
				.Include(es => es.ExamSubjects)
				.ThenInclude(es => es.Subject)
				.ToListAsync();
		}

		// GET: api/ExamSchedules/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ExamSchedule>> GetExamSchedule(int id)
		{
			var examSchedule = await _context.ExamSchedules
				.Include(es => es.ExamType)
				.Include(es => es.ExamSubjects)
				.ThenInclude(es => es.Subject)
				.FirstOrDefaultAsync(es => es.ExamScheduleId == id);

			if (examSchedule == null)
			{
				return NotFound();
			}

			return examSchedule;
		}

		// PUT: api/ExamSchedules/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutExamSchedule(int id, ExamSchedule examSchedule)
		{
			if (id != examSchedule.ExamScheduleId)
			{
				return BadRequest();
			}

			_context.Entry(examSchedule).State = (System.Data.Entity.EntityState)EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ExamScheduleExists(id))
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

		// POST: api/ExamSchedules
		[HttpPost]
		public async Task PostExamSchedule(SaveExamScheduleVM examScheduleRequest)
		{
			var examSchedule = new ExamSchedule
			{
				ExamScheduleName = examScheduleRequest.ExamScheduleName,
				ExamTypeId = examScheduleRequest.ExamTypeId,
			};
			_context.ExamSchedules.Add(examSchedule);
			await _context.SaveChangesAsync();

			foreach (var subjectId in examScheduleRequest.SubjectIds)
			{
				_context.ExamSubjects.Add(new ExamSubject
				{
					ExamScheduleId = examSchedule.ExamScheduleId,
					SubjectId = subjectId,
				});
				await _context.SaveChangesAsync();
			}
		}

		// DELETE: api/ExamSchedules/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteExamSchedule(int id)
		{
			var examSchedule = await _context.ExamSchedules.FindAsync(id);
			if (examSchedule == null)
			{
				return NotFound();
			}

			_context.ExamSchedules.Remove(examSchedule);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ExamScheduleExists(int id)
		{
			return _context.ExamSchedules.Any(es => es.ExamScheduleId == id);
		}
	}
}
