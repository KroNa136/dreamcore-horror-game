using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ArtifactsController : DatabaseController
    {
        public ArtifactsController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetArtifacts()
        {
            return _context.Artifacts == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Artifacts.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetArtifact(Guid? id)
        {
            if (id == null || _context.Artifacts == null)
                return NotFound();

            var artifact = await _context.Artifacts.FindAsync(id);

            if (artifact == null)
                return NotFound();

            return Ok(artifact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArtifact([Bind("Id,AssetName,RarityLevelId")] Artifact artifact)
        {
            if (ModelState.IsValid)
            {
                artifact.Id = Guid.NewGuid();
                _context.Add(artifact);
                await _context.SaveChangesAsync();
                return Ok(artifact);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtifact(Guid? id, [Bind("Id,AssetName,RarityLevelId")] Artifact artifact)
        {
            if (id == null || _context.Artifacts == null)
                return NotFound();

            if (id != artifact.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

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

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArtifact(Guid? id)
        {
            if (id == null || _context.Artifacts == null)
                return NotFound();

            var artifact = await _context.Artifacts.FindAsync(id);

            if (artifact == null)
                return NotFound();

            _context.Artifacts.Remove(artifact);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ArtifactExists(Guid id) => (_context.Artifacts?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
