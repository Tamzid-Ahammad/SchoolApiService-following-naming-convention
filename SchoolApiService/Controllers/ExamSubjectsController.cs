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
	public class ExamSubjectController : ControllerBase
	{
		private readonly SchoolDbContext _context;

		public ExamSubjectController(SchoolDbContext context)
		{
			_context = context;
		}

		// GET: api/ExamSubject
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ExamSubject>>> GetdbsExamSubject()
		{
			return await _context.ExamSubjects.ToListAsync();
		}

		// GET: api/ExamSubject/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ExamSubject>> GetExamSubject(int id)
		{
			var examSubject = await _context.ExamSubjects.FindAsync(id);

			if (examSubject == null)
			{
				return NotFound();
			}

			return examSubject;
		}

		// PUT: api/ExamSubject/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutExamSubject(int id, ExamSubject examSubject)
		{
			if (id != examSubject.ExamSubjectId)
			{
				return BadRequest();
			}

			_context.Entry(examSubject).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ExamSubjectExists(id))
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

		// POST: api/ExamSubject
		[HttpPost]
		public async Task<ActionResult<ExamSubject>> PostExamSubject(ExamSubject examSubject)
		{
			_context.ExamSubjects.Add(examSubject);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetExamSubject", new { id = examSubject.ExamSubjectId }, examSubject);
		}

		// DELETE: api/ExamSubject/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteExamSubject(int id)
		{
			var examSubject = await _context.ExamSubjects.FindAsync(id);
			if (examSubject == null)
			{
				return NotFound();
			}

			_context.ExamSubjects.Remove(examSubject);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ExamSubjectExists(int id)
		{
			return _context.ExamSubjects.Any(e => e.ExamSubjectId == id);
		}
	}
}
