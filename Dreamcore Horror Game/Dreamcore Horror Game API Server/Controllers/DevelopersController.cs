using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Models.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class DevelopersController : UserController<Developer>
{
    public DevelopersController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator,
        ITokenService tokenService,
        IPasswordHasher<Developer> passwordHasher
    )
    : base
    (
        context: context,
        propertyPredicateValidator: propertyPredicateValidator,
        tokenService: tokenService,
        passwordHasher: passwordHasher,
        alreadyExistsErrorMessage: ErrorMessages.DeveloperAlreadyExists,
        orderBySelector: developer => developer.Login,
        getAllWithFirstLevelRelationsFunction: async (context) =>
        {
            var developerAccessLevels = await context.DeveloperAccessLevels.ToListAsync();

            var developers = context.Developers.AsQueryable();

            await developers.ForEachAsync(developer =>
            {
                developer.DeveloperAccessLevel.Developers.Clear();
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
    { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetAllWithRelations()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesWithRelationsAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntityAsync(id);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> GetWithRelations(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntityWithRelationsAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntitiesWhereAsync(predicateCollection);

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetByLogin(string login)
        => RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntity(developer => developer.Login.Equals(login));

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Create([Bind(
        nameof(Developer.Id),
        nameof(Developer.Login),
        nameof(Developer.Password),
        nameof(Developer.RefreshToken),
        nameof(Developer.DeveloperAccessLevelId)
    )] Developer developer)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
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
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditEntityAsync(id, developer);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteEntityAsync(id);

    [HttpPost]
    [AllowAnonymous]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .LoginAsUserAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> Logout(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .LogoutAsUserAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .ChangeUserPasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAccessToken(string login)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAccessTokenForUserAsync(login);
}
