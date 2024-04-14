﻿using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class PlayerSessionsController : DatabaseController
{
    public PlayerSessionsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.PlayerSessions.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var playerSession = await _context.PlayerSessions.FindAsync(id);

        return playerSession is not null
            ? Ok(playerSession)
            : NotFound();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(PlayerSession.Id),
        nameof(PlayerSession.GameSessionId),
        nameof(PlayerSession.PlayerId),
        nameof(PlayerSession.StartTimestamp),
        nameof(PlayerSession.EndTimestamp),
        nameof(PlayerSession.IsCompleted),
        nameof(PlayerSession.IsWon),
        nameof(PlayerSession.TimeAlive),
        nameof(PlayerSession.PlayedAsCreature),
        nameof(PlayerSession.UsedCreatureId),
        nameof(PlayerSession.SelfReviveCount),
        nameof(PlayerSession.AllyReviveCount)
    )] PlayerSession playerSession)
    {
        if (NoHeader(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        playerSession.Id = Guid.NewGuid();

        _context.Add(playerSession);
        await _context.SaveChangesAsync();
        return Ok(playerSession);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(PlayerSession.Id),
        nameof(PlayerSession.GameSessionId),
        nameof(PlayerSession.PlayerId),
        nameof(PlayerSession.StartTimestamp),
        nameof(PlayerSession.EndTimestamp),
        nameof(PlayerSession.IsCompleted),
        nameof(PlayerSession.IsWon),
        nameof(PlayerSession.TimeAlive),
        nameof(PlayerSession.PlayedAsCreature),
        nameof(PlayerSession.UsedCreatureId),
        nameof(PlayerSession.SelfReviveCount),
        nameof(PlayerSession.AllyReviveCount)
    )] PlayerSession playerSession)
    {
        if (NoHeader(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != playerSession.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(playerSession);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlayerSessionExists(playerSession.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(playerSession);
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

        var playerSession = await _context.PlayerSessions.FindAsync(id);

        if (playerSession is null)
            return NotFound();

        _context.Remove(playerSession);
        await _context.SaveChangesAsync();
        return Ok();
    }

    private bool PlayerSessionExists(Guid id)
        => _context.PlayerSessions.Any(playerSession => playerSession.Id == id);
}
