using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CreaturesController : DatabaseController
    {
        public CreaturesController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetCreatures()
        {
            return _context.Creatures == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Creatures.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCreature(Guid? id)
        {
            if (id == null || _context.Creatures == null)
                return NotFound();

            var creature = await _context.Creatures.FindAsync(id);

            if (creature == null)
                return NotFound();

            return Ok(creature);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCreature([Bind("Id,AssetName,RequiredXpLevelId,Health,MovementSpeed")] Creature creature)
        {
            if (ModelState.IsValid)
            {
                creature.Id = Guid.NewGuid();
                _context.Add(creature);
                await _context.SaveChangesAsync();
                return Ok(creature);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCreature(Guid? id, [Bind("Id,AssetName,RequiredXpLevelId,Health,MovementSpeed")] Creature creature)
        {
            if (id == null || _context.Creatures == null)
                return NotFound();

            if (id != creature.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreatureExists(creature.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(creature);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCreature(Guid? id)
        {
            if (id == null || _context.Creatures == null)
                return NotFound();

            var creature = await _context.Creatures.FindAsync(id);

            if (creature == null)
                return NotFound();

            _context.Creatures.Remove(creature);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool CreatureExists(Guid id) => (_context.Creatures?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
