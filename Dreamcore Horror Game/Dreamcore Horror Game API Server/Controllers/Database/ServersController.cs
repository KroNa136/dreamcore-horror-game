using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class ServersController : DatabaseController
{
    private readonly IPasswordHasher<Server> _passwordHasher;

    public ServersController(DreamcoreHorrorGameContext context, IPasswordHasher<Server> passwordHasher)
        : base(context)
        => _passwordHasher = passwordHasher;

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.Servers.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var server = await _context.Servers.FindAsync(id);

        return server is not null
            ? Ok(server)
            : NotFound();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(Server.Id),
        nameof(Server.IpAddress),
        nameof(Server.Password),
        nameof(Server.RefreshToken),
        nameof(Server.PlayerCapacity),
        nameof(Server.IsOnline)
    )] Server server)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        bool serverExists = await _context.Servers.AnyAsync(s =>
            s.IpAddress.ToString().Equals(server.IpAddress.ToString()));

        if (serverExists)
            return UnprocessableEntity(ErrorMessages.ServerAlreadyExists);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        server.Id = Guid.NewGuid();
        server.Password = _passwordHasher.HashPassword(server, server.Password);

        _context.Add(server);
        await _context.SaveChangesAsync();
        return Ok(server);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Server.Id),
        nameof(Server.IpAddress),
        nameof(Server.Password),
        nameof(Server.RefreshToken),
        nameof(Server.PlayerCapacity),
        nameof(Server.IsOnline)
    )] Server server)
    {
        if (NoHeader(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != server.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(server);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServerExists(server.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(server);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var server = await _context.Servers.FindAsync(id);

        if (server is null)
            return NotFound();

        _context.Remove(server);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginData loginData)
    {
        if (NoHeader(CorsHeaders.GameServer))
            return Forbid(ErrorMessages.HeaderMissing);

        if (loginData.IsEmptyLogin || loginData.IsEmptyPassword)
            return Unauthorized();

        var server = GetByIpAddress(loginData.Login);

        if (server is null)
            return Unauthorized();

        if (VerifyPassword(server, loginData.Password))
        {
            string token = TokenService.CreateRefreshToken(server.IpAddress.ToString(), AuthenticationRoles.Server);
            SetRefreshToken(server, token);
            return Ok(token);
        }

        return Unauthorized();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(LoginData loginData, string newPassword)
    {
        if (NoHeader(CorsHeaders.GameServer))
            return Forbid(ErrorMessages.HeaderMissing);

        if (loginData.IsEmptyLogin || loginData.IsEmptyPassword || newPassword.IsEmpty())
            return Unauthorized();

        var server = GetByIpAddress(loginData.Login);

        if (server is null)
            return Unauthorized();

        if (VerifyPassword(server, loginData.Password))
        {
            string token = TokenService.CreateRefreshToken(server.IpAddress.ToString(), AuthenticationRoles.Server);
            SetPasswordAndRefreshToken(server, newPassword, token);
            return Ok(token);
        }

        return Unauthorized();
    }

    // TODO: password restore

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Server)]
    public IActionResult GetAccessToken(string ipAddress)
    {
        if (NoHeader(CorsHeaders.GameServer))
            return Forbid(ErrorMessages.HeaderMissing);

        if (ipAddress.IsEmpty())
            return Unauthorized();

        var server = GetByIpAddress(ipAddress);

        if (server is null)
            return Unauthorized();

        if (VerifyRefreshToken(server, AuthorizationToken))
            return Ok(TokenService.CreateAccessToken(ipAddress, AuthenticationRoles.Server));

        return Unauthorized();
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public async Task<IActionResult> GetServerWithFreeSlots(int slots)
    {
        if (NoHeader(CorsHeaders.GameClient))
            return Forbid(ErrorMessages.HeaderMissing);

        if (slots < 1)
            return UnprocessableEntity(ErrorMessages.UnacceptableParameterValue);

        var suitableServers = GetServersWithEnoughFreeSlots(slots);

        var server = await suitableServers.FirstOrDefaultAsync(async server => await HasWaitingSession(server, slots));

        return Ok(server?.IpAddress);
    }

    private bool ServerExists(Guid id)
        => _context.Servers.Any(server => server.Id == id);

    private Server? GetByIpAddress(string? ipAddress)
        => _context.Servers.FirstOrDefault(server => server.IpAddress.ToString().Equals(ipAddress));

    private bool VerifyPassword(Server server, string? password)
        => _passwordHasher.VerifyHashedPassword(server, server.Password, password ?? string.Empty)
            is not PasswordVerificationResult.Failed;

    private bool SetPasswordAndRefreshToken(Server server, string password, string? token)
    {
        server.Password = _passwordHasher.HashPassword(server, password);
        server.RefreshToken = token;

        try
        {
            _context.Update(server);
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServerExists(server.Id))
                return false;
            else
                throw;
        }
        return true;
    }

    private bool VerifyRefreshToken(Server server, string? refreshToken)
        => server.RefreshToken is not null && server.RefreshToken.Equals(refreshToken);

    private bool SetRefreshToken(Server server, string? token)
    {
        server.RefreshToken = token;

        try
        {
            _context.Update(server);
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServerExists(server.Id))
                return false;
            else
                throw;
        }
        return true;
    }

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
