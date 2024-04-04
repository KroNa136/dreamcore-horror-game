using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PlayersController : DatabaseController
    {
        public PlayersController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            return _context.Players == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Players.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayer(Guid? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            return Ok(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlayer([Bind("Id,Username,Email,Password,RegistrationTimestamp,CollectOptionalData,IsOnline,XpLevelId,Xp,AbilityPoints,SpiritEnergyPoints")] Player player)
        {
            if (ModelState.IsValid)
            {
                player.Id = Guid.NewGuid();
                _context.Add(player);
                await _context.SaveChangesAsync();
                return Ok(player);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlayer(Guid? id, [Bind("Id,Username,Email,Password,RegistrationTimestamp,CollectOptionalData,IsOnline,XpLevelId,Xp,AbilityPoints,SpiritEnergyPoints")] Player player)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            if (id != player.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(player);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlayer(Guid? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool PlayerExists(Guid id) => (_context.Players?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
