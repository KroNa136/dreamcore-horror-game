﻿using DreamcoreHorrorGameApiServer.ConstantValues;
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
public class DeveloperAccessLevelsController
(
    DreamcoreHorrorGameContext context,
    IPropertyPredicateValidator propertyPredicateValidator,
    ILogger<DeveloperAccessLevelsController> logger,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider,
    IRabbitMqProducer rabbitMqProducer
)
: DatabaseEntityController<DeveloperAccessLevel>
(
    context: context,
    propertyPredicateValidator: propertyPredicateValidator,
    logger: logger,
    jsonSerializerOptionsProvider: jsonSerializerOptionsProvider,
    rabbitMqProducer: rabbitMqProducer,
    orderBySelectorExpression: developerAccessLevel => developerAccessLevel.Name,
    orderByComparer: null,
    getAllWithFirstLevelRelationsFunction: async (context) =>
    {
        var developers = await context.Developers.ToListAsync();

        return context.DeveloperAccessLevels.AsQueryable();
    },
    setRelationsFromForeignKeysFunction: null
)
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetCount()
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetCountAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetAll(int page = 0, int showBy = 0)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetAllEntitiesAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetAllWithRelations(int page = 0, int showBy = 0)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetAllEntitiesWithRelationsAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Get(Guid? id)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetEntityAsync(id);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetWithRelations(Guid? id)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetEntityWithRelationsAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection, int page = 0, int showBy = 0)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetEntitiesWhereAsync(predicateCollection, page, showBy);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Create([Bind(
        nameof(DeveloperAccessLevel.Id),
        nameof(DeveloperAccessLevel.Name)
    )] DeveloperAccessLevel developerAccessLevel)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .CreateEntityAsync(developerAccessLevel);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(DeveloperAccessLevel.Id),
        nameof(DeveloperAccessLevel.Name)
    )] DeveloperAccessLevel developerAccessLevel)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .EditEntityAsync(id, developerAccessLevel);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .DeleteEntityAsync(id);
}
