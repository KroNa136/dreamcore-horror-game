using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DevelopersController : DatabaseController
    {
        public DevelopersController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetDevelopers()
        {
            return _context.Developers == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Developers.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetDeveloper(Guid? id)
        {
            if (id == null || _context.Developers == null)
                return NotFound();

            var developer = await _context.Developers.FindAsync(id);

            if (developer == null)
                return NotFound();

            return Ok(developer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeveloper([Bind("Id,Login,Password,DeveloperAccessLevelId")] Developer developer)
        {
            if (ModelState.IsValid)
            {
                developer.Id = Guid.NewGuid();
                _context.Add(developer);
                await _context.SaveChangesAsync();
                return Ok(developer);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeveloper(Guid? id, [Bind("Id,Login,Password,DeveloperAccessLevelId")] Developer developer)
        {
            if (id == null || _context.Developers == null)
                return NotFound();

            if (id != developer.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(developer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeveloperExists(developer.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(developer);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDeveloper(Guid? id)
        {
            if (id == null || _context.Developers == null)
                return NotFound();

            var developer = await _context.Developers.FindAsync(id);

            if (developer == null)
                return NotFound();

            _context.Developers.Remove(developer);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool DeveloperExists(Guid id) => (_context.Developers?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
