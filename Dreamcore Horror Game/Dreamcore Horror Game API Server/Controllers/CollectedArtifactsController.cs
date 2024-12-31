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
public class CollectedArtifactsController
(
    DreamcoreHorrorGameContext context,
    IPropertyPredicateValidator propertyPredicateValidator,
    ILogger<CollectedArtifactsController> logger
)
: DatabaseEntityController<CollectedArtifact>
(
    context: context,
    propertyPredicateValidator: propertyPredicateValidator,
    logger: logger,
    orderBySelectorExpression: collectedArtifact => collectedArtifact.CollectionTimestamp,
    orderByComparer: null,
    getAllWithFirstLevelRelationsFunction: async (context) =>
    {
        var players = await context.Players.ToListAsync();
        var artifacts = await context.Artifacts.ToListAsync();

        var collectedArtifacts = context.CollectedArtifacts.AsQueryable();

        await collectedArtifacts.ForEachAsync(collectedArtifact =>
        {
            collectedArtifact.Player?.CollectedArtifacts.Clear();
            collectedArtifact.Artifact?.CollectedArtifacts.Clear();
        });

        return collectedArtifacts;
    },
    setRelationsFromForeignKeysFunction: async (context, collectedArtifact) =>
    {
        var player = await context.Players
            .FindAsync(collectedArtifact.PlayerId);

        var artifact = await context.Artifacts
            .FindAsync(collectedArtifact.ArtifactId);

        if (player is null || artifact is null)
            throw new InvalidConstraintException();

        collectedArtifact.Player = player;
        collectedArtifact.PlayerId = Guid.Empty;

        collectedArtifact.Artifact = artifact;
        collectedArtifact.ArtifactId = Guid.Empty;
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
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    public override async Task<IActionResult> Create([Bind(
        nameof(CollectedArtifact.Id),
        nameof(CollectedArtifact.PlayerId),
        nameof(CollectedArtifact.ArtifactId),
        nameof(CollectedArtifact.CollectionTimestamp)
    )] CollectedArtifact collectedArtifact)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.ApplicationForDevelopers)
            .CreateEntityAsync(collectedArtifact);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(CollectedArtifact.Id),
        nameof(CollectedArtifact.PlayerId),
        nameof(CollectedArtifact.ArtifactId),
        nameof(CollectedArtifact.CollectionTimestamp)
    )] CollectedArtifact collectedArtifact)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .EditEntityAsync(id, collectedArtifact);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .DeleteEntityAsync(id);
}
