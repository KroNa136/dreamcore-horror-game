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
    public class CreaturesController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public CreaturesController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: Creatures
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dreamcoreHorrorGameContext = _context.Creatures.Include(c => c.RequiredExperienceLevel);
            return View(await dreamcoreHorrorGameContext.ToListAsync());
        }

        // GET: Creatures/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Creatures == null)
            {
                return NotFound();
            }

            var creature = await _context.Creatures
                .Include(c => c.RequiredExperienceLevel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creature == null)
            {
                return NotFound();
            }

            return View(creature);
        }

        // GET: Creatures/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["RequiredExperienceLevelId"] = new SelectList(_context.ExperienceLevels, "Id", "Id");
            return View();
        }

        // POST: Creatures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssetName,RequiredExperienceLevelId,Health,MovementSpeed")] Creature creature)
        {
            if (ModelState.IsValid)
            {
                creature.Id = Guid.NewGuid();
                _context.Add(creature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequiredExperienceLevelId"] = new SelectList(_context.ExperienceLevels, "Id", "Id", creature.RequiredExperienceLevelId);
            return View(creature);
        }

        // GET: Creatures/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Creatures == null)
            {
                return NotFound();
            }

            var creature = await _context.Creatures.FindAsync(id);
            if (creature == null)
            {
                return NotFound();
            }
            ViewData["RequiredExperienceLevelId"] = new SelectList(_context.ExperienceLevels, "Id", "Id", creature.RequiredExperienceLevelId);
            return View(creature);
        }

        // POST: Creatures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AssetName,RequiredExperienceLevelId,Health,MovementSpeed")] Creature creature)
        {
            if (id != creature.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreatureExists(creature.Id))
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
            ViewData["RequiredExperienceLevelId"] = new SelectList(_context.ExperienceLevels, "Id", "Id", creature.RequiredExperienceLevelId);
            return View(creature);
        }

        // GET: Creatures/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Creatures == null)
            {
                return NotFound();
            }

            var creature = await _context.Creatures
                .Include(c => c.RequiredExperienceLevel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (creature == null)
            {
                return NotFound();
            }

            return View(creature);
        }

        // POST: Creatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Creatures == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.Creatures'  is null.");
            }
            var creature = await _context.Creatures.FindAsync(id);
            if (creature != null)
            {
                _context.Creatures.Remove(creature);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreatureExists(Guid id)
        {
          return (_context.Creatures?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
