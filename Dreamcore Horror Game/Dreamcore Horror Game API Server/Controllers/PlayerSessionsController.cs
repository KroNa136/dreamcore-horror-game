using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class PlayerSessionsController : DatabaseEntityController<PlayerSession>
{
    public PlayerSessionsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetAsync(playerSession => playerSession.Id == id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([Bind(
        nameof(PlayerSession.Id),
        nameof(PlayerSession.GameSessionId),
        nameof(PlayerSession.PlayerId),
        nameof(PlayerSession.StartTimestamp),
        nameof(PlayerSession.EndTimestamp),
        nameof(PlayerSession.IsCompleted),
        nameof(PlayerSession.IsWon),
        nameof(PlayerSession.TimeAlive),
        nameof(PlayerSession.PlayedAsCreature),
        nameof(PlayerSession.UsedCreatureId),
        nameof(PlayerSession.SelfReviveCount),
        nameof(PlayerSession.AllyReviveCount)
    )] PlayerSession playerSession)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .CreateAsync(playerSession);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(PlayerSession.Id),
        nameof(PlayerSession.GameSessionId),
        nameof(PlayerSession.PlayerId),
        nameof(PlayerSession.StartTimestamp),
        nameof(PlayerSession.EndTimestamp),
        nameof(PlayerSession.IsCompleted),
        nameof(PlayerSession.IsWon),
        nameof(PlayerSession.TimeAlive),
        nameof(PlayerSession.PlayedAsCreature),
        nameof(PlayerSession.UsedCreatureId),
        nameof(PlayerSession.SelfReviveCount),
        nameof(PlayerSession.AllyReviveCount)
    )] PlayerSession playerSession)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .EditAsync(id, playerSession);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteAsync(id);
}
