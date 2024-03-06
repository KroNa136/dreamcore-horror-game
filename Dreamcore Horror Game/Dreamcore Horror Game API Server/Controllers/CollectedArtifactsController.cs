using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dreamcore_Horror_Game_API_Server;

namespace Dreamcore_Horror_Game_API_Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CollectedArtifactsController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public CollectedArtifactsController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: CollectedArtifacts
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dreamcoreHorrorGameContext = _context.CollectedArtifacts.Include(c => c.Artifact).Include(c => c.Player);
            return View(await dreamcoreHorrorGameContext.ToListAsync());
        }

        // GET: CollectedArtifacts/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.CollectedArtifacts == null)
            {
                return NotFound();
            }

            var collectedArtifact = await _context.CollectedArtifacts
                .Include(c => c.Artifact)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collectedArtifact == null)
            {
                return NotFound();
            }

            return View(collectedArtifact);
        }

        // GET: CollectedArtifacts/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["ArtifactId"] = new SelectList(_context.Artifacts, "Id", "Id");
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id");
            return View();
        }

        // POST: CollectedArtifacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlayerId,ArtifactId,CollectionTimestamp")] CollectedArtifact collectedArtifact)
        {
            if (ModelState.IsValid)
            {
                collectedArtifact.Id = Guid.NewGuid();
                _context.Add(collectedArtifact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtifactId"] = new SelectList(_context.Artifacts, "Id", "Id", collectedArtifact.ArtifactId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", collectedArtifact.PlayerId);
            return View(collectedArtifact);
        }

        // GET: CollectedArtifacts/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.CollectedArtifacts == null)
            {
                return NotFound();
            }

            var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);
            if (collectedArtifact == null)
            {
                return NotFound();
            }
            ViewData["ArtifactId"] = new SelectList(_context.Artifacts, "Id", "Id", collectedArtifact.ArtifactId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", collectedArtifact.PlayerId);
            return View(collectedArtifact);
        }

        // POST: CollectedArtifacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,PlayerId,ArtifactId,CollectionTimestamp")] CollectedArtifact collectedArtifact)
        {
            if (id != collectedArtifact.Id)
            {
                return NotFound();
            }

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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArtifactId"] = new SelectList(_context.Artifacts, "Id", "Id", collectedArtifact.ArtifactId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", collectedArtifact.PlayerId);
            return View(collectedArtifact);
        }

        // GET: CollectedArtifacts/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.CollectedArtifacts == null)
            {
                return NotFound();
            }

            var collectedArtifact = await _context.CollectedArtifacts
                .Include(c => c.Artifact)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collectedArtifact == null)
            {
                return NotFound();
            }

            return View(collectedArtifact);
        }

        // POST: CollectedArtifacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.CollectedArtifacts == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.CollectedArtifacts'  is null.");
            }
            var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);
            if (collectedArtifact != null)
            {
                _context.CollectedArtifacts.Remove(collectedArtifact);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectedArtifactExists(Guid id)
        {
          return (_context.CollectedArtifacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
