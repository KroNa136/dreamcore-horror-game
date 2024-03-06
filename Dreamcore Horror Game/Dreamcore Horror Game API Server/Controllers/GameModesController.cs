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
    public class GameModesController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public GameModesController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: GameModes
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return _context.GameModes != null ? 
                          View(await _context.GameModes.ToListAsync()) :
                          Problem("Entity set 'DreamcoreHorrorGameContext.GameModes'  is null.");
        }

        // GET: GameModes/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.GameModes == null)
            {
                return NotFound();
            }

            var gameMode = await _context.GameModes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameMode == null)
            {
                return NotFound();
            }

            return View(gameMode);
        }

        // GET: GameModes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameModes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssetName,MaxPlayers,TimeLimit,IsActive")] GameMode gameMode)
        {
            if (ModelState.IsValid)
            {
                gameMode.Id = Guid.NewGuid();
                _context.Add(gameMode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gameMode);
        }

        // GET: GameModes/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.GameModes == null)
            {
                return NotFound();
            }

            var gameMode = await _context.GameModes.FindAsync(id);
            if (gameMode == null)
            {
                return NotFound();
            }
            return View(gameMode);
        }

        // POST: GameModes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AssetName,MaxPlayers,TimeLimit,IsActive")] GameMode gameMode)
        {
            if (id != gameMode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameMode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameModeExists(gameMode.Id))
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
            return View(gameMode);
        }

        // GET: GameModes/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.GameModes == null)
            {
                return NotFound();
            }

            var gameMode = await _context.GameModes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameMode == null)
            {
                return NotFound();
            }

            return View(gameMode);
        }

        // POST: GameModes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.GameModes == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.GameModes'  is null.");
            }
            var gameMode = await _context.GameModes.FindAsync(id);
            if (gameMode != null)
            {
                _context.GameModes.Remove(gameMode);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameModeExists(Guid id)
        {
          return (_context.GameModes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
