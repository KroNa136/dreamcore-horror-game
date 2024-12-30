using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Extensions;
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
public class PlayersController : UserController<Player>
{
    public PlayersController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator,
        ILogger<PlayersController> logger,
        ITokenService tokenService,
        IPasswordHasher<Player> passwordHasher
    )
    : base
    (
        context: context,
        propertyPredicateValidator: propertyPredicateValidator,
        logger: logger,
        tokenService: tokenService,
        passwordHasher: passwordHasher,
        alreadyExistsErrorMessage: ErrorMessages.PlayerAlreadyExists,
        orderBySelectorExpression: player => player.Username,
        orderByComparer: null,
        getAllWithFirstLevelRelationsFunction: async (context) =>
        {
            var xpLevels = await context.XpLevels.ToListAsync();
            var acquiredAbilities = await context.AcquiredAbilities.ToListAsync();
            var collectedArtifacts = await context.CollectedArtifacts.ToListAsync();
            var playerSessions = await context.PlayerSessions.ToListAsync();

            var players = context.Players.AsQueryable();

            await players.ForEachAsync(player =>
            {
                player.XpLevel?.Players.Clear();
            });

            return players;
        },
        setRelationsFromForeignKeysFunction: async (context, player) =>
        {
            var xpLevel = await context.XpLevels
                .FindAsync(player.XpLevelId);

            if (xpLevel is null)
                throw new InvalidConstraintException();

            player.XpLevel = xpLevel;
            player.XpLevelId = Guid.Empty;
        },
        getByLoginFunction: async (context, login) =>
        {
            return await context.Players
                .FirstOrDefaultAsync(player => player.Email.Equals(login));
        }
    )
    { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetCount()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetCountAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAllWithRelations(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
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
        nameof(Player.Id),
        nameof(Player.Username),
        nameof(Player.Email),
        nameof(Player.Password),
        nameof(Player.RefreshToken),
        nameof(Player.RegistrationTimestamp),
        nameof(Player.CollectOptionalData),
        nameof(Player.IsOnline),
        nameof(Player.XpLevelId),
        nameof(Player.Xp),
        nameof(Player.AbilityPoints),
        nameof(Player.SpiritEnergyPoints)
    )] Player player)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateEntityAsync(player);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Player.Id),
        nameof(Player.Username),
        nameof(Player.Email),
        nameof(Player.Password),
        nameof(Player.RefreshToken),
        nameof(Player.RegistrationTimestamp),
        nameof(Player.CollectOptionalData),
        nameof(Player.IsOnline),
        nameof(Player.XpLevelId),
        nameof(Player.Xp),
        nameof(Player.AbilityPoints),
        nameof(Player.SpiritEnergyPoints)
    )] Player player)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditEntityAsync(id, player);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteEntityAsync(id);

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([Bind(
        nameof(Player.Id),
        nameof(Player.Username),
        nameof(Player.Email),
        nameof(Player.Password),
        nameof(Player.RefreshToken),
        nameof(Player.RegistrationTimestamp),
        nameof(Player.CollectOptionalData),
        nameof(Player.IsOnline),
        nameof(Player.XpLevelId),
        nameof(Player.Xp),
        nameof(Player.AbilityPoints),
        nameof(Player.SpiritEnergyPoints)
    )] Player player)
        => await RequireHeaders(CorsHeaders.GameClient)
            .ExecuteAsync(player, async player =>
            {
                _logger.LogInformation("Register called for {EntityType}", EntityType);

                bool playerExists = await _getByLogin(_context, player.Login) is not null;

                if (playerExists)
                    return UnprocessableEntity(_alreadyExistsErrorMessage);

                if (InvalidModelState)
                    return ValidationProblem();

                bool noFirstXpLevel = await _context.XpLevels.AnyAsync(xpLevel => xpLevel.Number == 1) is false;

                if (noFirstXpLevel)
                {
                    _logger.LogError
                    (
                        eventId: new EventId("Register".GetHashCode() + 1),
                        message: "Couldn't register a new player: the XP level with number 1 was not found in the database"
                    );

                    return this.InternalServerError();
                }

                player.Id = Guid.NewGuid();
                player.Password = _passwordHasher.HashPassword(player, player.Password);
                player.RefreshToken = _tokenService.CreateRefreshToken(player.Login, player.Role);
                player.IsOnline = true;
                player.XpLevelId = _context.XpLevels.First(xpLevel => xpLevel.Number == 1).Id;

                if (_setRelationsFromForeignKeys is not null)
                    await _setRelationsFromForeignKeys(_context, player);

                _context.Add(player);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError
                    (
                        eventId: new EventId("Register".GetHashCode() + ex.GetType().GetHashCode()),
                        message: "Database conflict occured while registering player with id = {id}", player.Id
                    );

                    return Conflict(ErrorMessages.PlayerRegisterConflict);
                }

                return Ok(player.RefreshToken);
            });

    [HttpPost]
    [AllowAnonymous]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await RequireHeaders(CorsHeaders.GameClient)
            .LoginAsUserAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> Logout(string email)
        => await RequireHeaders(CorsHeaders.GameClient)
            .LogoutAsUserAsync(email);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await RequireHeaders(CorsHeaders.GameClient)
            .ChangeUserPasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> GetAccessToken(string email)
        => await RequireHeaders(CorsHeaders.GameClient)
            .GetAccessTokenForUserAsync(email);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> VerifyAccessToken()
        => await RequireHeaders(CorsHeaders.GameClient)
            .VerifyAccessTokenAsync();
}
