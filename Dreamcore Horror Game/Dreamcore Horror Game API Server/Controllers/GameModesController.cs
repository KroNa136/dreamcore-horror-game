using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class GameModesController : DatabaseEntityController<GameMode>
{
    public GameModesController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication)
            .GetAllAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetAsync(gameMode => gameMode.Id == id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([Bind(
        nameof(GameMode.Id),
        nameof(GameMode.AssetName),
        nameof(GameMode.MaxPlayers),
        nameof(GameMode.TimeLimit),
        nameof(GameMode.IsActive)
    )] GameMode gameMode)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateAsync(gameMode);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(GameMode.Id),
        nameof(GameMode.AssetName),
        nameof(GameMode.MaxPlayers),
        nameof(GameMode.TimeLimit),
        nameof(GameMode.IsActive)
    )] GameMode gameMode)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditAsync(id, gameMode);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteAsync(id);
}
