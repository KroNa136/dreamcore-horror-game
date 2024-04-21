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
public class ServersController : UserController<Server>
{
    private readonly IHttpFetcher _httpFetcher;
    private readonly IJsonSerializerOptionsProvider _jsonSerializerOptionsProvider;

    public ServersController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator,
        ITokenService tokenService,
        IPasswordHasher<Server> passwordHasher,
        IHttpFetcher httpFetcher,
        IJsonSerializerOptionsProvider jsonSerializerOptionsProvider
    )
    : base
    (
        context: context,
        propertyPredicateValidator: propertyPredicateValidator,
        tokenService: tokenService,
        passwordHasher: passwordHasher,
        alreadyExistsErrorMessage: ErrorMessages.ServerAlreadyExists,
        orderBySelector: server => server.IpAddress,
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
        _httpFetcher = httpFetcher;
        _jsonSerializerOptionsProvider = jsonSerializerOptionsProvider;
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAllWithRelations()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesWithRelationsAsync();

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
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntitiesWhereAsync(predicateCollection);

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
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
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
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .EditEntityAsync(id, server);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteEntityAsync(id);

    [HttpPost]
    [AllowAnonymous]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await RequireHeaders(CorsHeaders.GameServer)
            .LoginAsUserAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> Logout(Guid? id)
        => await RequireHeaders(CorsHeaders.GameServer)
            .LogoutAsUserAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await RequireHeaders(CorsHeaders.GameServer)
            .ChangeUserPasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> GetAccessToken(string ipAddress)
        => await RequireHeaders(CorsHeaders.GameServer)
            .GetAccessTokenForUserAsync(ipAddress);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public async Task<IActionResult> GetServerWithFreeSlots(int slots)
        => await RequireHeaders(CorsHeaders.GameClient)
            .ExecuteAsync(slots, async slots =>
            {
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
            .Where(playerSession => playerSession.EndTimestamp == null && playerSession.GameSession.ServerId == server.Id)
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
        catch (JsonException)
        {
            // TODO: log error
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
