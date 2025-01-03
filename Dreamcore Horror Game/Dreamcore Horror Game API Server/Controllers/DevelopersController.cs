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
    ITokenService tokenService,
    IPasswordHasher<Developer> passwordHasher
)
: UserController<Developer>
(
    context: context,
    propertyPredicateValidator: propertyPredicateValidator,
    logger: logger,
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
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetCountAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetAll(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetAllEntitiesAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetAllWithRelations(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetAllEntitiesWithRelationsAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetEntityAsync(id);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetWithRelations(Guid? id)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetEntityWithRelationsAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection, int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetEntitiesWhereAsync(predicateCollection, page, showBy);

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByLogin(string login)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
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
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
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
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .EditEntityAsync(id, developer);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .DeleteEntityAsync(id);

    [HttpPost]
    [AllowAnonymous]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .LoginAsUserAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> Logout(string login)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .LogoutAsUserAsync(login);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .ChangeUserPasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAccessToken(string login)
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .GetAccessTokenForUserAsync(login);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> VerifyAccessToken()
        => await RequireHeaders(CorsHeaders.ApplicationForDevelopers)
            .VerifyAccessTokenAsync();
}
