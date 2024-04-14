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
public class DevelopersController : DatabaseController
{
    private readonly IPasswordHasher<Developer> _passwordHasher;

    public DevelopersController(DreamcoreHorrorGameContext context, IPasswordHasher<Developer> passwordHasher)
        : base(context)
        => _passwordHasher = passwordHasher;

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.Developers.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var developer = await _context.Developers.FindAsync(id);

        return developer is not null
            ? Ok(developer)
            : NotFound();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByLogin(string login)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (login.IsEmpty())
            return NotFound();

        var developer = await _context.Developers.FirstOrDefaultAsync(developer => developer.Login.Equals(login));

        return developer is not null
            ? Ok(developer)
            : NotFound();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(Developer.Id),
        nameof(Developer.Login),
        nameof(Developer.Password),
        nameof(Developer.RefreshToken),
        nameof(Developer.DeveloperAccessLevelId)
    )] Developer developer)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        bool developerExists = await _context.Developers.AnyAsync(d => d.Login.Equals(developer.Login));

        if (developerExists)
            return UnprocessableEntity(ErrorMessages.DeveloperAlreadyExists);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        developer.Id = Guid.NewGuid();
        developer.Password = _passwordHasher.HashPassword(developer, developer.Password);

        _context.Add(developer);
        await _context.SaveChangesAsync();
        return Ok(developer);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Developer.Id),
        nameof(Developer.Login),
        nameof(Developer.Password),
        nameof(Developer.RefreshToken),
        nameof(Developer.DeveloperAccessLevelId)
    )] Developer developer)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != developer.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(developer);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DeveloperExists(developer.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(developer);
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

        var developer = await _context.Developers.FindAsync(id);

        if (developer is null)
            return NotFound();

        _context.Remove(developer);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginData loginData)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (loginData.IsEmptyLogin || loginData.IsEmptyPassword)
            return Unauthorized();

        var developer = GetByLoginIncludeDeveloperAccessLevel(loginData.Login);

        if (developer is null)
            return Unauthorized();

        if (VerifyPassword(developer, loginData.Password))
        {
            string token = TokenService.CreateRefreshToken(developer.Login, developer.DeveloperAccessLevel.Name);
            SetRefreshToken(developer, token);
            return Ok(token);
        }

        return Unauthorized();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(LoginData loginData, string newPassword)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (loginData.IsEmptyLogin || loginData.IsEmptyPassword || newPassword.IsEmpty())
            return Unauthorized();

        var developer = GetByLoginIncludeDeveloperAccessLevel(loginData.Login);

        if (developer is null)
            return Unauthorized();

        if (VerifyPassword(developer, loginData.Password))
        {
            string token = TokenService.CreateRefreshToken(developer.Login, developer.DeveloperAccessLevel.Name);
            SetPasswordAndRefreshToken(developer, newPassword, token);
            return Ok(token);
        }

        return Unauthorized();
    }


    // TODO: password restore

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Developer)]
    public IActionResult GetAccessToken(string login)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (login.IsEmpty())
            return Unauthorized();

        var developer = GetByLoginIncludeDeveloperAccessLevel(login);

        if (developer is null)
            return Unauthorized();

        if (VerifyRefreshToken(developer, AuthorizationToken))
            return Ok(TokenService.CreateAccessToken(login, developer.DeveloperAccessLevel.Name));

        return Unauthorized();
    }

    private bool DeveloperExists(Guid id)
        => _context.Developers.Any(developer => developer.Id == id);

    private Developer? GetByLoginIncludeDeveloperAccessLevel(string? login)
        => _context.Developers.Include(developer => developer.DeveloperAccessLevel)
            .FirstOrDefault(developer => developer.Login.Equals(login));

    private bool VerifyPassword(Developer developer, string? password)
        => _passwordHasher.VerifyHashedPassword(developer, developer.Password, password ?? string.Empty)
            is not PasswordVerificationResult.Failed;

    private bool SetPasswordAndRefreshToken(Developer developer, string password, string? token)
    {
        developer.Password = _passwordHasher.HashPassword(developer, password);
        developer.RefreshToken = token;

        try
        {
            _context.Update(developer);
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DeveloperExists(developer.Id))
                return false;
            else
                throw;
        }
        return true;
    }

    private bool VerifyRefreshToken(Developer developer, string? refreshToken)
        => developer.RefreshToken is not null && developer.RefreshToken.Equals(refreshToken);

    private bool SetRefreshToken(Developer developer, string? token)
    {
        developer.RefreshToken = token;

        try
        {
            _context.Update(developer);
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DeveloperExists(developer.Id))
                return false;
            else
                throw;
        }
        return true;
    }
}
