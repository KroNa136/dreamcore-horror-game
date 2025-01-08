using DreamcoreHorrorGameStatisticsServer.ConstantValues;
using DreamcoreHorrorGameStatisticsServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DreamcoreHorrorGameStatisticsServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SendersController
(
    DreamcoreHorrorGameStatisticsContext context,
    ILogger<SendersController> logger
)
: ControllerBase
{
    private readonly DreamcoreHorrorGameStatisticsContext _context = context;
    private readonly ILogger<SendersController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GetAll was called for Sender.");

        var senders = await _context.Senders
            .OrderBy(sender => sender.Name)
            .ToListAsync();

        return Ok(senders);
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid? id)
    {
        _logger.LogInformation("Get was called for Sender.");

        if (id is null)
            return NotFound();

        var sender = await _context.Senders
            .FirstOrDefaultAsync(sender => sender.Id == id);

        if (sender is null)
            return NotFound();

        return Ok(sender);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(
        nameof(Sender.Id),
        nameof(Sender.Name)
    )] Sender sender)
    {
        _logger.LogInformation("Create was called for Sender.");

        if (!ModelState.IsValid)
            return ValidationProblem();

        if (await SenderExistsAsync(sender.Name))
            return ValidationProblem();

        sender.Id = Guid.NewGuid();

        _context.Senders.Add(sender);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("Create".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while creating Sender with id = {id}.", sender.Id
            );

            return Conflict(ErrorMessages.CreateConflict);
        }

        return Ok(sender);
    }

    [HttpPut]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Sender.Id),
        nameof(Sender.Name)
    )] Sender sender)
    {
        _logger.LogInformation("Edit was called for Sender.");

        if (id is null)
            return NotFound();

        if (id != sender.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (!ModelState.IsValid)
            return ValidationProblem();

        if (await SenderExistsAsync(sender.Name))
            return ValidationProblem();

        _context.Senders.Update(sender);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (await SenderExistsAsync(sender.Id) is false)
            {
                return NotFound();
            }
            else
            {
                _logger.LogError
                (
                    eventId: new EventId("Edit".GetHashCode() + ex.GetType().GetHashCode()),
                    message: "Database conflict occured while editing Sender with id = {id}.", sender.Id
                );

                return Conflict(ErrorMessages.EditConflict);
            }
        }

        return Ok(sender);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        _logger.LogInformation("Delete was called for Sender.");

        if (id is null)
            return NotFound();

        var sender = await _context.Senders
            .FirstOrDefaultAsync(sender => sender.Id == id);

        if (sender is null)
            return NotFound();

        _context.Senders.Remove(sender);

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

            if (await SenderExistsAsync(sender.Id) is false)
                // The most possible scenario for this case is when two users try to delete the same entity
                // one right after the other, so it makes sense to return Ok here because the user's goal
                // is accomplished - the entity doesn't exist anymore.
                return Ok();

            _logger.LogError
            (
                eventId: new EventId("Delete".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while deleting Sender with id = {id}.", sender.Id
            );

            return Conflict(ErrorMessages.DeleteConflict);
        }

        return Ok();
    }

    private async Task<bool> SenderExistsAsync(Guid id)
        => await _context.Senders.AnyAsync(sender => sender.Id == id);

    private async Task<bool> SenderExistsAsync(string name)
        => await _context.Senders.AnyAsync(sender => sender.Name.Equals(name));
}
