using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class PlayersController : UserController<Player>
{
    public PlayersController(DreamcoreHorrorGameContext context, IPasswordHasher<Player> passwordHasher)
        : base(
            context: context,
            passwordHasher: passwordHasher,
            getByLoginFunction: async (context, login) => await context.Players
                .FirstOrDefaultAsync(player => player.Email.Equals(login)),
            alreadyExistsErrorMessage: ErrorMessages.PlayerAlreadyExists
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
            .GetAsync(player => player.Id == id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
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
        nameof(Player.SpiritEnergyPoints)
    )] Player player)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateAsync(player);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
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
        nameof(Player.SpiritEnergyPoints)
    )] Player player)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditAsync(id, player);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteAsync(id);

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
        => await RequireHeaders(CorsHeaders.GameClient)
            .RegisterAsync(player);

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Login(LoginData loginData)
        => await RequireHeaders(CorsHeaders.GameClient)
            .LoginAsync(loginData);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Player)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> ChangePassword(LoginData loginData, string newPassword)
        => await RequireHeaders(CorsHeaders.GameClient)
            .ChangePasswordAsync(loginData, newPassword);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Refresh, Roles = AuthenticationRoles.Player)]
    public override async Task<IActionResult> GetAccessToken(string email)
        => await RequireHeaders(CorsHeaders.GameClient)
            .GetAccessTokenAsync(email);
}
