﻿using DreamcoreHorrorGameApiServer.ConstantValues;
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
public class AcquiredAbilitiesController
(
    DreamcoreHorrorGameContext context,
    IPropertyPredicateValidator propertyPredicateValidator,
    ILogger<AcquiredAbilitiesController> logger
)
: DatabaseEntityController<AcquiredAbility>
(
    context: context,
    propertyPredicateValidator: propertyPredicateValidator,
    logger: logger,
    orderBySelectorExpression: acquiredAbility => acquiredAbility.AcquirementTimestamp,
    orderByComparer: null,
    getAllWithFirstLevelRelationsFunction: async (context) =>
    {
        var players = await context.Players.ToListAsync();
        var abilities = await context.Abilities.ToListAsync();

        var acquiredAbilities = context.AcquiredAbilities.AsQueryable();

        await acquiredAbilities.ForEachAsync(acquiredAbility =>
        {
            acquiredAbility.Player?.AcquiredAbilities.Clear();
            acquiredAbility.Ability?.AcquiredAbilities.Clear();
        });

        return acquiredAbilities;
    },
    setRelationsFromForeignKeysFunction: async (context, acquiredAbility) =>
    {
        var player = await context.Players
            .FindAsync(acquiredAbility.PlayerId);

        var ability = await context.Abilities
            .FindAsync(acquiredAbility.AbilityId);

        if (player is null || ability is null)
            throw new InvalidConstraintException();

        acquiredAbility.Player = player;
        acquiredAbility.PlayerId = Guid.Empty;

        acquiredAbility.Ability = ability;
        acquiredAbility.AbilityId = Guid.Empty;
    }
)
{

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetCount()
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetCountAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetAllEntitiesAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAllWithRelations(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetAllEntitiesWithRelationsAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.ApplicationForDevelopers)
            .GetEntityAsync(id);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public override async Task<IActionResult> GetWithRelations(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.ApplicationForDevelopers)
            .GetEntityWithRelationsAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection, int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetEntitiesWhereAsync(predicateCollection, page, showBy);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    public override async Task<IActionResult> Create([Bind(
        nameof(AcquiredAbility.Id),
        nameof(AcquiredAbility.PlayerId),
        nameof(AcquiredAbility.AbilityId),
        nameof(AcquiredAbility.AcquirementTimestamp)
    )] AcquiredAbility acquiredAbility)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .CreateEntityAsync(acquiredAbility);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(AcquiredAbility.Id),
        nameof(AcquiredAbility.PlayerId),
        nameof(AcquiredAbility.AbilityId),
        nameof(AcquiredAbility.AcquirementTimestamp)
    )] AcquiredAbility acquiredAbility)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .EditEntityAsync(id, acquiredAbility);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .DeleteEntityAsync(id);
}
