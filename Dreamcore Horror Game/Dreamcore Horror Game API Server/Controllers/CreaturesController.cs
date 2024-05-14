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
public class CreaturesController : DatabaseEntityController<Creature>
{
    public CreaturesController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator
    )
    : base
    (
        context: context,
        propertyPredicateValidator: propertyPredicateValidator,
        orderBySelectorExpression: creature => creature.AssetName,
        orderByComparer: null,
        getAllWithFirstLevelRelationsFunction: async (context) =>
        {
            var xpLevels = await context.XpLevels.ToListAsync();
            var playerSessions = await context.PlayerSessions.ToListAsync();

            var creatures = context.Creatures.AsQueryable();

            await creatures.ForEachAsync(creature =>
            {
                creature.RequiredXpLevel?.Creatures.Clear();
            });

            return creatures;
        },
        setRelationsFromForeignKeysFunction: async (context, creature) =>
        {
            var requiredXpLevel = await context.XpLevels
                .FindAsync(creature.RequiredXpLevelId);

            if (requiredXpLevel is null)
                throw new InvalidConstraintException();

            creature.RequiredXpLevel = requiredXpLevel;
            creature.RequiredXpLevelId = Guid.Empty;
        }
    )
    { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetCount()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetCountAsync();

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
        nameof(Creature.Id),
        nameof(Creature.AssetName),
        nameof(Creature.RequiredXpLevelId),
        nameof(Creature.Health),
        nameof(Creature.MovementSpeed)
    )] Creature creature)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateEntityAsync(creature);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Creature.Id),
        nameof(Creature.AssetName),
        nameof(Creature.RequiredXpLevelId),
        nameof(Creature.Health),
        nameof(Creature.MovementSpeed)
    )] Creature creature)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditEntityAsync(id, creature);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteEntityAsync(id);
}
