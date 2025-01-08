using DreamcoreHorrorGameStatisticsServer.ConstantValues;
using DreamcoreHorrorGameStatisticsServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameStatisticsServer.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class ControllersController
(
    DreamcoreHorrorGameStatisticsContext context,
    ILogger<ControllersController> logger
)
: ControllerBase
{
    private readonly DreamcoreHorrorGameStatisticsContext _context = context;
    private readonly ILogger<ControllersController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GetAll was called for Controller.");

        var controllers = await _context.Controllers
            .OrderBy(entityType => entityType.Name)
            .ToListAsync();

        return Ok(controllers);
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid? id)
    {
        _logger.LogInformation("Get was called for Controller.");

        if (id is null)
            return NotFound();

        var controller = await _context.Controllers
            .FirstOrDefaultAsync(controller => controller.Id == id);

        if (controller is null)
            return NotFound();

        return Ok(controller);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(
        nameof(Models.Controller.Id),
        nameof(Models.Controller.Name)
    )] Models.Controller controller)
    {
        _logger.LogInformation("Create was called for Controller.");

        if (!ModelState.IsValid)
            return ValidationProblem();

        if (await ControllerExistsAsync(controller.Name))
            return ValidationProblem();

        controller.Id = Guid.NewGuid();

        _context.Controllers.Add(controller);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError
            (
                eventId: new EventId("Create".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while creating Controller with id = {id}.", controller.Id
            );

            return Conflict(ErrorMessages.CreateConflict);
        }

        return Ok(controller);
    }

    [HttpPut]
    public async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Models.Controller.Id),
        nameof(Models.Controller.Name)
    )] Models.Controller controller)
    {
        _logger.LogInformation("Edit was called for Controller.");

        if (id is null)
            return NotFound();

        if (id != controller.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (!ModelState.IsValid)
            return ValidationProblem();

        if (await ControllerExistsAsync(controller.Name))
            return ValidationProblem();

        _context.Controllers.Update(controller);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (await ControllerExistsAsync(controller.Id) is false)
            {
                return NotFound();
            }
            else
            {
                _logger.LogError
                (
                    eventId: new EventId("Edit".GetHashCode() + ex.GetType().GetHashCode()),
                    message: "Database conflict occured while editing Controller with id = {id}.", controller.Id
                );

                return Conflict(ErrorMessages.EditConflict);
            }
        }

        return Ok(controller);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        _logger.LogInformation("Delete was called for Controller.");

        if (id is null)
            return NotFound();

        var controller = await _context.Controllers
            .FirstOrDefaultAsync(controller => controller.Id == id);

        if (controller is null)
            return NotFound();

        _context.Controllers.Remove(controller);

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

            if (await ControllerExistsAsync(controller.Id) is false)
                // The most possible scenario for this case is when two users try to delete the same entity
                // one right after the other, so it makes sense to return Ok here because the user's goal
                // is accomplished - the entity doesn't exist anymore.
                return Ok();

            _logger.LogError
            (
                eventId: new EventId("Delete".GetHashCode() + ex.GetType().GetHashCode()),
                message: "Database conflict occured while deleting Controller with id = {id}.", controller.Id
            );

            return Conflict(ErrorMessages.DeleteConflict);
        }

        return Ok();
    }

    private async Task<bool> ControllerExistsAsync(Guid id)
        => await _context.Controllers.AnyAsync(controller => controller.Id == id);

    private async Task<bool> ControllerExistsAsync(string name)
        => await _context.Controllers.AnyAsync(controller => controller.Name.Equals(name));
}
