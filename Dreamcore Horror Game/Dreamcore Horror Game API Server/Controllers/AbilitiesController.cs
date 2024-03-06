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
    public class AbilitiesController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public AbilitiesController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: Abilities
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return _context.Abilities != null ? 
                          View(await _context.Abilities.ToListAsync()) :
                          Problem("Entity set 'DreamcoreHorrorGameContext.Abilities'  is null.");
        }

        // GET: Abilities/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Abilities == null)
            {
                return NotFound();
            }

            var ability = await _context.Abilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ability == null)
            {
                return NotFound();
            }

            return View(ability);
        }

        // GET: Abilities/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Abilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssetName")] Ability ability)
        {
            if (ModelState.IsValid)
            {
                ability.Id = Guid.NewGuid();
                _context.Add(ability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ability);
        }

        // GET: Abilities/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Abilities == null)
            {
                return NotFound();
            }

            var ability = await _context.Abilities.FindAsync(id);
            if (ability == null)
            {
                return NotFound();
            }
            return View(ability);
        }

        // POST: Abilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AssetName")] Ability ability)
        {
            if (id != ability.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbilityExists(ability.Id))
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
            return View(ability);
        }

        // GET: Abilities/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Abilities == null)
            {
                return NotFound();
            }

            var ability = await _context.Abilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ability == null)
            {
                return NotFound();
            }

            return View(ability);
        }

        // POST: Abilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Abilities == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.Abilities'  is null.");
            }
            var ability = await _context.Abilities.FindAsync(id);
            if (ability != null)
            {
                _context.Abilities.Remove(ability);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AbilityExists(Guid id)
        {
          return (_context.Abilities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
