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
    public class PlayerSessionsController : Controller
    {
        private readonly DreamcoreHorrorGameContext _context;

        public PlayerSessionsController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // GET: PlayerSessions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dreamcoreHorrorGameContext = _context.PlayerSessions.Include(p => p.GameSession).Include(p => p.Player).Include(p => p.UsedCreature);
            return View(await dreamcoreHorrorGameContext.ToListAsync());
        }

        // GET: PlayerSessions/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.PlayerSessions == null)
            {
                return NotFound();
            }

            var playerSession = await _context.PlayerSessions
                .Include(p => p.GameSession)
                .Include(p => p.Player)
                .Include(p => p.UsedCreature)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerSession == null)
            {
                return NotFound();
            }

            return View(playerSession);
        }

        // GET: PlayerSessions/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["GameSessionId"] = new SelectList(_context.GameSessions, "Id", "Id");
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id");
            ViewData["UsedCreatureId"] = new SelectList(_context.Creatures, "Id", "Id");
            return View();
        }

        // POST: PlayerSessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameSessionId,PlayerId,StartTimestamp,EndTimestamp,IsCompleted,IsWon,TimeAlive,PlayedAsCreature,UsedCreatureId")] PlayerSession playerSession)
        {
            if (ModelState.IsValid)
            {
                playerSession.Id = Guid.NewGuid();
                _context.Add(playerSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameSessionId"] = new SelectList(_context.GameSessions, "Id", "Id", playerSession.GameSessionId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", playerSession.PlayerId);
            ViewData["UsedCreatureId"] = new SelectList(_context.Creatures, "Id", "Id", playerSession.UsedCreatureId);
            return View(playerSession);
        }

        // GET: PlayerSessions/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.PlayerSessions == null)
            {
                return NotFound();
            }

            var playerSession = await _context.PlayerSessions.FindAsync(id);
            if (playerSession == null)
            {
                return NotFound();
            }
            ViewData["GameSessionId"] = new SelectList(_context.GameSessions, "Id", "Id", playerSession.GameSessionId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", playerSession.PlayerId);
            ViewData["UsedCreatureId"] = new SelectList(_context.Creatures, "Id", "Id", playerSession.UsedCreatureId);
            return View(playerSession);
        }

        // POST: PlayerSessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,GameSessionId,PlayerId,StartTimestamp,EndTimestamp,IsCompleted,IsWon,TimeAlive,PlayedAsCreature,UsedCreatureId")] PlayerSession playerSession)
        {
            if (id != playerSession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playerSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerSessionExists(playerSession.Id))
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
            ViewData["GameSessionId"] = new SelectList(_context.GameSessions, "Id", "Id", playerSession.GameSessionId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Id", playerSession.PlayerId);
            ViewData["UsedCreatureId"] = new SelectList(_context.Creatures, "Id", "Id", playerSession.UsedCreatureId);
            return View(playerSession);
        }

        // GET: PlayerSessions/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.PlayerSessions == null)
            {
                return NotFound();
            }

            var playerSession = await _context.PlayerSessions
                .Include(p => p.GameSession)
                .Include(p => p.Player)
                .Include(p => p.UsedCreature)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerSession == null)
            {
                return NotFound();
            }

            return View(playerSession);
        }

        // POST: PlayerSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.PlayerSessions == null)
            {
                return Problem("Entity set 'DreamcoreHorrorGameContext.PlayerSessions'  is null.");
            }
            var playerSession = await _context.PlayerSessions.FindAsync(id);
            if (playerSession != null)
            {
                _context.PlayerSessions.Remove(playerSession);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerSessionExists(Guid id)
        {
          return (_context.PlayerSessions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
