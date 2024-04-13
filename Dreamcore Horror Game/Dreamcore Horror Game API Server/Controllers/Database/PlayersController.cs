using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class PlayersController : DatabaseController
{
    private readonly IPasswordHasher<Player> _passwordHasher;

    public PlayersController(DreamcoreHorrorGameContext context, IPasswordHasher<Player> passwordHasher)
        : base(context)
        => _passwordHasher = passwordHasher;

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : Ok(await _context.Players.ToListAsync());

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public async Task<IActionResult> Get(Guid? id)
        => NoHeader(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : id is not null
                && await _context.Players.FindAsync(id) is Player player
            ? Ok(player)
            : NotFound();

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
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
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        bool playerExists = await _context.Players.AnyAsync(p => p.Email.Equals(player.Email));

        if (playerExists)
            return UnprocessableEntity(ErrorMessages.PlayerAlreadyExists);

        if (ModelState.IsValid)
        {
            player.Id = Guid.NewGuid();
            player.Password = _passwordHasher.HashPassword(player, player.Password);

            _context.Add(player);
            await _context.SaveChangesAsync();
            return Ok(player);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
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
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != player.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(player);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(player.Id))
                    return NotFound();
                else
                    throw;
            }
            return Ok(player);
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

        if (id is not null && await _context.Players.FindAsync(id) is Player player)
        {
            _context.Remove(player);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
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
    {
        if (NoHeader(CorsHeaders.GameClient))
            return Forbid(ErrorMessages.HeaderMissing);

        bool playerExists = await _context.Players.AnyAsync(p => p.Email.Equals(player.Email));

        if (playerExists)
            return UnprocessableEntity(ErrorMessages.PlayerAlreadyExists);

        if (ModelState.IsValid)
        {
            player.Id = Guid.NewGuid();
            player.Password = _passwordHasher.HashPassword(player, player.Password);
            player.RefreshToken = TokenService.CreateRefreshToken(player.Email, AuthenticationRoles.Player);

            _context.Add(player);
            await _context.SaveChangesAsync();
            return Ok(player.RefreshToken);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginData loginData)
        => NoHeader(CorsHeaders.GameClient)
            ? Forbid(ErrorMessages.HeaderMissing)
            : loginData.IsNotEmptyLogin && loginData.IsNotEmptyPassword
                && GetByEmail(loginData.Login) is Player player
                && VerifyPassword(player, loginData.Password)
                && TokenService.CreateRefreshToken(player.Email, AuthenticationRoles.Player) is string token
                && SetRefreshToken(player, token)
            ? Ok(token)
            : Unauthorized();

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(LoginData loginData, string newPassword)
        => NoHeader(CorsHeaders.GameClient)
            ? Forbid(ErrorMessages.HeaderMissing)
            : loginData.IsNotEmptyLogin && loginData.IsNotEmptyPassword && newPassword.IsNotEmpty()
                && GetByEmail(loginData.Login) is Player player
                && VerifyPassword(player, loginData.Password)
                && TokenService.CreateRefreshToken(player.Email, AuthenticationRoles.Player) is string token
                && SetPasswordAndRefreshToken(player, newPassword, token)
            ? Ok(token)
            : Unauthorized();

    // TODO: password restore

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Player)]
    public IActionResult GetAccessToken(string email)
        => NoHeader(CorsHeaders.GameClient)
            ? Forbid(ErrorMessages.HeaderMissing)
            : email.IsNotEmpty()
                && GetByEmail(email) is Player player
                && VerifyRefreshToken(player, GetTokenFromHeaders())
            ? Ok(TokenService.CreateAccessToken(email, AuthenticationRoles.Player))
            : Unauthorized();

    private bool PlayerExists(Guid id)
        => _context.Players.Any(player => player.Id == id);

    private Player? GetByEmail(string? email)
        => _context.Players.FirstOrDefault(player => player.Email.Equals(email));

    private bool VerifyPassword(Player player, string? password)
        => _passwordHasher.VerifyHashedPassword(player, player.Password, password ?? string.Empty)
            is not PasswordVerificationResult.Failed;

    private bool SetPasswordAndRefreshToken(Player player, string password, string? token)
    {
        player.Password = _passwordHasher.HashPassword(player, password);
        player.RefreshToken = token;

        try
        {
            _context.Update(player);
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlayerExists(player.Id))
                return false;
            else
                throw;
        }
        return true;
    }

    private bool VerifyRefreshToken(Player player, string? refreshToken)
        => player.RefreshToken is not null && player.RefreshToken.Equals(refreshToken);

    private bool SetRefreshToken(Player player, string? token)
    {
        player.RefreshToken = token;

        try
        {
            _context.Update(player);
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlayerExists(player.Id))
                return false;
            else
                throw;
        }
        return true;
    }
}
