using DreamcoreHorrorGameStatisticsServer.ConstantValues;
using DreamcoreHorrorGameStatisticsServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DreamcoreHorrorGameStatisticsServer.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RequestsController
(
    DreamcoreHorrorGameStatisticsContext context,
    ILogger<RequestsController> logger
)
: ControllerBase
{
    private readonly DreamcoreHorrorGameStatisticsContext _context = context;
    private readonly ILogger<RequestsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GetAll was called for Request.");

        var requests = await _context.Requests
            .Include(request => request.Sender)
            .Include(request => request.Controller)
            .Include(request => request.Method)
            .OrderByDescending(request => request.ReceptionTimestamp)
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReceivedCloseToTimeMoment(DateTime moment, int secondsThreshold)
    {
        _logger.LogInformation("GetAllReceivedCloseToTimeMoment was called for Request");

        DateTime minReceptionDateTime = moment - TimeSpan.FromSeconds(secondsThreshold);
        DateTime maxReceptionDateTime = moment + TimeSpan.FromSeconds(secondsThreshold);

        var requests = await _context.Requests
            .Include(request => request.Sender)
            .Include(request => request.Controller)
            .Include(request => request.Method)
            .Where(request => request.ReceptionTimestamp >= minReceptionDateTime && request.ReceptionTimestamp <= maxReceptionDateTime)
            .OrderByDescending(request => request.ReceptionTimestamp)
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReceivedFromSenderCloseToTimeMoment(string senderName, DateTime moment, int secondsThreshold)
    {
        _logger.LogInformation("GetAllReceivedFromSenderCloseToTimeMoment was called for Request");

        List<Request> emptyList = [];

        var sender = await _context.Senders
            .FirstOrDefaultAsync(sender => sender.Name.Equals(senderName));

        if (sender is null)
            return Ok(emptyList);

        DateTime minReceptionDateTime = moment - TimeSpan.FromSeconds(secondsThreshold);
        DateTime maxReceptionDateTime = moment + TimeSpan.FromSeconds(secondsThreshold);

        var requests = await _context.Requests
            .Include(request => request.Sender)
            .Include(request => request.Controller)
            .Include(request => request.Method)
            .Where(request => request.ReceptionTimestamp >= minReceptionDateTime && request.ReceptionTimestamp <= maxReceptionDateTime)
            .OrderByDescending(request => request.ReceptionTimestamp)
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAddressedToControllerReceivedCloseToTimeMoment(string controllerName, DateTime moment, int secondsThreshold)
    {
        _logger.LogInformation("GetAllAddressedToControllerReceivedCloseToTimeMoment was called for Request");

        List<Request> emptyList = [];

        var controller = await _context.Controllers
            .FirstOrDefaultAsync(controller => controller.Name.Equals(controllerName));

        if (controller is null)
            return Ok(emptyList);

        DateTime minReceptionDateTime = moment - TimeSpan.FromSeconds(secondsThreshold);
        DateTime maxReceptionDateTime = moment + TimeSpan.FromSeconds(secondsThreshold);

        var requests = await _context.Requests
            .Include(request => request.Sender)
            .Include(request => request.Controller)
            .Include(request => request.Method)
            .Where(request => request.ControllerId == controller.Id &&
                request.ReceptionTimestamp >= minReceptionDateTime && request.ReceptionTimestamp <= maxReceptionDateTime)
            .OrderByDescending(request => request.ReceptionTimestamp)
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAddressedToControllerAndMethodReceivedCloseToTimeMoment(string controllerName, string methodName, DateTime moment, int secondsThreshold)
    {
        _logger.LogInformation("GetAllAddressedToControllerAndMethodReceivedCloseToTimeMoment was called for Request");

        List<Request> emptyList = [];

        var controller = await _context.Controllers
            .FirstOrDefaultAsync(controller => controller.Name.Equals(controllerName));

        var method = await _context.Methods
            .FirstOrDefaultAsync(method => method.Name.Equals(methodName));

        if (controller is null || method is null)
            return Ok(emptyList);

        DateTime minReceptionDateTime = moment - TimeSpan.FromSeconds(secondsThreshold);
        DateTime maxReceptionDateTime = moment + TimeSpan.FromSeconds(secondsThreshold);

        var requests = await _context.Requests
            .Include(request => request.Sender)
            .Include(request => request.Controller)
            .Include(request => request.Method)
            .Where(request => request.ControllerId == controller.Id && request.MethodId == method.Id &&
                request.ReceptionTimestamp >= minReceptionDateTime && request.ReceptionTimestamp <= maxReceptionDateTime)
            .OrderByDescending(request => request.ReceptionTimestamp)
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid? id)
    {
        _logger.LogInformation("Get was called for Request.");

        if (id is null)
            return NotFound();

        var request = await _context.Requests
            .Include(request => request.Sender)
            .Include(request => request.Controller)
            .Include(request => request.Method)
            .FirstOrDefaultAsync(request => request.Id == id);

        if (request is null)
            return NotFound();

        return Ok(request);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(
        nameof(Models.Request.Id),
        nameof(Models.Request.ReceptionTimestamp),
        nameof(Models.Request.SenderId),
        nameof(Models.Request.ControllerId),
        nameof(Models.Request.MethodId)
    )] Models.Request request)
    {
        _logger.LogInformation("Create was called for Request.");

        if (!ModelState.IsValid)
            return ValidationProblem();

        request.Id = Guid.NewGuid();

        try
        {
            request = await SetRelationsFromForeignKeys(request);
        }
        catch (InvalidConstraintException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("Edit".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Failed to set relations from foreign keys while creating Request with id = {id}.", request.Id
            );

            return UnprocessableEntity(ErrorMessages.RelatedEntityDoesNotExist);
        }

        _context.Requests.Add(request);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("Create".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while creating Request with id = {id}.", request.Id
            );

            return Conflict(ErrorMessages.CreateConflict);
        }

        return Ok(request);
    }

    [HttpPut]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Models.Request.Id),
        nameof(Models.Request.ReceptionTimestamp),
        nameof(Models.Request.SenderId),
        nameof(Models.Request.ControllerId),
        nameof(Models.Request.MethodId)
    )] Models.Request request)
    {
        _logger.LogInformation("Edit was called for Request.");

        if (id is null)
            return NotFound();

        if (id != request.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (!ModelState.IsValid)
            return ValidationProblem();

        try
        {
            request = await SetRelationsFromForeignKeys(request);
        }
        catch (InvalidConstraintException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("Edit".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Failed to set relations from foreign keys while editing Request with id = {id}.", request.Id
            );

            return UnprocessableEntity(ErrorMessages.RelatedEntityDoesNotExist);
        }

        _context.Requests.Update(request);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (await RequestExistsAsync(request.Id) is false)
            {
                return NotFound();
            }
            else
            {
                _logger.LogError
                (
                    eventId: new EventId("Edit".GetHashCode() + ex.GetType().GetHashCode()),
                    message: "Database conflict occured while editing Request with id = {id}.", request.Id
                );

                return Conflict(ErrorMessages.EditConflict);
            }
        }

        return Ok(request);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        _logger.LogInformation("Delete was called for Request.");

        if (id is null)
            return NotFound();

        var request = await _context.Requests
            .FirstOrDefaultAsync(request => request.Id == id);

        if (request is null)
            return NotFound();

        _context.Requests.Remove(request);

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

            if (await RequestExistsAsync(request.Id) is false)
                // The most possible scenario for this case is when two users try to delete the same entity
                // one right after the other, so it makes sense to return Ok here because the user's goal
                // is accomplished - the entity doesn't exist anymore.
                return Ok();

            return Conflict(ErrorMessages.DeleteConflict);
        }

        return Ok();
    }

    private async Task<bool> RequestExistsAsync(Guid id)
        => await _context.Requests.AnyAsync(request => request.Id == id);

    private async Task<Request> SetRelationsFromForeignKeys(Request request)
    {
        var sender = await _context.Senders
            .FindAsync(request.SenderId);

        var controller = await _context.Controllers
            .FindAsync(request.ControllerId);

        var method = await _context.Methods
            .FindAsync(request.MethodId);

        if (sender is null || controller is null || method is null)
            throw new InvalidConstraintException();

        request.Sender = sender;
        request.SenderId = Guid.Empty;

        request.Controller = controller;
        request.ControllerId = Guid.Empty;

        request.Method = method;
        request.MethodId = Guid.Empty;

        return request;
    }
}
