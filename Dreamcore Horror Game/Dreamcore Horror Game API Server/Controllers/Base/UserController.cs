using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DreamcoreHorrorGameApiServer.Controllers.Base;

public abstract class UserController<TUser> : DatabaseEntityController<TUser>
    where TUser : class, IDatabaseEntity, IUser
{
    protected readonly ITokenService _tokenService;
    protected readonly IPasswordHasher<TUser> _passwordHasher;

    protected readonly string _alreadyExistsErrorMessage;

    protected readonly Func<DreamcoreHorrorGameContext, string?, Task<TUser?>> _getByLogin;

    public UserController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator,
        ITokenService tokenService,
        IPasswordHasher<TUser> passwordHasher,
        string alreadyExistsErrorMessage,
        Expression<Func<TUser, object?>> orderBySelectorExpression,
        IComparer<object?>? orderByComparer,
        Func<DreamcoreHorrorGameContext, Task<IQueryable<TUser>>> getAllWithFirstLevelRelationsFunction,
        Func<DreamcoreHorrorGameContext, TUser, Task>? setRelationsFromForeignKeysFunction,
        Func<DreamcoreHorrorGameContext, string?, Task<TUser?>> getByLoginFunction
    )
    : base
    (
        context,
        propertyPredicateValidator,
        orderBySelectorExpression,
        orderByComparer,
        getAllWithFirstLevelRelationsFunction,
        setRelationsFromForeignKeysFunction
    )
    {
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;

        _alreadyExistsErrorMessage = alreadyExistsErrorMessage;

        _getByLogin = getByLoginFunction;
    }

    public override abstract Task<IActionResult> Create(TUser entity);
    public abstract Task<IActionResult> Login(LoginData loginData);
    public abstract Task<IActionResult> Logout(string login);
    public abstract Task<IActionResult> ChangePassword(LoginData loginData, string newPassword);
    public abstract Task<IActionResult> GetAccessToken(string login);
    public abstract Task<IActionResult> VerifyAccessToken();

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public override UserController<TUser> RequireHeaders(params string[] headers)
    {
        SetRequiredHeaders(headers);
        return this;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public override async Task<IActionResult> CreateEntityAsync(TUser user)
        => await ValidateHeadersAndHandleErrorsAsync(user, async user =>
        {
            bool userExists = await _getByLogin(_context, user.Login) is not null;

            if (userExists)
                return UnprocessableEntity(_alreadyExistsErrorMessage);

            if (InvalidModelState)
                return ValidationProblem();

            user.Id = Guid.NewGuid();
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            if (_setRelationsFromForeignKeys is not null)
                await _setRelationsFromForeignKeys(_context, user);

            _context.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO: log error
                return Conflict(ErrorMessages.CreateConflict);
            }

            return Ok(user);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> LoginAsUserAsync(LoginData loginData)
        => await ValidateHeadersAndHandleErrorsAsync(loginData, async loginData =>
        {
            if (loginData.IsEmptyLogin() || loginData.IsEmptyPassword())
                return BadRequest(ErrorMessages.EmptyLoginOrPassword);

            var user = await _getByLogin(_context, loginData.Login);

            if (user is null)
                return NotFound();

            /*
            if (user.IsOnline)
                return UnprocessableEntity(ErrorMessages.UserIsAlreadyLoggedIn);

            This may cause problems if, for example, some player experiences a game crash and tries to log in again.
            The system would consider them being still online, and wouldn't let them sign in.
            TODO: consider doing something to make sure it's still the same person logging in (maybe an IP check or smth)
            */

            if (VerifyPassword(user, loginData.Password) is false)
                return Unauthorized();

            string token = _tokenService.CreateRefreshToken(user.Login, user.Role);

            try
            {
                if (await SetRefreshTokenAsync(user, token) is false
                    || await SetIsOnlineAsync(user, true) is false)
                    return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO: log error
                return Conflict(ErrorMessages.UserLoginConflict);
            }

            return Ok(token);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> LogoutAsUserAsync(string login)
        => await ValidateHeadersAndHandleErrorsAsync(login, async login =>
        {
            if (login.IsEmpty())
                return NotFound();

            var user = await _getByLogin(_context, login);

            if (user is null)
                return NotFound();

            if (user.IsOnline is false)
                return UnprocessableEntity(ErrorMessages.UserIsAlreadyLoggedOut);

            try
            {
                if (await SetIsOnlineAsync(user, false) is false
                    || await SetRefreshTokenAsync(user, null) is false)
                    return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO: log error
                return Conflict(ErrorMessages.UserLogoutConflict);
            }

            return Ok();
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> ChangeUserPasswordAsync(LoginData loginData, string newPassword)
        => await ValidateHeadersAndHandleErrorsAsync(loginData, newPassword, async (loginData, newPassword) =>
        {
            if (loginData.IsEmptyLogin() || loginData.IsEmptyPassword() || newPassword.IsEmpty())
                return BadRequest(ErrorMessages.EmptyLoginOrPassword);

            var user = await _getByLogin(_context, loginData.Login);

            if (user is null)
                return NotFound();

            if (VerifyPassword(user, loginData.Password) is false)
                return Unauthorized();

            string token = _tokenService.CreateRefreshToken(user.Login, user.Role);

            try
            {
                if (await SetPasswordAndRefreshTokenAsync(user, newPassword, token) is false)
                    return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO: log error
                return Conflict(ErrorMessages.ChangeUserPasswordConflict);
            }

            return Ok(token);
        });

    // TODO: password restore

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAccessTokenForUserAsync(string login)
        => await ValidateHeadersAndHandleErrorsAsync(login, async login =>
        {
            if (login.IsEmpty())
                return NotFound();

            var user = await _getByLogin(_context, login);

            if (user is null)
                return NotFound();

            if (VerifyRefreshToken(user, AuthorizationToken) is false)
                return Unauthorized();

            return Ok(_tokenService.CreateAccessToken(user.Login, user.Role));
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> VerifyAccessTokenAsync()
        => await ValidateHeadersAndHandleErrorsAsync(async () => Ok());

    protected bool VerifyPassword(TUser user, string? password)
        => _passwordHasher.VerifyHashedPassword(user, user.Password, password ?? string.Empty)
            is not PasswordVerificationResult.Failed;

    protected async Task<bool> SetPasswordAndRefreshTokenAsync(TUser user, string password, string? token)
    {
        user.Password = _passwordHasher.HashPassword(user, password);
        user.RefreshToken = token;

        _context.Update(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await EntityExistsAsync(user.Id) is false)
                return false;
            else
                throw;
        }

        return true;
    }

    protected bool VerifyRefreshToken(TUser user, string? refreshToken)
        => user.RefreshToken is not null && user.RefreshToken.Equals(refreshToken);

    protected async Task<bool> SetRefreshTokenAsync(TUser user, string? token)
    {
        user.RefreshToken = token;

        _context.Update(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await EntityExistsAsync(user.Id) is false)
                return false;
            else
                throw;
        }

        return true;
    }

    protected async Task<bool> SetIsOnlineAsync(TUser user, bool isOnline)
    {
        user.IsOnline = isOnline;

        _context.Update(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await EntityExistsAsync(user.Id) is false)
                return false;
            else
                throw;
        }

        return true;
    }
}
