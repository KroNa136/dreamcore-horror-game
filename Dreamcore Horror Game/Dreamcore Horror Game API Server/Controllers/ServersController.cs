using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class ServersController : UserController<Server>
{
    public ServersController(DreamcoreHorrorGameContext context, IPasswordHasher<Server> passwordHasher)
        : base(
            context: context,
            passwordHasher: passwordHasher,
            getByLoginFunction: (context, login) => context.Servers
                .FirstOrDefaultAsync(server => server.IpAddress.ToString().Equals(login)),
            alreadyExistsErrorMessage: ErrorMessages.ServerAlreadyExists
        )
    { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetAsync(server => server.Id == id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([Bind(
        nameof(Server.Id),
        nameof(Server.IpAddress),
        nameof(Server.Password),
        nameof(Server.RefreshToken),
        nameof(Server.PlayerCapacity),
        nameof(Server.IsOnline)
    )] Server server)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateAsync(server);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Server.Id),
        nameof(Server.IpAddress),
        nameof(Server.Password),
        nameof(Server.RefreshToken),
        nameof(Server.PlayerCapacity),
        nameof(Server.IsOnline)
    )] Server server)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .EditAsync(id, server);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteAsync(id);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await RequireHeaders(CorsHeaders.GameServer)
            .LoginAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await RequireHeaders(CorsHeaders.GameServer)
            .ChangePasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Server)]
    public override async Task<IActionResult> GetAccessToken(string ipAddress)
        => await RequireHeaders(CorsHeaders.GameServer)
            .GetAccessTokenAsync(ipAddress);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public async Task<IActionResult> GetServerWithFreeSlots(int slots)
        => await RequireHeaders(CorsHeaders.GameClient)
            .DoAsync(slots, async slots =>
            {
                if (slots < 1)
                    return UnprocessableEntity(ErrorMessages.UnacceptableParameterValue);

                var suitableServers = GetServersWithEnoughFreeSlots(slots);

                var server = await suitableServers.FirstOrDefaultAsync(async server => await HasWaitingSession(server, slots));

                return Ok(server?.IpAddress);
            });

    private IEnumerable<Server> GetServersWithEnoughFreeSlots(int requestedSlots)
        => _context.Servers.Where(server => server.IsOnline).ToList()
            .Where(server => GetCurrentPlayerCount(server) + requestedSlots <= server.PlayerCapacity);

    private int GetCurrentPlayerCount(Server server)
        => _context.PlayerSessions.Where(playerSession => playerSession.EndTimestamp == null
            && playerSession.GameSession.ServerId == server.Id).Count();

    private static async Task<bool> HasWaitingSession(Server server, int slots)
    {
        HttpResponseMessage anyWaitingSessionsResponse = await HttpFetcher.GetAsync(
            host: server.IpAddress.ToString(),
            port: 80,
            path: $"/api/WaitingSessions/Any?playerCount={slots}"
        );

        if (!anyWaitingSessionsResponse.IsSuccessStatusCode)
            return false;

        string responseText = await anyWaitingSessionsResponse.Content.ReadAsStringAsync();

        try
        {
            bool hasWaitingSessions = JsonSerializer.Deserialize<bool>(responseText, JsonSerializerOptionsProvider.Shared);

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

    private static async Task<bool> CreateWaitingSession(Server server, int slots)
    {
        JsonContent jsonContent = JsonContent.Create(slots, typeof(int));

        HttpResponseMessage createWaitingSessionResponse = await HttpFetcher.PostAsync(
            host: server.IpAddress.ToString(),
            port: 80,
            path: $"/api/WaitingSessions/Create?playerCount={slots}",
            content: jsonContent
        );

        string responseText = await createWaitingSessionResponse.Content.ReadAsStringAsync();

        return createWaitingSessionResponse.IsSuccessStatusCode && responseText.IsEmpty();
    }
}
