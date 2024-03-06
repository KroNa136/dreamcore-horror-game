using Dreamcore_Horror_Game_API_Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Dreamcore_Horror_Game_API_Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ServersController : ControllerBase
    {
        private readonly DreamcoreHorrorGameContext _context;

        public ServersController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetServer(int availablePlayerSlots)
        {
            List<Server> suitableServers = new();

            await _context.Servers.Where(s => s.IsOnline).ForEachAsync(s =>
            {
                var activePlayerSessions = _context.PlayerSessions.Where
                (
                    ps => ps.GameSession.Server != null &&
                        ps.GameSession.Server.Id == s.Id &&
                        ps.EndTimestamp == null
                );

                if (activePlayerSessions.Count() + availablePlayerSlots <= s.PlayerCapacity)
                    suitableServers.Add(s);
            });

            if (suitableServers.Any() == false)
                return NotFound();

            List<Server> serversWithWaitingSessions = new();

            suitableServers.ForEach(async server =>
            {
                UriBuilder uriBuilder = new()
                {
                    Host = server.IpAddress.ToString(),
                    Port = 80,
                    Path = $"/api/WaitingSessionsController/AnyWaitingSessions?playerCount={availablePlayerSlots}"
                };

                var response = await HttpClientProvider.Shared.GetAsync(uriBuilder.Uri);

                // TODO: if response headers contain an error code, return
                if (response.Headers.Contains("*some error*"))
                    return;

                var anyWaitingSessions = await response.Content.ReadFromJsonAsync(typeof(bool));

                if (anyWaitingSessions is bool b && b == true)
                    serversWithWaitingSessions.Add(server);
            });

            if (serversWithWaitingSessions.Any())
            {
                int randomIndex = Random.Shared.Next(0, serversWithWaitingSessions.Count);
                var server = serversWithWaitingSessions[randomIndex];

                return Ok(server);
            }
            else
            {
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    int randomIndex = Random.Shared.Next(0, suitableServers.Count);
                    var server = suitableServers[randomIndex];

                    UriBuilder uriBuilder = new()
                    {
                        Host = server.IpAddress.ToString(),
                        Port = 80,
                        Path = $"/api/WaitingSessionsController/CreateWaitingSession?playerCount={availablePlayerSlots}"
                    };

                    StringContent jsonContent = new
                    (
                        JsonSerializer.Serialize(new { playerCount = availablePlayerSlots }),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await HttpClientProvider.Shared.PostAsync(uriBuilder.Uri, jsonContent);

                    // TODO: if response headers contain an error code, continue
                    if (response.Headers.Contains("*some error*"))
                        continue;

                    return Ok(server);
                }
            }

            return NotFound();
        }
    }
}
