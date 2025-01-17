﻿using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class DevelopersController
(
    DreamcoreHorrorGameContext context,
    IPropertyPredicateValidator propertyPredicateValidator,
    ILogger<DevelopersController> logger,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider,
    IRabbitMqProducer rabbitMqProducer,
    ITokenService tokenService,
    IPasswordHasher<Developer> passwordHasher
)
: UserController<Developer>
(
    context: context,
    propertyPredicateValidator: propertyPredicateValidator,
    logger: logger,
    jsonSerializerOptionsProvider: jsonSerializerOptionsProvider,
    rabbitMqProducer: rabbitMqProducer,
    tokenService: tokenService,
    passwordHasher: passwordHasher,
    alreadyExistsErrorMessage: ErrorMessages.DeveloperAlreadyExists,
    orderBySelectorExpression: developer => developer.Login,
    orderByComparer: null,
    getAllWithFirstLevelRelationsFunction: async (context) =>
    {
        var developerAccessLevels = await context.DeveloperAccessLevels.ToListAsync();

        var developers = context.Developers.AsQueryable();

        await developers.ForEachAsync(developer =>
        {
            developer.DeveloperAccessLevel?.Developers.Clear();
        });

        return developers;
    },
    setRelationsFromForeignKeysFunction: async (context, developer) =>
    {
        var developerAccessLevel = await context.DeveloperAccessLevels
            .FindAsync(developer.DeveloperAccessLevelId);

        if (developerAccessLevel is null)
            throw new InvalidConstraintException();

        developer.DeveloperAccessLevel = developerAccessLevel;
        developer.DeveloperAccessLevelId = Guid.Empty;
    },
    getByLoginFunction: async (context, login) =>
    {
        return await context.Developers
            .Include(developer => developer.DeveloperAccessLevel)
            .FirstOrDefaultAsync(developer => developer.Login.Equals(login));
    }
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

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByLogin(string login)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetEntityWithRelationsAsync(developer => developer.Login.Equals(login));

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Create([Bind(
        nameof(Developer.Id),
        nameof(Developer.Login),
        nameof(Developer.Password),
        nameof(Developer.RefreshToken),
        nameof(Developer.DeveloperAccessLevelId)
    )] Developer developer)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .CreateEntityAsync(developer);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Developer.Id),
        nameof(Developer.Login),
        nameof(Developer.Password),
        nameof(Developer.RefreshToken),
        nameof(Developer.DeveloperAccessLevelId)
    )] Developer developer)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .EditEntityAsync(id, developer);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .DeleteEntityAsync(id);

    [HttpPost]
    [AllowAnonymous]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .LoginAsUserAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> Logout(string login)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .LogoutAsUserAsync(login);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .ChangeUserPasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAccessToken(string login)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetAccessTokenForUserAsync(login);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> VerifyAccessToken()
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .VerifyAccessTokenAsync();
}
