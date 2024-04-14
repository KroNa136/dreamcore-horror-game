using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class XpLevelsController : DatabaseController
{
    public XpLevelsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.XpLevels.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var xpLevel = await _context.XpLevels.FindAsync(id);

        return xpLevel is not null
            ? Ok(xpLevel)
            : NotFound();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(XpLevel.Id),
        nameof(XpLevel.Number),
        nameof(XpLevel.RequiredXp)
    )] XpLevel xpLevel)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        xpLevel.Id = Guid.NewGuid();

        _context.Add(xpLevel);
        await _context.SaveChangesAsync();
        return Ok(xpLevel);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(XpLevel.Id),
        nameof(XpLevel.Number),
        nameof(XpLevel.RequiredXp)
    )] XpLevel xpLevel)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != xpLevel.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(xpLevel);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!XpLevelExists(xpLevel.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(xpLevel);
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

        var xpLevel = await _context.XpLevels.FindAsync(id);

        if (xpLevel is null)
            return NotFound();

        _context.Remove(xpLevel);
        await _context.SaveChangesAsync();
        return Ok();
    }

    private bool XpLevelExists(Guid id)
        => _context.XpLevels.Any(xpLevel => xpLevel.Id == id);
}
