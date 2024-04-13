using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class GameSessionsController : DatabaseController
{
    public GameSessionsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : Ok(await _context.GameSessions.ToListAsync());

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public async Task<IActionResult> Get(Guid? id)
        => NoHeader(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : id is not null
                && await _context.GameSessions.FindAsync(id) is GameSession gameSession
            ? Ok(gameSession)
            : NotFound();

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(GameSession.Id),
        nameof(GameSession.ServerId),
        nameof(GameSession.GameModeId),
        nameof(GameSession.StartTimestamp),
        nameof(GameSession.EndTimestamp)
    )] GameSession gameSession)
    {
        if (NoHeader(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (ModelState.IsValid)
        {
            gameSession.Id = Guid.NewGuid();

            _context.Add(gameSession);
            await _context.SaveChangesAsync();
            return Ok(gameSession);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(GameSession.Id),
        nameof(GameSession.ServerId),
        nameof(GameSession.GameModeId),
        nameof(GameSession.StartTimestamp),
        nameof(GameSession.EndTimestamp)
    )] GameSession gameSession)
    {
        if (NoHeader(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != gameSession.Id)
            return BadRequest(ErrorMessages.IdMismatch);

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

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is not null && await _context.GameSessions.FindAsync(id) is GameSession gameSession)
        {
            _context.Remove(gameSession);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }

    private bool GameSessionExists(Guid id)
        => _context.GameSessions.Any(gameSession => gameSession.Id == id);
}
