using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PlayerSessionsController : DatabaseController
    {
        public PlayerSessionsController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetPlayerSessions()
        {
            return _context.PlayerSessions == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.PlayerSessions.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayerSession(Guid? id)
        {
            if (id == null || _context.PlayerSessions == null)
                return NotFound();

            var playerSession = await _context.PlayerSessions.FindAsync(id);

            if (playerSession == null)
                return NotFound();

            return Ok(playerSession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlayerSession([Bind("Id,GameSessionId,PlayerId,StartTimestamp,EndTimestamp,IsCompleted,IsWon,TimeAlive,PlayedAsCreature,UsedCreatureId,SelfReviveCount,AllyReviveCount")] PlayerSession playerSession)
        {
            if (ModelState.IsValid)
            {
                playerSession.Id = Guid.NewGuid();
                _context.Add(playerSession);
                await _context.SaveChangesAsync();
                return Ok(playerSession);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlayerSession(Guid? id, [Bind("Id,GameSessionId,PlayerId,StartTimestamp,EndTimestamp,IsCompleted,IsWon,TimeAlive,PlayedAsCreature,UsedCreatureId,SelfReviveCount,AllyReviveCount")] PlayerSession playerSession)
        {
            if (id == null || _context.PlayerSessions == null)
                return NotFound();

            if (id != playerSession.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playerSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerSessionExists(playerSession.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(playerSession);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlayerSession(Guid? id)
        {
            if (id == null || _context.PlayerSessions == null)
                return NotFound();

            var playerSession = await _context.PlayerSessions.FindAsync(id);

            if (playerSession == null)
                return NotFound();

            _context.PlayerSessions.Remove(playerSession);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool PlayerSessionExists(Guid id) => (_context.PlayerSessions?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
