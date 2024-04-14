using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class CreaturesController : DatabaseEntityController<Creature>
{
    public CreaturesController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication)
            .GetAllAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetAsync(creature => creature.Id == id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([Bind(
        nameof(Creature.Id),
        nameof(Creature.AssetName),
        nameof(Creature.RequiredXpLevelId),
        nameof(Creature.Health),
        nameof(Creature.MovementSpeed)
    )] Creature creature)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateAsync(creature);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Creature.Id),
        nameof(Creature.AssetName),
        nameof(Creature.RequiredXpLevelId),
        nameof(Creature.Health),
        nameof(Creature.MovementSpeed)
    )] Creature creature)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditAsync(id, creature);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteAsync(id);
}
