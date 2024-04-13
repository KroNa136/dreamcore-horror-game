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
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : Ok(await _context.DeveloperAccessLevels.ToListAsync());

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public async Task<IActionResult> Get(Guid? id)
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : id is not null
                && await _context.DeveloperAccessLevels.FindAsync(id) is DeveloperAccessLevel developerAccessLevel
            ? Ok(developerAccessLevel)
            : NotFound();

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

        if (ModelState.IsValid)
        {
            developerAccessLevel.Id = Guid.NewGuid();

            _context.Add(developerAccessLevel);
            await _context.SaveChangesAsync();
            return Ok(developerAccessLevel);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
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

        if (ModelState.IsValid)
        {
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

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is not null && await _context.DeveloperAccessLevels.FindAsync(id) is DeveloperAccessLevel developerAccessLevel)
        {
            _context.Remove(developerAccessLevel);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }

    private bool DeveloperAccessLevelExists(Guid id)
        => _context.DeveloperAccessLevels.Any(developerAccessLevel => developerAccessLevel.Id == id);
}
