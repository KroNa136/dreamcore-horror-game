using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class ArtifactsController : DatabaseController
{
    public ArtifactsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public async Task<IActionResult> GetAll()
        => NoHeader(CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : Ok(await _context.Artifacts.ToListAsync());

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public async Task<IActionResult> Get(Guid? id)
        => NoHeader(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            ? Forbid(ErrorMessages.HeaderMissing)
            : id is not null
                && await _context.Artifacts.FindAsync(id) is Artifact artifact
            ? Ok(artifact)
            : NotFound();

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(
        nameof(Artifact.Id),
        nameof(Artifact.AssetName),
        nameof(Artifact.RarityLevelId)
    )] Artifact artifact)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (ModelState.IsValid)
        {
            artifact.Id = Guid.NewGuid();

            _context.Add(artifact);
            await _context.SaveChangesAsync();
            return Ok(artifact);
        }

        return BadRequest(ErrorMessages.InvalidModelData);
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Artifact.Id),
        nameof(Artifact.AssetName),
        nameof(Artifact.RarityLevelId)
    )] Artifact artifact)
    {
        if (NoHeader(CorsHeaders.DeveloperWebApplication))
            return Forbid(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != artifact.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(artifact);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtifactExists(artifact.Id))
                    return NotFound();
                else
                    throw;
            }
            return Ok(artifact);
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

        if (id is not null && await _context.Artifacts.FindAsync(id) is Artifact artifact)
        {
            _context.Remove(artifact);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return NotFound();
    }

    private bool ArtifactExists(Guid id)
        => _context.Artifacts.Any(artifact => artifact.Id == id);
}
