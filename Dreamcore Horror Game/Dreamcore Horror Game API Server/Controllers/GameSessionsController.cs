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
    public class GameSessionsController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public GameSessionsController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: GameSessions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dreamcoreHorrorGameContext = _context.GameSessions.Include(g => g.GameMode).Include(g => g.Server);
            return View(await dreamcoreHorrorGameContext.ToListAsync());
        }

        // GET: GameSessions/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.GameSessions == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions
                .Include(g => g.GameMode)
                .Include(g => g.Server)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameSession == null)
            {
                return NotFound();
            }

            return View(gameSession);
        }

        // GET: GameSessions/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["GameModeId"] = new SelectList(_context.GameModes, "Id", "Id");
            ViewData["ServerId"] = new SelectList(_context.Servers, "Id", "Id");
            return View();
        }

        // POST: GameSessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServerId,GameModeId,StartTimestamp,EndTimestamp")] GameSession gameSession)
        {
            if (ModelState.IsValid)
            {
                gameSession.Id = Guid.NewGuid();
                _context.Add(gameSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameModeId"] = new SelectList(_context.GameModes, "Id", "Id", gameSession.GameModeId);
            ViewData["ServerId"] = new SelectList(_context.Servers, "Id", "Id", gameSession.ServerId);
            return View(gameSession);
        }

        // GET: GameSessions/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.GameSessions == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions.FindAsync(id);
            if (gameSession == null)
            {
                return NotFound();
            }
            ViewData["GameModeId"] = new SelectList(_context.GameModes, "Id", "Id", gameSession.GameModeId);
            ViewData["ServerId"] = new SelectList(_context.Servers, "Id", "Id", gameSession.ServerId);
            return View(gameSession);
        }

        // POST: GameSessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ServerId,GameModeId,StartTimestamp,EndTimestamp")] GameSession gameSession)
        {
            if (id != gameSession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameSessionExists(gameSession.Id))
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
            ViewData["GameModeId"] = new SelectList(_context.GameModes, "Id", "Id", gameSession.GameModeId);
            ViewData["ServerId"] = new SelectList(_context.Servers, "Id", "Id", gameSession.ServerId);
            return View(gameSession);
        }

        // GET: GameSessions/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.GameSessions == null)
            {
                return NotFound();
            }

            var gameSession = await _context.GameSessions
                .Include(g => g.GameMode)
                .Include(g => g.Server)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameSession == null)
            {
                return NotFound();
            }

            return View(gameSession);
        }

        // POST: GameSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.GameSessions == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.GameSessions'  is null.");
            }
            var gameSession = await _context.GameSessions.FindAsync(id);
            if (gameSession != null)
            {
                _context.GameSessions.Remove(gameSession);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameSessionExists(Guid id)
        {
          return (_context.GameSessions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
