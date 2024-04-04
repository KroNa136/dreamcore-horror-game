using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RarityLevelsController : DatabaseController
    {
        public RarityLevelsController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetRarityLevels()
        {
            return _context.RarityLevels == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.RarityLevels.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetRarityLevel(Guid? id)
        {
            if (id == null || _context.RarityLevels == null)
                return NotFound();

            var rarityLevel = await _context.RarityLevels.FindAsync(id);

            if (rarityLevel == null)
                return NotFound();

            return Ok(rarityLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRarityLevel([Bind("Id,AssetName,Probability")] RarityLevel rarityLevel)
        {
            if (ModelState.IsValid)
            {
                rarityLevel.Id = Guid.NewGuid();
                _context.Add(rarityLevel);
                await _context.SaveChangesAsync();
                return Ok(rarityLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRarityLevel(Guid? id, [Bind("Id,AssetName,Probability")] RarityLevel rarityLevel)
        {
            if (id == null || _context.RarityLevels == null)
                return NotFound();

            if (id != rarityLevel.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rarityLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RarityLevelExists(rarityLevel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(rarityLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRarityLevel(Guid? id)
        {
            if (id == null || _context.RarityLevels == null)
                return NotFound();

            var rarityLevel = await _context.RarityLevels.FindAsync(id);

            if (rarityLevel == null)
                return NotFound();

            _context.RarityLevels.Remove(rarityLevel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool RarityLevelExists(Guid id) => (_context.RarityLevels?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
