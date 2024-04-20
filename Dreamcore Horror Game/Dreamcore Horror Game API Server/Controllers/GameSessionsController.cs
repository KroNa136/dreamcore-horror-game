using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class GameSessionsController : DatabaseEntityController<GameSession>
{
    public GameSessionsController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator
    )
    : base
    (
        context: context,
        propertyPredicateValidator: propertyPredicateValidator,
        orderBySelector: gameSession => gameSession.StartTimestamp,
        getAllWithFirstLevelRelationsFunction: async (context) =>
        {
            var servers = await context.Servers.ToListAsync();
            var gameModes = await context.GameModes.ToListAsync();
            var playerSessions = await context.PlayerSessions.ToListAsync();

            var gameSessions = context.GameSessions.AsQueryable();

            await gameSessions.ForEachAsync(gameSession =>
            {
                gameSession.GameMode.GameSessions.Clear();
                gameSession.Server?.GameSessions.Clear();
            });

            return gameSessions;
        },
        setRelationsFromForeignKeysFunction: async (context, gameSession) =>
        {
            var server = await context.Servers
                .FindAsync(gameSession.ServerId);

            var gameMode = await context.GameModes
                .FindAsync(gameSession.GameModeId);

            if (server is null || gameMode is null)
                throw new InvalidConstraintException();

            gameSession.Server = server;
            gameSession.ServerId = Guid.Empty;

            gameSession.GameMode = gameMode;
            gameSession.GameModeId = Guid.Empty;
        }
    )
    { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAllWithRelations()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesWithRelationsAsync();

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
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntitiesWhereAsync(predicateCollection);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    public override async Task<IActionResult> Create([Bind(
        nameof(GameSession.Id),
        nameof(GameSession.ServerId),
        nameof(GameSession.GameModeId),
        nameof(GameSession.StartTimestamp),
        nameof(GameSession.EndTimestamp)
    )] GameSession gameSession)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .CreateEntityAsync(gameSession);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(GameSession.Id),
        nameof(GameSession.ServerId),
        nameof(GameSession.GameModeId),
        nameof(GameSession.StartTimestamp),
        nameof(GameSession.EndTimestamp)
    )] GameSession gameSession)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .EditEntityAsync(id, gameSession);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteEntityAsync(id);
}
