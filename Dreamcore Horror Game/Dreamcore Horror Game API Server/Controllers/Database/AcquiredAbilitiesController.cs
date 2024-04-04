using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AcquiredAbilitiesController : DatabaseController
    {
        public AcquiredAbilitiesController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetAcquiredAbilities()
        {
            return _context.AcquiredAbilities == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.AcquiredAbilities.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAcquiredAbility(Guid? id)
        {
            if (id == null || _context.AcquiredAbilities == null)
                return NotFound();

            var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);

            if (acquiredAbility == null)
                return NotFound();

            return Ok(acquiredAbility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAcquiredAbility([Bind("Id,PlayerId,AbilityId,AcquirementTimestamp")] AcquiredAbility acquiredAbility)
        {
            if (ModelState.IsValid)
            {
                acquiredAbility.Id = Guid.NewGuid();
                _context.Add(acquiredAbility);
                await _context.SaveChangesAsync();
                return Ok(acquiredAbility);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAcquiredAbility(Guid? id, [Bind("Id,PlayerId,AbilityId,AcquirementTimestamp")] AcquiredAbility acquiredAbility)
        {
            if (id == null || _context.AcquiredAbilities == null)
                return NotFound();

            if (id != acquiredAbility.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acquiredAbility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcquiredAbilityExists(acquiredAbility.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(acquiredAbility);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAcquiredAbility(Guid? id)
        {
            if (id == null || _context.AcquiredAbilities == null)
                return NotFound();

            var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);

            if (acquiredAbility == null)
                return NotFound();

            _context.AcquiredAbilities.Remove(acquiredAbility);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AcquiredAbilityExists(Guid id) => (_context.AcquiredAbilities?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
