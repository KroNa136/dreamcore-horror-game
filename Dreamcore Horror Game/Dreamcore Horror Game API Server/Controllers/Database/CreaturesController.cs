using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class CreaturesController : DatabaseController
{
    public CreaturesController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.Creatures.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var creature = await _context.Creatures.FindAsync(id);

        return creature is not null
            ? Ok(creature)
            : NotFound();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(Creature.Id),
        nameof(Creature.AssetName),
        nameof(Creature.RequiredXpLevelId),
        nameof(Creature.Health),
        nameof(Creature.MovementSpeed)
    )] Creature creature)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        creature.Id = Guid.NewGuid();

        _context.Add(creature);
        await _context.SaveChangesAsync();
        return Ok(creature);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Creature.Id),
        nameof(Creature.AssetName),
        nameof(Creature.RequiredXpLevelId),
        nameof(Creature.Health),
        nameof(Creature.MovementSpeed)
    )] Creature creature)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != creature.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(creature);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CreatureExists(creature.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(creature);
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

        var creature = await _context.Creatures.FindAsync(id);

        if (creature is null)
            return NotFound();

        _context.Remove(creature);
        await _context.SaveChangesAsync();
        return Ok();
    }

    private bool CreatureExists(Guid id)
        => _context.Creatures.Any(creature => creature.Id == id);
}
