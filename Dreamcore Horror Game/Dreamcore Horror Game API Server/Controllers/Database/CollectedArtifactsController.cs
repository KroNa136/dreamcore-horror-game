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
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : Ok(await _context.CollectedArtifacts.ToListAsync());

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public async Task<IActionResult> Get(Guid? id)
        => NoHeader(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : id is not null
                && await _context.CollectedArtifacts.FindAsync(id) is CollectedArtifact collectedArtifact
            ? Ok(collectedArtifact)
            : NotFound();

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

        if (ModelState.IsValid)
        {
            collectedArtifact.Id = Guid.NewGuid();

            _context.Add(collectedArtifact);
            await _context.SaveChangesAsync();
            return Ok(collectedArtifact);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
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

        if (ModelState.IsValid)
        {
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

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is not null && await _context.CollectedArtifacts.FindAsync(id) is CollectedArtifact collectedArtifact)
        {
            _context.Remove(collectedArtifact);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }

    private bool CollectedArtifactExists(Guid id)
        => _context.CollectedArtifacts.Any(collectedArtifact => collectedArtifact.Id == id);
}
