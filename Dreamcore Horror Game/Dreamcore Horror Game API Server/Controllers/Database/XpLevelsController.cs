using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class XpLevelsController : DatabaseController
    {
        public XpLevelsController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetXpLevels()
        {
            return _context.XpLevels == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.XpLevels.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetXpLevel(Guid? id)
        {
            if (id == null || _context.XpLevels == null)
                return NotFound();

            var xpLevel = await _context.XpLevels.FindAsync(id);

            if (xpLevel == null)
                return NotFound();

            return Ok(xpLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateXpLevel([Bind("Id,Number,RequiredXp")] XpLevel xpLevel)
        {
            if (ModelState.IsValid)
            {
                xpLevel.Id = Guid.NewGuid();
                _context.Add(xpLevel);
                await _context.SaveChangesAsync();
                return Ok(xpLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditXpLevel(Guid? id, [Bind("Id,Number,RequiredXp")] XpLevel xpLevel)
        {
            if (id == null || _context.XpLevels == null)
                return NotFound();

            if (id != xpLevel.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xpLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XpLevelExists(xpLevel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(xpLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteXpLevel(Guid? id)
        {
            if (id == null || _context.XpLevels == null)
                return NotFound();

            var xpLevel = await _context.XpLevels.FindAsync(id);

            if (xpLevel == null)
                return NotFound();

            _context.XpLevels.Remove(xpLevel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool XpLevelExists(Guid id) => (_context.XpLevels?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
