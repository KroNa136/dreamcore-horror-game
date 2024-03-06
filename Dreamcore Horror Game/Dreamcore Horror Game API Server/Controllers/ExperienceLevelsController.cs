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
    public class ExperienceLevelsController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public ExperienceLevelsController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: ExperienceLevels
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return _context.ExperienceLevels != null ? 
                          View(await _context.ExperienceLevels.ToListAsync()) :
                          Problem("Entity set 'DreamcoreHorrorGameContext.ExperienceLevels'  is null.");
        }

        // GET: ExperienceLevels/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ExperienceLevels == null)
            {
                return NotFound();
            }

            var experienceLevel = await _context.ExperienceLevels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (experienceLevel == null)
            {
                return NotFound();
            }

            return View(experienceLevel);
        }

        // GET: ExperienceLevels/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExperienceLevels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,RequiredExperiencePoints")] ExperienceLevel experienceLevel)
        {
            if (ModelState.IsValid)
            {
                experienceLevel.Id = Guid.NewGuid();
                _context.Add(experienceLevel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(experienceLevel);
        }

        // GET: ExperienceLevels/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ExperienceLevels == null)
            {
                return NotFound();
            }

            var experienceLevel = await _context.ExperienceLevels.FindAsync(id);
            if (experienceLevel == null)
            {
                return NotFound();
            }
            return View(experienceLevel);
        }

        // POST: ExperienceLevels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Number,RequiredExperiencePoints")] ExperienceLevel experienceLevel)
        {
            if (id != experienceLevel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(experienceLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExperienceLevelExists(experienceLevel.Id))
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
            return View(experienceLevel);
        }

        // GET: ExperienceLevels/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ExperienceLevels == null)
            {
                return NotFound();
            }

            var experienceLevel = await _context.ExperienceLevels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (experienceLevel == null)
            {
                return NotFound();
            }

            return View(experienceLevel);
        }

        // POST: ExperienceLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ExperienceLevels == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.ExperienceLevels'  is null.");
            }
            var experienceLevel = await _context.ExperienceLevels.FindAsync(id);
            if (experienceLevel != null)
            {
                _context.ExperienceLevels.Remove(experienceLevel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExperienceLevelExists(Guid id)
        {
          return (_context.ExperienceLevels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
