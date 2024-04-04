using Dreamcore_Horror_Game_API_Server.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers.Database
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ServersController : DatabaseController
    {
        public ServersController(DreamcoreHorrorGameContext context) : base(context) { }

        [HttpGet]
        public async Task<IActionResult> GetServers()
        {
            return _context.Servers == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Servers.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetServer(Guid? id)
        {
            if (id == null || _context.Servers == null)
                return NotFound();

            var server = await _context.Servers.FindAsync(id);

            if (server == null)
                return NotFound();

            return Ok(server);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateServer([Bind("Id,IpAddress,PlayerCapacity,IsOnline")] Server server)
        {
            if (ModelState.IsValid)
            {
                server.Id = Guid.NewGuid();
                _context.Add(server);
                await _context.SaveChangesAsync();
                return Ok(server);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditServer(Guid? id, [Bind("Id,IpAddress,PlayerCapacity,IsOnline")] Server server)
        {
            if (id == null || _context.Servers == null)
                return NotFound();

            if (id != server.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(server);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServerExists(server.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(server);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServer(Guid? id)
        {
            if (id == null || _context.Servers == null)
                return NotFound();

            var server = await _context.Servers.FindAsync(id);

            if (server == null)
                return NotFound();

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ServerExists(Guid id) => (_context.Servers?.Any(x => x.Id == id)).GetValueOrDefault();

        private IEnumerable<Server> GetServersWithEnoughFreePlayerSlots(IEnumerable<Server> servers, int playerSlots)
        {
            List<Server> result = new();

            if (servers == null)
                return result;

            var onlineServers = servers.Where(server => server.IsOnline);

            foreach (Server server in onlineServers)
            {
                var activePlayerSessions = _context.PlayerSessions.Where
                (
                    playerSession => playerSession.EndTimestamp == null &&
                                     playerSession.GameSession.Server != null &&
                                     playerSession.GameSession.Server.Id == server.Id
                );

                if (activePlayerSessions.Count() + playerSlots <= server.PlayerCapacity)
                    result.Add(server);
            }

            return result;
        }

        private IEnumerable<Server> GetServersWithWaitingSessions(IEnumerable<Server> servers, int playerSlots)
        {
            List<Server> result = new();

            if (servers == null)
                return result;

            servers.ToList().ForEach(async server =>
            {
                UriBuilder uriBuilder = new()
                {
                    Host = server.IpAddress.ToString(),
                    Port = 80,
                    Path = $"/api/WaitingSessions/Any?playerCount={playerSlots}"
                };

                HttpResponseMessage response = await HttpClientProvider.Shared.GetAsync(uriBuilder.Uri);

                if (!response.IsSuccessStatusCode)
                    return;

                object? anyWaitingSessions = await response.Content.ReadFromJsonAsync(typeof(bool));

                if (anyWaitingSessions is bool b && b == true)
                    result.Add(server);
            });

            return result;
        }

        [HttpGet]
        public async Task<IActionResult> GetServerWithFreePlayerSlots(int freePlayerSlots)
        {
            var serversWithEnoughFreePlayerSlots = GetServersWithEnoughFreePlayerSlots(_context.Servers, freePlayerSlots);

            if (!serversWithEnoughFreePlayerSlots.Any())
                return Ok(null);

            // EXECUTIONTIME: about 4 seconds

            var serversWithWaitingSessions = GetServersWithWaitingSessions(serversWithEnoughFreePlayerSlots, freePlayerSlots);

            if (serversWithWaitingSessions.Any())
            {
                int randomIndex = Random.Shared.Next(0, serversWithWaitingSessions.Count());
                Server server = serversWithWaitingSessions.ElementAt(randomIndex);
                return Ok(server);
            }
            else
            {
                foreach (Server server in serversWithEnoughFreePlayerSlots)
                {
                    UriBuilder uriBuilder = new()
                    {
                        Host = server.IpAddress.ToString(),
                        Port = 80,
                        Path = $"/api/WaitingSessions/Create?playerCount={freePlayerSlots}"
                    };

                    JsonContent jsonContent = JsonContent.Create(freePlayerSlots, typeof(int));

                    HttpResponseMessage response = await HttpClientProvider.Shared.PostAsync(uriBuilder.Uri, jsonContent);

                    if (response.IsSuccessStatusCode)
                        return Ok(server);
                }
            }

            return Ok(null);
        }
    }
}
