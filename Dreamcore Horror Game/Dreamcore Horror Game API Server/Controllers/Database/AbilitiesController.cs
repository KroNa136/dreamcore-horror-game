using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AbilitiesController : DatabaseController
    {
        public AbilitiesController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetAbilities()
        {
            return _context.Abilities == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Abilities.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAbility(Guid? id)
        {
            if (id == null || _context.Abilities == null)
                return NotFound();

            var ability = await _context.Abilities.FindAsync(id);

            if (ability == null)
                return NotFound();

            return Ok(ability);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAbility([Bind("Id,AssetName")] Ability ability)
        {
            if (ModelState.IsValid)
            {
                ability.Id = Guid.NewGuid();
                _context.Add(ability);
                await _context.SaveChangesAsync();
                return Ok(ability);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbility(Guid? id, [Bind("Id,AssetName")] Ability ability)
        {
            if (id == null || _context.Abilities == null)
                return NotFound();

            if (id != ability.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbilityExists(ability.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(ability);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAbility(Guid? id)
        {
            if (id == null || _context.Abilities == null)
                return NotFound();

            var ability = await _context.Abilities.FindAsync(id);

            if (ability == null)
                return NotFound();

            _context.Abilities.Remove(ability);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool AbilityExists(Guid id) => (_context.Abilities?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
