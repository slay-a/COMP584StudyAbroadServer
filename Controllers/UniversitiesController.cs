using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP584StudyAbroadServer.Data;
using COMP584StudyAbroadServer.Models;

namespace COMP584StudyAbroadServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UniversitiesController : ControllerBase
{
    private readonly StudyAbroadContext _context;

    public UniversitiesController(StudyAbroadContext context)
    {
        _context = context;
    }

    // GET: api/Universities
    [HttpGet]
    [AllowAnonymous]   // everyone can browse universities
    public async Task<ActionResult<IEnumerable<University>>> GetUniversities(
        [FromQuery] string? country,
        [FromQuery] string? city,
        [FromQuery] string? search)
    {
        var query = _context.Universities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(country))
        {
            var term = country.Trim().ToLower();
            query = query.Where(u => u.Country.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var term = city.Trim().ToLower();
            query = query.Where(u => u.City.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                u.Name.ToLower().Contains(term) ||
                u.Country.ToLower().Contains(term) ||
                u.City.ToLower().Contains(term));
        }

        return await query.ToListAsync();
    }

    // GET: api/Universities/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<University>> GetUniversity(int id)
    {
        var university = await _context.Universities.FindAsync(id);

        if (university == null)
        {
            return NotFound();
        }

        return university;
    }

    // POST: api/Universities  (ADMIN ONLY)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<University>> PostUniversity(University university)
    {
        _context.Universities.Add(university);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUniversity), new { id = university.Id }, university);
    }

    // PUT: api/Universities/5  (ADMIN ONLY)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutUniversity(int id, University university)
    {
        if (id != university.Id)
        {
            return BadRequest();
        }

        _context.Entry(university).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Universities.Any(e => e.Id == id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Universities/5  (ADMIN ONLY)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUniversity(int id)
    {
        var university = await _context.Universities.FindAsync(id);
        if (university == null)
        {
            return NotFound();
        }

        _context.Universities.Remove(university);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
