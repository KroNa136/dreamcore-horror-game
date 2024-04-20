using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mime;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class TestOutputController : ControllerBase
{
    private readonly DreamcoreHorrorGameContext _context;
    private readonly ITokenService _tokenService;
    private readonly IJsonSerializerOptionsProvider _jsonSerializerOptionsProvider;

    public TestOutputController(DreamcoreHorrorGameContext context, ITokenService tokenService, IJsonSerializerOptionsProvider jsonSerializerOptionsProvider)
    {
        _context = context;
        _tokenService = tokenService;
        _jsonSerializerOptionsProvider = jsonSerializerOptionsProvider;
    }

    [HttpGet]
    public async Task<IActionResult> RandomizePlayerLevels()
    {
        var players = _context.Players.ToList();

        foreach (var player in players)
        {
            var count = _context.XpLevels.Count();
            player.XpLevel = _context.XpLevels.AsEnumerable().ElementAt(new Random().Next(0, count));

            _context.Update(player);
            await _context.SaveChangesAsync();
        }
        return Ok();
    }

    [HttpGet]
    public IResult Test()
    {
        var devs = _context.Developers.Include(developer => developer.DeveloperAccessLevel)
            .Select(dev => new { dev.Login, dev.DeveloperAccessLevel.Name }).ToList();

        return Results.Json
        (
            data: devs,
            options: _jsonSerializerOptionsProvider.Default,
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
            RefreshToken = _tokenService.CreateRefreshToken("161.0.15.89", AuthenticationRoles.Server),
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
            RefreshToken = _tokenService.CreateRefreshToken("test@gmail.com", AuthenticationRoles.Player)
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
