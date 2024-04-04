using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CollectedArtifactsController : DatabaseController
    {
        public CollectedArtifactsController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetCollectedArtifacts()
        {
            return _context.CollectedArtifacts == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.CollectedArtifacts.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCollectedArtifact(Guid? id)
        {
            if (id == null || _context.CollectedArtifacts == null)
                return NotFound();

            var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);

            if (collectedArtifact == null)
                return NotFound();

            return Ok(collectedArtifact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCollectedArtifact([Bind("Id,PlayerId,ArtifactId,CollectionTimestamp")] CollectedArtifact collectedArtifact)
        {
            if (ModelState.IsValid)
            {
                collectedArtifact.Id = Guid.NewGuid();
                _context.Add(collectedArtifact);
                await _context.SaveChangesAsync();
                return Ok(collectedArtifact);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCollectedArtifact(Guid? id, [Bind("Id,PlayerId,ArtifactId,CollectionTimestamp")] CollectedArtifact collectedArtifact)
        {
            if (id == null || _context.CollectedArtifacts == null)
                return NotFound();

            if (id != collectedArtifact.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

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

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCollectedArtifact(Guid? id)
        {
            if (id == null || _context.CollectedArtifacts == null)
                return NotFound();

            var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);

            if (collectedArtifact == null)
                return NotFound();

            _context.CollectedArtifacts.Remove(collectedArtifact);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool CollectedArtifactExists(Guid id) => (_context.CollectedArtifacts?.Any(x => x.Id == id)).GetValueOrDefault();
    }
}
