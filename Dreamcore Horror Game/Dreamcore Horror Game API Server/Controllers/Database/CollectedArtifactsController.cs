using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class CollectedArtifactsController : DatabaseController
{
    public CollectedArtifactsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        return Ok(await _context.CollectedArtifacts.ToListAsync());
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public async Task<IActionResult> Get(Guid? id)
    {
        if (NoHeader(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);

        return collectedArtifact is not null
            ? Ok(collectedArtifact)
            : NotFound();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(CollectedArtifact.Id),
        nameof(CollectedArtifact.PlayerId),
        nameof(CollectedArtifact.ArtifactId),
        nameof(CollectedArtifact.CollectionTimestamp)
    )] CollectedArtifact collectedArtifact)
    {
        if (NoHeader(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        collectedArtifact.Id = Guid.NewGuid();

        _context.Add(collectedArtifact);
        await _context.SaveChangesAsync();
        return Ok(collectedArtifact);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(CollectedArtifact.Id),
        nameof(CollectedArtifact.PlayerId),
        nameof(CollectedArtifact.ArtifactId),
        nameof(CollectedArtifact.CollectionTimestamp)
    )] CollectedArtifact collectedArtifact)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != collectedArtifact.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return BadRequest(ErrorMessages.InvalidModelData);

        try
        {
            _context.Update(collectedArtifact);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CollectedArtifactExists(collectedArtifact.Id))
                return NotFound();
            else
                throw;
        }

        return Ok(collectedArtifact);
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

        var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);

        if (collectedArtifact is null)
            return NotFound();

        _context.Remove(collectedArtifact);
        await _context.SaveChangesAsync();
        return Ok();
    }

    private bool CollectedArtifactExists(Guid id)
        => _context.CollectedArtifacts.Any(collectedArtifact => collectedArtifact.Id == id);
}
