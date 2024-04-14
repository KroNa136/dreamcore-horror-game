using DreamcoreHorrorGameApiServer.ConstantValues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DreamcoreHorrorGameApiServer.Controllers.Database;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class DatabaseController : ControllerBase
{
    protected readonly DreamcoreHorrorGameContext _context;

    public DatabaseController(DreamcoreHorrorGameContext context)
        => _context = context;

    // POST note:
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    protected string AuthorizationToken
        => HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);

    protected bool NoHeader(params string[] headers)
    {
        foreach (var header in headers)
        {
            if (HttpContext.Request.Headers.ContainsKey(header))
                return false;
        }
        return true;
    }

    protected bool InvalidModelState => !ModelState.IsValid;
}
