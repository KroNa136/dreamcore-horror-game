using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class GameModesController : DatabaseEntityController<GameMode>
{
    public GameModesController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator
    )
    : base
    (
        context: context,
        propertyPredicateValidator: propertyPredicateValidator,
        orderBySelector: gameMode => gameMode.AssetName,
        getAllWithFirstLevelRelationsFunction: async (context) =>
        {
            var gameSessions = await context.GameSessions.ToListAsync();

            return context.GameModes.AsQueryable();
        },
        setRelationsFromForeignKeysFunction: null
    )
    { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public override async Task<IActionResult> GetAll(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public override async Task<IActionResult> GetAllWithRelations(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesWithRelationsAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetEntityAsync(id);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> GetWithRelations(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetEntityWithRelationsAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection, int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntitiesWhereAsync(predicateCollection, page, showBy);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    public override async Task<IActionResult> Create([Bind(
        nameof(GameMode.Id),
        nameof(GameMode.AssetName),
        nameof(GameMode.MaxPlayers),
        nameof(GameMode.TimeLimit),
        nameof(GameMode.IsActive)
    )] GameMode gameMode)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateEntityAsync(gameMode);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(GameMode.Id),
        nameof(GameMode.AssetName),
        nameof(GameMode.MaxPlayers),
        nameof(GameMode.TimeLimit),
        nameof(GameMode.IsActive)
    )] GameMode gameMode)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditEntityAsync(id, gameMode);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteEntityAsync(id);
}
