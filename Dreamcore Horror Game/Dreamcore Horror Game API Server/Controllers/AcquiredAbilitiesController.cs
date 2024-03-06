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
    public class AcquiredAbilitiesController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public AcquiredAbilitiesController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: AcquiredAbilities
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dreamcoreHorrorGameContext = _context.AcquiredAbilities.Include(a => a.Ability).Include(a => a.Player);
            return View(await dreamcoreHorrorGameContext.ToListAsync());
        }

        // GET: AcquiredAbilities/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.AcquiredAbilities == null)
            {
                return NotFound();
            }

            var acquiredAbility = await _context.AcquiredAbilities
                .Include(a => a.Ability)
                .Include(a => a.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (acquiredAbility == null)
            {
                return NotFound();
            }

            return View(acquiredAbility);
        }

        // GET: AcquiredAbilities/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["AbilityId"] = new SelectList(_context.Abilities, "Id", "Id");
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id");
            return View();
        }

        // POST: AcquiredAbilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlayerId,AbilityId,AcquirementTimestamp")] AcquiredAbility acquiredAbility)
        {
            if (ModelState.IsValid)
            {
                acquiredAbility.Id = Guid.NewGuid();
                _context.Add(acquiredAbility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AbilityId"] = new SelectList(_context.Abilities, "Id", "Id", acquiredAbility.AbilityId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", acquiredAbility.PlayerId);
            return View(acquiredAbility);
        }

        // GET: AcquiredAbilities/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.AcquiredAbilities == null)
            {
                return NotFound();
            }

            var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);
            if (acquiredAbility == null)
            {
                return NotFound();
            }
            ViewData["AbilityId"] = new SelectList(_context.Abilities, "Id", "Id", acquiredAbility.AbilityId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", acquiredAbility.PlayerId);
            return View(acquiredAbility);
        }

        // POST: AcquiredAbilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,PlayerId,AbilityId,AcquirementTimestamp")] AcquiredAbility acquiredAbility)
        {
            if (id != acquiredAbility.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acquiredAbility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcquiredAbilityExists(acquiredAbility.Id))
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
            ViewData["AbilityId"] = new SelectList(_context.Abilities, "Id", "Id", acquiredAbility.AbilityId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", acquiredAbility.PlayerId);
            return View(acquiredAbility);
        }

        // GET: AcquiredAbilities/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.AcquiredAbilities == null)
            {
                return NotFound();
            }

            var acquiredAbility = await _context.AcquiredAbilities
                .Include(a => a.Ability)
                .Include(a => a.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (acquiredAbility == null)
            {
                return NotFound();
            }

            return View(acquiredAbility);
        }

        // POST: AcquiredAbilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.AcquiredAbilities == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.AcquiredAbilities'  is null.");
            }
            var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);
            if (acquiredAbility != null)
            {
                _context.AcquiredAbilities.Remove(acquiredAbility);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcquiredAbilityExists(Guid id)
        {
          return (_context.AcquiredAbilities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
