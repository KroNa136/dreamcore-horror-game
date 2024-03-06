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
    public class RarityLevelsController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public RarityLevelsController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: RarityLevels
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return _context.RarityLevels != null ? 
                          View(await _context.RarityLevels.ToListAsync()) :
                          Problem("Entity set 'DreamcoreHorrorGameContext.RarityLevels'  is null.");
        }

        // GET: RarityLevels/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.RarityLevels == null)
            {
                return NotFound();
            }

            var rarityLevel = await _context.RarityLevels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rarityLevel == null)
            {
                return NotFound();
            }

            return View(rarityLevel);
        }

        // GET: RarityLevels/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: RarityLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssetName,Probability")] RarityLevel rarityLevel)
        {
            if (ModelState.IsValid)
            {
                rarityLevel.Id = Guid.NewGuid();
                _context.Add(rarityLevel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rarityLevel);
        }

        // GET: RarityLevels/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.RarityLevels == null)
            {
                return NotFound();
            }

            var rarityLevel = await _context.RarityLevels.FindAsync(id);
            if (rarityLevel == null)
            {
                return NotFound();
            }
            return View(rarityLevel);
        }

        // POST: RarityLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AssetName,Probability")] RarityLevel rarityLevel)
        {
            if (id != rarityLevel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rarityLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RarityLevelExists(rarityLevel.Id))
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
            return View(rarityLevel);
        }

        // GET: RarityLevels/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.RarityLevels == null)
            {
                return NotFound();
            }

            var rarityLevel = await _context.RarityLevels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rarityLevel == null)
            {
                return NotFound();
            }

            return View(rarityLevel);
        }

        // POST: RarityLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.RarityLevels == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.RarityLevels'  is null.");
            }
            var rarityLevel = await _context.RarityLevels.FindAsync(id);
            if (rarityLevel != null)
            {
                _context.RarityLevels.Remove(rarityLevel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RarityLevelExists(Guid id)
        {
          return (_context.RarityLevels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
