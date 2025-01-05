using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Models.DTO;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class PlayersController
(
    DreamcoreHorrorGameContext context,
    IPropertyPredicateValidator propertyPredicateValidator,
    ILogger<PlayersController> logger,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider,
    IRabbitMqProducer rabbitMqProducer,
    ITokenService tokenService,
    IPasswordHasher<Player> passwordHasher
)
: UserController<Player>
(
    context: context,
    propertyPredicateValidator: propertyPredicateValidator,
    logger: logger,
    jsonSerializerOptionsProvider: jsonSerializerOptionsProvider,
    rabbitMqProducer: rabbitMqProducer,
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
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetCount()
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetCountAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll(int page = 0, int showBy = 0)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetAllEntitiesAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAllWithRelations(int page = 0, int showBy = 0)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .GetAllEntitiesWithRelationsAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await AllowRequestSenders(RequestSenders.GameClient, RequestSenders.GameServer, RequestSenders.ApplicationForDevelopers)
            .GetEntityAsync(id);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> GetWithRelations(Guid? id)
        => await AllowRequestSenders(RequestSenders.GameClient, RequestSenders.GameServer, RequestSenders.ApplicationForDevelopers)
            .GetEntityWithRelationsAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection, int page = 0, int showBy = 0)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
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
        nameof(Player.SpiritEnergyPoints),
        nameof(Player.EmailVerified),
        nameof(Player.EmailVerificationTokenId)
    )] Player player)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
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
        nameof(Player.SpiritEnergyPoints),
        nameof(Player.EmailVerified),
        nameof(Player.EmailVerificationTokenId)
    )] Player player)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .EditEntityAsync(id, player);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
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
        nameof(Player.SpiritEnergyPoints),
        nameof(Player.EmailVerified),
        nameof(Player.EmailVerificationTokenId)
    )] Player player)
        => await AllowRequestSenders(RequestSenders.GameClient, RequestSenders.PublicWebsite)
            .ExecuteAsync(player, async player =>
            {
                _logger.LogInformation("Register was called for {EntityType}.", EntityType);
                PublishStatistics("Register");

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
                        message: "The XP level with number 1 was not found in the database while registering {EntityType}", EntityType
                    );

                    return UnprocessableEntity(ErrorMessages.NoFirstXpLevel);
                }

                player.Id = Guid.NewGuid();
                player.Password = _passwordHasher.HashPassword(player, player.Password);
                player.XpLevelId = _context.XpLevels.First(xpLevel => xpLevel.Number == 1).Id;
                player.EmailVerificationToken = _tokenService.CreateEmailVerificationToken();
                player.EmailVerificationTokenId = Guid.Empty;

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
                        message: "Database conflict occured while registering {EntityType} with id = {id}.", EntityType, player.Id
                    );

                    return Conflict(ErrorMessages.PlayerRegisterConflict);
                }

                PublishVerificationEmailRequest(player.Email, player.EmailVerificationToken.Token);

                return Ok(player.RefreshToken);
            });

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public async Task<IActionResult> SendVerificationEmail(string login)
        => await AllowRequestSenders(RequestSenders.GameClient, RequestSenders.PublicWebsite)
            .ExecuteAsync(login, async login =>
            {
                _logger.LogInformation("SendVerificationEmail was called for {EntityType}.", EntityType);
                PublishStatistics("SendVerificationEmail");

                if (login.IsEmpty())
                    return NotFound();

                var player = await _getByLogin(_context, login);

                if (player is null)
                    return NotFound();

                if (player.EmailVerified)
                    return Unauthorized();

                if (player.EmailVerificationTokenId is null)
                {
                    var newVerificationToken = _tokenService.CreateEmailVerificationToken();

                    if (await SetEmailVerificationTokenAsync(player, newVerificationToken) is false)
                        return NotFound();

                    PublishVerificationEmailRequest(player.Email, newVerificationToken.Token);
                    return Ok();
                }

                var verificationToken = _context.EmailVerificationTokens
                    .FirstOrDefault(emailVerificationToken => emailVerificationToken.Id == player.EmailVerificationTokenId);

                if (verificationToken is null)
                {
                    var newVerificationToken = _tokenService.CreateEmailVerificationToken();

                    if (await SetEmailVerificationTokenAsync(player, newVerificationToken) is false)
                        return NotFound();

                    PublishVerificationEmailRequest(player.Email, newVerificationToken.Token);
                    return Ok();
                }

                if (verificationToken.ExpirationTimestamp <= DateTime.Now)
                {
                    if (await DeleteEmailVerificationTokenAsync(player, verificationToken) is false)
                        return NotFound();

                    var newVerificationToken = _tokenService.CreateEmailVerificationToken();

                    if (await SetEmailVerificationTokenAsync(player, newVerificationToken) is false)
                        return NotFound();

                    PublishVerificationEmailRequest(player.Email, newVerificationToken.Token);
                    return Ok();
                }

                PublishVerificationEmailRequest(player.Email, verificationToken.Token);
                return Ok();
            });

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public async Task<IActionResult> VerifyEmail(string login, string emailVerificationToken)
        => await AllowRequestSenders(RequestSenders.PublicWebsite)
            .ExecuteAsync(login, emailVerificationToken, async (login, emailVerificationToken) =>
            {
                _logger.LogInformation("VerifyEmail was called for {EntityType}.", EntityType);
                PublishStatistics("VerifyEmail");

                if (login.IsEmpty())
                    return NotFound();

                var player = await _getByLogin(_context, login);

                if (player is null)
                    return NotFound();

                if (VerifyEmailVerificationToken(player, emailVerificationToken) is false)
                    return Unauthorized();

                var verificationToken = _context.EmailVerificationTokens
                    .FirstOrDefault(emailVerificationToken => emailVerificationToken.Id == player.EmailVerificationTokenId);

                if (verificationToken is null)
                    return Unauthorized();

                if (await DeleteEmailVerificationTokenAsync(player, verificationToken) is false)
                    return NotFound();

                if (await SetEmailVerifiedAsync(player) is false)
                    return NotFound();

                return Ok();
            });

    [HttpPost]
    [AllowAnonymous]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await AllowRequestSenders(RequestSenders.GameClient)
            .LoginAsUserAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> Logout(string email)
        => await AllowRequestSenders(RequestSenders.GameClient)
            .LogoutAsUserAsync(email);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await AllowRequestSenders(RequestSenders.GameClient)
            .ChangeUserPasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> GetAccessToken(string email)
        => await AllowRequestSenders(RequestSenders.GameClient)
            .GetAccessTokenForUserAsync(email);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> VerifyAccessToken()
        => await AllowRequestSenders(RequestSenders.GameClient)
            .VerifyAccessTokenAsync();

    private async Task<bool> SetEmailVerificationTokenAsync(Player player, EmailVerificationToken token)
    {
        player.EmailVerificationToken = token;
        player.EmailVerificationTokenId = Guid.Empty;

        _context.Update(player);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await EntityExistsAsync(player.Id) is false)
                return false;
            else
                throw;
        }

        return true;
    }

    private async Task<bool> DeleteEmailVerificationTokenAsync(Player player, EmailVerificationToken token)
    {
        player.EmailVerificationToken = null;
        player.EmailVerificationTokenId = null;

        _context.Update(player);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await EntityExistsAsync(player.Id) is false)
                return false;
            else
                throw;
        }

        _context.Remove(token);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _context.EmailVerificationTokens.AnyAsync(emailVerificationToken => emailVerificationToken.Id == token.Id) is false)
                return true;
            else
                throw;
        }

        return true;
    }

    private bool VerifyEmailVerificationToken(Player player, string? emailVerificationToken)
    {
        if (player.EmailVerificationTokenId is null)
            return false;

        var verificationToken = _context.EmailVerificationTokens
            .FirstOrDefault(emailVerificationToken => emailVerificationToken.Id == player.EmailVerificationTokenId);

        return verificationToken is not null && verificationToken.Token.Equals(emailVerificationToken);
    }

    private async Task<bool> SetEmailVerifiedAsync(Player player)
    {
        player.EmailVerified = true;

        _context.Update(player);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await EntityExistsAsync(player.Id) is false)
                return false;
            else
                throw;
        }

        return true;
    }

    private void PublishVerificationEmailRequest(string email, string verificationToken)
        => Task.Run(async () => await PublishVerificationEmailRequestAsync(email, verificationToken));

    private async Task PublishVerificationEmailRequestAsync(string email, string verificationToken)
    {
        VerificationEmailDTO verificationEmailDTO = new()
        {
            To = email,
            VerificationToken = verificationToken
        };

        string json = JsonSerializer.Serialize(verificationEmailDTO, _jsonSerializerOptionsProvider.Default);
        byte[] data = Encoding.UTF8.GetBytes(json);

        bool success = await _rabbitMqProducer.PublishMessageAsync(exchange: RabbitMqExchangeNames.VerificationEmails, message: data);

        _logger.LogInformation("Completed publishing verification email.{newline}Email intended for {email}.{newline}Status: {status}.",
            Environment.NewLine, verificationEmailDTO.To,
            Environment.NewLine, success ? "success" : "fail");
    }
}
