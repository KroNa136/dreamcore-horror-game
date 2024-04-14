using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Base
{
    public abstract class UserController<TUser> : DatabaseEntityController<TUser>
        where TUser : class, IDatabaseEntity, IUser
    {
        private readonly IPasswordHasher<TUser> _passwordHasher;

        private readonly Func<DreamcoreHorrorGameContext, string?, Task<TUser?>> _getByLogin;

        private readonly string _alreadyExistsErrorMessage;

        // private readonly Func<DreamcoreHorrorGameContext, IPasswordHasher<TUser>, UserController<TUser>> _derivedClassConstructor;

        public UserController(DreamcoreHorrorGameContext context, IPasswordHasher<TUser> passwordHasher, Func<DreamcoreHorrorGameContext, string?, Task<TUser?>> getByLoginFunction, string alreadyExistsErrorMessage)
            : base(context)
        {
            _passwordHasher = passwordHasher;
            _getByLogin = getByLoginFunction;
            _alreadyExistsErrorMessage = alreadyExistsErrorMessage;
        }

        public override UserController<TUser> WithHttpContext(HttpContext context)
        {
            SetHttpContextRequestHeaders(context.Request.Headers);
            return this;
        }

        public override UserController<TUser> RequireHeaders(params string[] headers)
        {
            SetRequiredHeaders(headers);
            return this;
        }

        public override abstract Task<IActionResult> Create(TUser entity);
        public abstract Task<IActionResult> Login(LoginData loginData);
        public abstract Task<IActionResult> ChangePassword(LoginData loginData, string newPassword);
        public abstract Task<IActionResult> GetAccessToken(string login);

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public override async Task<IActionResult> CreateAsync(TUser user)
        {
            if (NoHeader(_requiredHeaders))
                return this.Forbidden(ErrorMessages.HeaderMissing);

            bool userExists = await _context.Set<TUser>().AnyAsync(u => u.Login.Equals(user.Login));

            if (userExists)
                return UnprocessableEntity(_alreadyExistsErrorMessage);

            if (InvalidModelState)
                return ValidationProblem();

            user.Id = Guid.NewGuid();
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            _context.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<IActionResult> RegisterAsync(TUser user)
        {
            if (NoHeader(_requiredHeaders))
                return Forbid(ErrorMessages.HeaderMissing);

            bool userExists = await _context.Set<TUser>().AnyAsync(u => u.Login.Equals(user.Login));

            if (userExists)
                return UnprocessableEntity(_alreadyExistsErrorMessage);

            if (InvalidModelState)
                return ValidationProblem();

            user.Id = Guid.NewGuid();
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            user.RefreshToken = TokenService.CreateRefreshToken(user.Login, user.Role);

            _context.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user.RefreshToken);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<IActionResult> LoginAsync(LoginData loginData)
        {
            if (NoHeader(_requiredHeaders))
                return this.Forbidden(ErrorMessages.HeaderMissing);

            if (loginData.IsEmptyLogin || loginData.IsEmptyPassword)
                return Unauthorized();

            var user = await _getByLogin(_context, loginData.Login);

            if (user is null)
                return Unauthorized();

            if (VerifyPassword(user, loginData.Password))
            {
                string token = TokenService.CreateRefreshToken(user.Login, user.Role);
                await SetRefreshTokenAsync(user, token);
                return Ok(token);
            }

            return Unauthorized();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<IActionResult> ChangePasswordAsync(LoginData loginData, string newPassword)
        {
            if (NoHeader(_requiredHeaders))
                return this.Forbidden(ErrorMessages.HeaderMissing);

            if (loginData.IsEmptyLogin || loginData.IsEmptyPassword || newPassword.IsEmpty())
                return Unauthorized();

            var user = await _getByLogin(_context, loginData.Login);

            if (user is null)
                return Unauthorized();

            if (VerifyPassword(user, loginData.Password))
            {
                string token = TokenService.CreateRefreshToken(user.Login, user.Role);
                await SetPasswordAndRefreshTokenAsync(user, newPassword, token);
                return Ok(token);
            }

            return Unauthorized();
        }

        // TODO: password restore

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<IActionResult> GetAccessTokenAsync(string login)
        {
            if (NoHeader(_requiredHeaders))
                return this.Forbidden(ErrorMessages.HeaderMissing);

            if (login.IsEmpty())
                return Unauthorized();

            var user = await _getByLogin(_context, login);

            if (user is null)
                return Unauthorized();

            if (VerifyRefreshToken(user, AuthorizationToken))
                return Ok(TokenService.CreateAccessToken(user.Login, user.Role));

            return Unauthorized();
        }

        private bool VerifyPassword(TUser user, string? password)
            => _passwordHasher.VerifyHashedPassword(user, user.Password, password ?? string.Empty)
                is not PasswordVerificationResult.Failed;

        private async Task<bool> SetPasswordAndRefreshTokenAsync(TUser user, string password, string? token)
        {
            user.Password = _passwordHasher.HashPassword(user, password);
            user.RefreshToken = token;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await EntityExistsAsync(user.Id) == false)
                    return false;
                else
                    throw;
            }
            return true;
        }

        private bool VerifyRefreshToken(TUser user, string? refreshToken)
            => user.RefreshToken is not null && user.RefreshToken.Equals(refreshToken);

        private async Task<bool> SetRefreshTokenAsync(TUser user, string? token)
        {
            user.RefreshToken = token;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await EntityExistsAsync(user.Id) == false)
                    return false;
                else
                    throw;
            }
            return true;
        }
    }
}
