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
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : Ok(await _context.Servers.ToListAsync());

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public async Task<IActionResult> Get(Guid? id)
        => NoHeader(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : id is not null
                && await _context.Servers.FindAsync(id) is Server server
            ? Ok(server)
            : NotFound();

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

        if (ModelState.IsValid)
        {
            server.Id = Guid.NewGuid();
            server.Password = _passwordHasher.HashPassword(server, server.Password);

            _context.Add(server);
            await _context.SaveChangesAsync();
            return Ok(server);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
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

        if (ModelState.IsValid)
        {
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

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is not null && await _context.Servers.FindAsync(id) is Server server)
        {
            _context.Remove(server);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginData loginData)
        => NoHeader(CorsHeaders.GameServer)
            ? Forbid(ErrorMessages.HeaderMissing)
            : loginData.IsNotEmptyLogin && loginData.IsNotEmptyPassword
                && GetByIpAddress(loginData.Login!) is Server server
                && VerifyPassword(server, loginData.Password)
                && TokenService.CreateRefreshToken(server.IpAddress.ToString(), AuthenticationRoles.Server) is string token
                && SetRefreshToken(server, token)
            ? Ok(token)
            : Unauthorized();

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Server)]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(LoginData loginData, string newPassword)
        => NoHeader(CorsHeaders.GameServer)
            ? Forbid(ErrorMessages.HeaderMissing)
            : loginData.IsNotEmptyLogin && loginData.IsNotEmptyPassword && newPassword.IsNotEmpty()
                && GetByIpAddress(loginData.Login!) is Server server
                && VerifyPassword(server, loginData.Password)
                && TokenService.CreateRefreshToken(server.IpAddress.ToString(), AuthenticationRoles.Server) is string token
                && SetPasswordAndRefreshToken(server, newPassword, token)
            ? Ok(token)
            : Unauthorized();

    // TODO: password restore

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Server)]
    public IActionResult GetAccessToken(string ipAddress)
        => NoHeader(CorsHeaders.GameServer)
            ? Forbid(ErrorMessages.HeaderMissing)
            : ipAddress.IsNotEmpty()
                && GetByIpAddress(ipAddress) is Server server
                && VerifyRefreshToken(server, GetTokenFromHeaders())
            ? Ok(TokenService.CreateAccessToken(ipAddress, AuthenticationRoles.Server))
            : Unauthorized();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    public async Task<IActionResult> GetServerWithFreeSlots(int slots)
        => NoHeader(CorsHeaders.GameClient)
            ? Forbid(ErrorMessages.HeaderMissing)
            : slots < 1
            ? UnprocessableEntity(ErrorMessages.UnacceptableParameterValue)
            : GetServersWithEnoughFreeSlots(slots) is IEnumerable<Server> suitableServers
                && await suitableServers.FirstOrDefaultAsync(async server => await HasWaitingSession(server, slots))
                    is Server server
            ? Ok(server.IpAddress)
            : Ok(null);

    private bool ServerExists(Guid id)
        => _context.Servers.Any(server => server.Id == id);

    private Server? GetByIpAddress(string ipAddress)
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
        UriBuilder uriBuilder = new()
        {
            Host = server.IpAddress.ToString(),
            Port = 80,
            Path = $"/api/WaitingSessions/Any?playerCount={slots}"
        };

        HttpResponseMessage anyWaitingSessionsResponse = await HttpClientProvider.Shared.GetAsync(uriBuilder.Uri);

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
            return false;
        }

        return await CreateWaitingSession(server, slots);
    }

    private static async Task<bool> CreateWaitingSession(Server server, int slots)
    {
        UriBuilder uriBuilder = new()
        {
            Host = server.IpAddress.ToString(),
            Port = 80,
            Path = $"/api/WaitingSessions/Create?playerCount={slots}"
        };

        JsonContent jsonContent = JsonContent.Create(slots, typeof(int));

        HttpResponseMessage createWaitingSessionResponse = await HttpClientProvider.Shared
            .PostAsync(uriBuilder.Uri, jsonContent);

        string responseText = await createWaitingSessionResponse.Content.ReadAsStringAsync();

        bool success = createWaitingSessionResponse.IsSuccessStatusCode && responseText.IsEmpty();

        return success;
    }
}
