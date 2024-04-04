using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GameModesController : DatabaseController
    {
        public GameModesController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetGameModes()
        {
            return _context.GameModes == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.GameModes.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetGameMode(Guid? id)
        {
            if (id == null || _context.GameModes == null)
                return NotFound();

            var gameMode = await _context.GameModes.FindAsync(id);

            if (gameMode == null)
                return NotFound();

            return Ok(gameMode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGameMode([Bind("Id,AssetName,MaxPlayers,TimeLimit,IsActive")] GameMode gameMode)
        {
            if (ModelState.IsValid)
            {
                gameMode.Id = Guid.NewGuid();
                _context.Add(gameMode);
                await _context.SaveChangesAsync();
                return Ok(gameMode);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGameMode(Guid? id, [Bind("Id,AssetName,MaxPlayers,TimeLimit,IsActive")] GameMode gameMode)
        {
            if (id == null || _context.GameModes == null)
                return NotFound();

            if (id != gameMode.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameMode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameModeExists(gameMode.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(gameMode);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGameMode(Guid? id)
        {
            if (id == null || _context.GameModes == null)
                return NotFound();

            var gameMode = await _context.GameModes.FindAsync(id);

            if (gameMode == null)
                return NotFound();

            _context.GameModes.Remove(gameMode);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool GameModeExists(Guid id) => (_context.GameModes?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
