using DreamcoreHorrorGameApiServer.Comparers;
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
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;

using AuthenticationSchemes = DreamcoreHorrorGameApiServer.ConstantValues.AuthenticationSchemes;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class ServersController
(
    DreamcoreHorrorGameContext context,
    IPropertyPredicateValidator propertyPredicateValidator,
    ILogger<ServersController> logger,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider,
    IRabbitMqProducer rabbitMqProducer,
    ITokenService tokenService,
    IPasswordHasher<Server> passwordHasher,
    IHttpFetcher httpFetcher
)
: UserController<Server>
(
    context: context,
    propertyPredicateValidator: propertyPredicateValidator,
    logger: logger,
    jsonSerializerOptionsProvider: jsonSerializerOptionsProvider,
    rabbitMqProducer: rabbitMqProducer,
    tokenService: tokenService,
    passwordHasher: passwordHasher,
    alreadyExistsErrorMessage: ErrorMessages.ServerAlreadyExists,
    orderBySelectorExpression: server => server.IpAddress,
    orderByComparer: new IPAddressComparer(),
    getAllWithFirstLevelRelationsFunction: async (context) =>
    {
        var gameSessions = await context.GameSessions.ToListAsync();

        return context.Servers.AsQueryable();
    },
    setRelationsFromForeignKeysFunction: null,
    getByLoginFunction: async (context, login) =>
    {
        return IPAddress.TryParse(login, out var ipAddress)
            ? await context.Servers.FirstOrDefaultAsync(server => server.IpAddress.Equals(ipAddress))
            : null;
    }
)
{
    private readonly IHttpFetcher _httpFetcher = httpFetcher;

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
        nameof(Server.Id),
        nameof(Server.IpAddress),
        nameof(Server.Password),
        nameof(Server.RefreshToken),
        nameof(Server.PlayerCapacity),
        nameof(Server.IsOnline)
    )] Server server)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .CreateEntityAsync(server);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Server.Id),
        nameof(Server.IpAddress),
        nameof(Server.Password),
        nameof(Server.RefreshToken),
        nameof(Server.PlayerCapacity),
        nameof(Server.IsOnline)
    )] Server server)
        => await AllowRequestSenders(RequestSenders.GameServer, RequestSenders.ApplicationForDevelopers)
            .EditEntityAsync(id, server);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await AllowRequestSenders(RequestSenders.ApplicationForDevelopers)
            .DeleteEntityAsync(id);

    [HttpPost]
    [AllowAnonymous]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await AllowRequestSenders(RequestSenders.GameServer)
            .LoginAsUserAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> Logout(string ipAddress)
        => await AllowRequestSenders(RequestSenders.GameServer)
            .LogoutAsUserAsync(ipAddress);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await AllowRequestSenders(RequestSenders.GameServer)
            .ChangeUserPasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> GetAccessToken(string ipAddress)
        => await AllowRequestSenders(RequestSenders.GameServer)
            .GetAccessTokenForUserAsync(ipAddress);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> VerifyAccessToken()
        => await AllowRequestSenders(RequestSenders.GameServer)
            .VerifyAccessTokenAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public async Task<IActionResult> GetServerWithFreeSlots(int slots)
        => await AllowRequestSenders(RequestSenders.GameClient)
            .ExecuteAsync(slots, async slots =>
            {
                _logger.LogInformation("GetServerWithFreeSlots was called for {EntityType}.", EntityType);
                PublishStatistics("GetServerWithFreeSlots");

                if (slots is < 1)
                    return UnprocessableEntity(ErrorMessages.UnacceptableParameterValue);

                var server = await GetServersWithEnoughFreeSlots(slots)
                    .FirstOrDefaultWithAsyncPredicate(async server => await HasWaitingSession(server, slots));

                return Ok(server?.IpAddress);
            });

    private ParallelQuery<Server> GetServersWithEnoughFreeSlots(int requestedSlots)
        => _context.Servers
            .Where(server => server.IsOnline)
            .AsForceParallel()
            .Where(server => GetCurrentPlayerCount(server) + requestedSlots <= server.PlayerCapacity);

    private int GetCurrentPlayerCount(Server server)
        => _context.PlayerSessions
            .Where(playerSession => playerSession.EndTimestamp == null
                && playerSession.GameSession != null && playerSession.GameSession.ServerId == server.Id)
            .Count();

    private async Task<bool> HasWaitingSession(Server server, int slots)
    {
        HttpResponseMessage? anyWaitingSessionsResponse = await _httpFetcher.GetAsync
        (
            host: server.IpAddress.ToString(),
            port: 8024,
            path: $"/api/WaitingSessions/Any?playerCount={slots}"
        );

        if (anyWaitingSessionsResponse is null || anyWaitingSessionsResponse.IsNotSuccessStatusCode())
            return false;

        string responseText = await anyWaitingSessionsResponse.Content.ReadAsStringAsync();

        try
        {
            bool hasWaitingSessions = JsonSerializer
                .Deserialize<bool>(responseText, _jsonSerializerOptionsProvider.Default);

            if (hasWaitingSessions)
                return true;
        }
        catch (JsonException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("HasWaitingSession".GetHashCode() + ex.GetType().GetHashCode()),
                message: "An error occured while deserializing HTTP response on having any waiting sessions from server {IpAddress}.", server.IpAddress.ToString()
            );

            return false;
        }

        return await CreateWaitingSession(server, slots);
    }

    private async Task<bool> CreateWaitingSession(Server server, int slots)
    {
        MediaTypeHeaderValue requestContentType = new(MediaTypeNames.Application.Json);

        JsonContent jsonContent = JsonContent.Create
        (
            inputValue: slots,
            inputType: typeof(int),
            mediaType: requestContentType,
            options: _jsonSerializerOptionsProvider.Default
        );

        HttpResponseMessage? createWaitingSessionResponse = await _httpFetcher.PostAsync
        (
            host: server.IpAddress.ToString(),
            port: 8024,
            path: $"/api/WaitingSessions/Create",
            content: jsonContent
        );

        if (createWaitingSessionResponse is null)
            return false;

        string responseText = await createWaitingSessionResponse.Content.ReadAsStringAsync();

        return createWaitingSessionResponse.IsSuccessStatusCode && responseText.IsEmpty();
    }
}
