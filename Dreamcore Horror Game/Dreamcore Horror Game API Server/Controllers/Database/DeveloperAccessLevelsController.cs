using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DeveloperAccessLevelsController : DatabaseController
    {
        public DeveloperAccessLevelsController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetDeveloperAccessLevels()
        {
            return _context.DeveloperAccessLevels == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.DeveloperAccessLevels.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetDeveloperAccessLevel(Guid? id)
        {
            if (id == null || _context.DeveloperAccessLevels == null)
                return NotFound();

            var developerAccessLevel = await _context.DeveloperAccessLevels.FindAsync(id);

            if (developerAccessLevel == null)
                return NotFound();

            return Ok(developerAccessLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeveloperAccessLevel([Bind("Id,Name")] DeveloperAccessLevel developerAccessLevel)
        {
            if (ModelState.IsValid)
            {
                developerAccessLevel.Id = Guid.NewGuid();
                _context.Add(developerAccessLevel);
                await _context.SaveChangesAsync();
                return Ok(developerAccessLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeveloperAccessLevel(Guid? id, [Bind("Id,Name")] DeveloperAccessLevel developerAccessLevel)
        {
            if (id == null || _context.DeveloperAccessLevels == null)
                return NotFound();

            if (id != developerAccessLevel.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(developerAccessLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeveloperAccessLevelExists(developerAccessLevel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(developerAccessLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDeveloperAccessLevel(Guid? id)
        {
            if (id == null || _context.DeveloperAccessLevels == null)
                return NotFound();

            var developerAccessLevel = await _context.DeveloperAccessLevels.FindAsync(id);

            if (developerAccessLevel == null)
                return NotFound();

            _context.DeveloperAccessLevels.Remove(developerAccessLevel);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool DeveloperAccessLevelExists(Guid id) => (_context.DeveloperAccessLevels?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
