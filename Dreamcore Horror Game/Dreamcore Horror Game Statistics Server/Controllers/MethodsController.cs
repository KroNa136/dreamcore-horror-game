using DreamcoreHorrorGameStatisticsServer.ConstantValues;
using DreamcoreHorrorGameStatisticsServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameStatisticsServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MethodsController
(
    DreamcoreHorrorGameStatisticsContext context,
    ILogger<MethodsController> logger
)
: ControllerBase
{
    private readonly DreamcoreHorrorGameStatisticsContext _context = context;
    private readonly ILogger<MethodsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GetAll was called for Method.");

        var methods = await _context.Methods
            .OrderBy(method => method.Name)
            .ToListAsync();

        return Ok(methods);
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid? id)
    {
        _logger.LogInformation("Get was called for Method.");

        if (id is null)
            return NotFound();

        var method = await _context.Methods
            .FirstOrDefaultAsync(method => method.Id == id);

        if (method is null)
            return NotFound();

        return Ok(method);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(
        nameof(Method.Id),
        nameof(Method.Name)
    )] Method method)
    {
        _logger.LogInformation("Create was called for Method.");

        if (!ModelState.IsValid)
            return ValidationProblem();

        if (await MethodExistsAsync(method.Name))
            return ValidationProblem();

        method.Id = Guid.NewGuid();

        _context.Methods.Add(method);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("Create".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while creating Method with id = {id}.", method.Id
            );

            return Conflict(ErrorMessages.CreateConflict);
        }

        return Ok(method);
    }

    [HttpPut]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Method.Id),
        nameof(Method.Name)
    )] Method method)
    {
        _logger.LogInformation("Edit was called for Method.");

        if (id is null)
            return NotFound();

        if (id != method.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (!ModelState.IsValid)
            return ValidationProblem();

        if (await MethodExistsAsync(method.Name))
            return ValidationProblem();

        _context.Methods.Update(method);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (await MethodExistsAsync(method.Id) is false)
            {
                return NotFound();
            }
            else
            {
                _logger.LogError
                (
                    eventId: new EventId("Edit".GetHashCode() + ex.GetType().GetHashCode()),
                    message: "Database conflict occured while editing Method with id = {id}.", method.Id
                );

                return Conflict(ErrorMessages.EditConflict);
            }
        }

        return Ok(method);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        _logger.LogInformation("Delete was called for Method.");

        if (id is null)
            return NotFound();

        var method = await _context.Methods
            .FirstOrDefaultAsync(method => method.Id == id);

        if (method is null)
            return NotFound();

        _context.Methods.Remove(method);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex is not DbUpdateConcurrencyException)
                // TODO: think of a way to check if this is a constraint violation or some other database failure
                //       because Npgsql unfortunately does not provide any clarification in its errors :(
                return UnprocessableEntity(ErrorMessages.DeleteConstraintViolation);

            if (await MethodExistsAsync(method.Id) is false)
                // The most possible scenario for this case is when two users try to delete the same entity
                // one right after the other, so it makes sense to return Ok here because the user's goal
                // is accomplished - the entity doesn't exist anymore.
                return Ok();

            _logger.LogError
            (
                eventId: new EventId("Delete".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while deleting Method with id = {id}.", method.Id
            );

            return Conflict(ErrorMessages.DeleteConflict);
        }

        return Ok();
    }

    private async Task<bool> MethodExistsAsync(Guid id)
        => await _context.Methods.AnyAsync(method => method.Id == id);

    private async Task<bool> MethodExistsAsync(string name)
        => await _context.Methods.AnyAsync(method => method.Name.Equals(name));
}
