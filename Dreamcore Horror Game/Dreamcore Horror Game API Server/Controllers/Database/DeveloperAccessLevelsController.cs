using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class DeveloperAccessLevelsController : DatabaseController
{
    public DeveloperAccessLevelsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.DeveloperAccessLevels.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var developerAccessLevel = await _context.DeveloperAccessLevels.FindAsync(id);

        return developerAccessLevel is not null
            ? Ok(developerAccessLevel)
            : NotFound();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(DeveloperAccessLevel.Id),
        nameof(DeveloperAccessLevel.Name)
    )] DeveloperAccessLevel developerAccessLevel)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        developerAccessLevel.Id = Guid.NewGuid();

        _context.Add(developerAccessLevel);
        await _context.SaveChangesAsync();
        return Ok(developerAccessLevel);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(DeveloperAccessLevel.Id),
        nameof(DeveloperAccessLevel.Name)
    )] DeveloperAccessLevel developerAccessLevel)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != developerAccessLevel.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(developerAccessLevel);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DeveloperAccessLevelExists(developerAccessLevel.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(developerAccessLevel);
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

        var developerAccessLevel = await _context.DeveloperAccessLevels.FindAsync(id);

        if (developerAccessLevel is null)
            return NotFound();

        _context.Remove(developerAccessLevel);
        await _context.SaveChangesAsync();
        return Ok();
    }

    private bool DeveloperAccessLevelExists(Guid id)
        => _context.DeveloperAccessLevels.Any(developerAccessLevel => developerAccessLevel.Id == id);
}
