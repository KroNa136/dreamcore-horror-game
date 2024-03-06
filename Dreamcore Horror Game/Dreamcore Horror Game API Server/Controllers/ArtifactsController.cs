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
    public class ArtifactsController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public ArtifactsController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: Artifacts
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dreamcoreHorrorGameContext = _context.Artifacts.Include(a => a.RarityLevel);
            return View(await dreamcoreHorrorGameContext.ToListAsync());
        }

        // GET: Artifacts/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Artifacts == null)
            {
                return NotFound();
            }

            var artifact = await _context.Artifacts
                .Include(a => a.RarityLevel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artifact == null)
            {
                return NotFound();
            }

            return View(artifact);
        }

        // GET: Artifacts/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["RarityLevelId"] = new SelectList(_context.RarityLevels, "Id", "Id");
            return View();
        }

        // POST: Artifacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssetName,RarityLevelId")] Artifact artifact)
        {
            if (ModelState.IsValid)
            {
                artifact.Id = Guid.NewGuid();
                _context.Add(artifact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RarityLevelId"] = new SelectList(_context.RarityLevels, "Id", "Id", artifact.RarityLevelId);
            return View(artifact);
        }

        // GET: Artifacts/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Artifacts == null)
            {
                return NotFound();
            }

            var artifact = await _context.Artifacts.FindAsync(id);
            if (artifact == null)
            {
                return NotFound();
            }
            ViewData["RarityLevelId"] = new SelectList(_context.RarityLevels, "Id", "Id", artifact.RarityLevelId);
            return View(artifact);
        }

        // POST: Artifacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AssetName,RarityLevelId")] Artifact artifact)
        {
            if (id != artifact.Id)
            {
                return NotFound();
            }

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
            ViewData["RarityLevelId"] = new SelectList(_context.RarityLevels, "Id", "Id", artifact.RarityLevelId);
            return View(artifact);
        }

        // GET: Artifacts/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Artifacts == null)
            {
                return NotFound();
            }

            var artifact = await _context.Artifacts
                .Include(a => a.RarityLevel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artifact == null)
            {
                return NotFound();
            }

            return View(artifact);
        }

        // POST: Artifacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Artifacts == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.Artifacts'  is null.");
            }
            var artifact = await _context.Artifacts.FindAsync(id);
            if (artifact != null)
            {
                _context.Artifacts.Remove(artifact);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtifactExists(Guid id)
        {
          return (_context.Artifacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
