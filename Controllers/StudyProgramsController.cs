using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP584StudyAbroadServer.Data;
using COMP584StudyAbroadServer.Models;

namespace COMP584StudyAbroadServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudyProgramsController : ControllerBase
{
    private readonly StudyAbroadContext _context;

    public StudyProgramsController(StudyAbroadContext context)
    {
        _context = context;
    }

    // GET: api/StudyPrograms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudyProgram>>> GetStudyPrograms(
        [FromQuery] int? universityId,
        [FromQuery] string? degreeLevel,
        [FromQuery] string? language)
    {
        var query = _context.StudyPrograms.AsQueryable();

        if (universityId.HasValue)
        {
            query = query.Where(p => p.UniversityId == universityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(degreeLevel))
        {
            query = query.Where(p => p.DegreeLevel == degreeLevel);
        }

        if (!string.IsNullOrWhiteSpace(language))
        {
            query = query.Where(p => p.Language == language);
        }

        return await query.ToListAsync();
    }

    // GET: api/StudyPrograms/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StudyProgram>> GetStudyProgram(int id)
    {
        var program = await _context.StudyPrograms.FindAsync(id);

        if (program == null)
        {
            return NotFound();
        }

        return program;
    }

    // POST: api/StudyPrograms
    [HttpPost]
    public async Task<ActionResult<StudyProgram>> PostStudyProgram(StudyProgram program)
    {
        // optional: validate that the university exists
        var uniExists = await _context.Universities.AnyAsync(u => u.Id == program.UniversityId);
        if (!uniExists)
        {
            return BadRequest($"University with id {program.UniversityId} does not exist.");
        }

        _context.StudyPrograms.Add(program);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudyProgram), new { id = program.Id }, program);
    }

    // PUT: api/StudyPrograms/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudyProgram(int id, StudyProgram program)
    {
        if (id != program.Id)
        {
            return BadRequest();
        }

        _context.Entry(program).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.StudyPrograms.Any(e => e.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/StudyPrograms/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudyProgram(int id)
    {
        var program = await _context.StudyPrograms.FindAsync(id);
        if (program == null)
        {
            return NotFound();
        }

        _context.StudyPrograms.Remove(program);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
