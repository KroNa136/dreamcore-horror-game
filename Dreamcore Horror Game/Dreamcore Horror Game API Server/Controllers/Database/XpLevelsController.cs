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
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : Ok(await _context.XpLevels.ToListAsync());

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public async Task<IActionResult> Get(Guid? id)
        => NoHeader(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : id is not null
                && await _context.XpLevels.FindAsync(id) is XpLevel xpLevel
            ? Ok(xpLevel)
            : NotFound();

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

        if (ModelState.IsValid)
        {
            xpLevel.Id = Guid.NewGuid();

            _context.Add(xpLevel);
            await _context.SaveChangesAsync();
            return Ok(xpLevel);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
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

        if (ModelState.IsValid)
        {
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

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is not null && await _context.XpLevels.FindAsync(id) is XpLevel xpLevel)
        {
            _context.Remove(xpLevel);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }

    private bool XpLevelExists(Guid id)
        => _context.XpLevels.Any(xpLevel => xpLevel.Id == id);
}
