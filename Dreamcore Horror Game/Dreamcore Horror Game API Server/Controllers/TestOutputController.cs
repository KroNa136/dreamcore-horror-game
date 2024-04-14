using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mime;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class TestOutputController : ControllerBase
{
    private DreamcoreHorrorGameContext _context;

    public TestOutputController(DreamcoreHorrorGameContext context)
        => _context = context;

    [HttpGet]
    public IResult Test()
    {
        var devs = _context.Developers.Include(developer => developer.DeveloperAccessLevel)
            .Select(dev => new { dev.Login, dev.DeveloperAccessLevel.Name }).ToList();

        return Results.Json
        (
            data: devs,
            options: JsonSerializerOptionsProvider.Shared,
            contentType: MediaTypeNames.Application.Json,
            statusCode: 200
        );

        //var dals = _context.DeveloperAccessLevels.ToList();
    }

    [HttpGet]
    public IActionResult TestTwo()
    {
        var server = new Server()
        {
            Id = Guid.NewGuid(),
            IpAddress = IPAddress.Parse("161.0.15.89"),
            Password = "password",
            RefreshToken = TokenService.CreateRefreshToken("161.0.15.89", AuthenticationRoles.Server),
            IsOnline = true,
            PlayerCapacity = 64
        };
        return Ok(server);

        //return Ok(JsonSerializer.Serialize(server.IpAddress, JsonSerializerOptionsProvider.Shared));
    }

    [HttpGet]
    public IActionResult TestThree()
    {
        var player = new Player()
        {
            Id = Guid.NewGuid(),
            Username = "test",
            Email = "test@gmail.com",
            XpLevelId = Guid.NewGuid(),
            Xp = 0,
            AbilityPoints = 0,
            SpiritEnergyPoints = 0,
            CollectOptionalData = true,
            IsOnline = true,
            RegistrationTimestamp = DateTime.UtcNow,
            RefreshToken = TokenService.CreateRefreshToken("test@gmail.com", AuthenticationRoles.Player)
        };

        Microsoft.AspNetCore.Identity.PasswordHasher<Player> passwordHasher = new();

        string plainTextPassword = "123456";
        string hashedPassword = passwordHasher.HashPassword(player, plainTextPassword);
        player.Password = hashedPassword;

        var start = DateTime.UtcNow;

        var c = player.Email.Equals("test@gmail.com") && passwordHasher.VerifyHashedPassword(player, player.Password, "123456") is not Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;

        var end = DateTime.UtcNow;

        return Ok((end - start).TotalMilliseconds);
    }
}
