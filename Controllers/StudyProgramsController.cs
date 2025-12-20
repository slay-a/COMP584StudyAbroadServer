using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP584StudyAbroadServer.Data;
using COMP584StudyAbroadServer.Models;

namespace COMP584StudyAbroadServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudyProgramsController : ControllerBase
{
    private readonly StudyAbroadContext _context;

    public StudyProgramsController(StudyAbroadContext context)
    {
        _context = context;
    }

  
[HttpGet]
[AllowAnonymous]
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
        var term = degreeLevel.Trim().ToLower();
        query = query.Where(p => p.DegreeLevel.ToLower().Contains(term));
    }

    if (!string.IsNullOrWhiteSpace(language))
    {
        var term = language.Trim().ToLower();
        query = query.Where(p => p.Language.ToLower().Contains(term));
    }

    return await query.ToListAsync();
}



    // GET: api/StudyPrograms/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<StudyProgram>> GetStudyProgram(int id)
    {
        var program = await _context.StudyPrograms.FindAsync(id);

        if (program == null)
        {
            return NotFound();
        }

        return program;
    }

    // POST: api/StudyPrograms   (ADMIN ONLY)
    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    // PUT: api/StudyPrograms/5   (ADMIN ONLY)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
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

    // DELETE: api/StudyPrograms/5   (ADMIN ONLY)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
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
