using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GameSessionsController : DatabaseController
    {
        public GameSessionsController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetGameSessions()
        {
            return _context.GameSessions == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.GameSessions.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetGameSession(Guid? id)
        {
            if (id == null || _context.GameSessions == null)
                return NotFound();

            var gameSession = await _context.GameSessions.FindAsync(id);

            if (gameSession == null)
                return NotFound();

            return Ok(gameSession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGameSession([Bind("Id,ServerId,GameModeId,StartTimestamp,EndTimestamp")] GameSession gameSession)
        {
            if (ModelState.IsValid)
            {
                gameSession.Id = Guid.NewGuid();
                _context.Add(gameSession);
                await _context.SaveChangesAsync();
                return Ok(gameSession);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGameSession(Guid? id, [Bind("Id,ServerId,GameModeId,StartTimestamp,EndTimestamp")] GameSession gameSession)
        {
            if (id == null || _context.GameSessions == null)
                return NotFound();

            if (id != gameSession.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameSessionExists(gameSession.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(gameSession);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGameSession(Guid? id)
        {
            if (id == null || _context.GameSessions == null)
                return NotFound();

            var gameSession = await _context.GameSessions.FindAsync(id);

            if (gameSession == null)
                return NotFound();

            _context.GameSessions.Remove(gameSession);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool GameSessionExists(Guid id) => (_context.GameSessions?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
