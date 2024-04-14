using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class AcquiredAbilitiesController : DatabaseController
{
    public AcquiredAbilitiesController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.AcquiredAbilities.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);

        return acquiredAbility is not null
            ? Ok(acquiredAbility)
            : NotFound();
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(AcquiredAbility.Id),
        nameof(AcquiredAbility.PlayerId),
        nameof(AcquiredAbility.AbilityId),
        nameof(AcquiredAbility.AcquirementTimestamp)
    )] AcquiredAbility acquiredAbility)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        acquiredAbility.Id = Guid.NewGuid();

        _context.Add(acquiredAbility);
        await _context.SaveChangesAsync();
        return Ok(acquiredAbility);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(AcquiredAbility.Id),
        nameof(AcquiredAbility.PlayerId),
        nameof(AcquiredAbility.AbilityId),
        nameof(AcquiredAbility.AcquirementTimestamp)
    )] AcquiredAbility acquiredAbility)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != acquiredAbility.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(acquiredAbility);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AcquiredAbilityExists(acquiredAbility.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(acquiredAbility);
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

        var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);

        if (acquiredAbility is null)
            return NotFound();

        _context.Remove(acquiredAbility);
        await _context.SaveChangesAsync();
        return Ok();
    }

    private bool AcquiredAbilityExists(Guid id)
        => _context.AcquiredAbilities.Any(acquiredAbility => acquiredAbility.Id == id);
}
