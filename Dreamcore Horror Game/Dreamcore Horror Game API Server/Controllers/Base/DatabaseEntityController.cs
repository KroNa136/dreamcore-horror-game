using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace DreamcoreHorrorGameApiServer.Controllers.Base;

[ApiController]
public abstract class DatabaseEntityController<TEntity> : ControllerBase
    where TEntity : class, IDatabaseEntity
{
    protected string AuthorizationToken => HttpContext.Request.Headers[HeaderNames.Authorization]
        .ToString().Replace("Bearer ", string.Empty);

    protected bool InvalidModelState => !ModelState.IsValid;

    protected readonly DreamcoreHorrorGameContext _context;

    protected readonly List<string> _requiredHeaders;

    //private readonly Func<DreamcoreHorrorGameContext, DatabaseController<TEntity>> _derivedClassConstructor;

    public DatabaseEntityController(DreamcoreHorrorGameContext context)
    {
        _context = context;
        _requiredHeaders = new List<string>();
    }

    // POST note:
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public virtual DatabaseEntityController<TEntity> RequireHeaders(params string[] headers)
    {
        SetRequiredHeaders(headers);
        return this;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> DoAsync<T>(T arg, Func<T, Task<IActionResult>> action)
    {
        if (NoHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.HeaderMissing);

        return await action(arg);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> DoAsync<T1, T2>(T1 arg1, T2 arg2, Func<T1, T2, Task<IActionResult>> action)
    {
        if (NoHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.HeaderMissing);

        return await action(arg1, arg2);
    }

    public abstract Task<IActionResult> GetAll();
    public abstract Task<IActionResult> Get(Guid? id);
    public abstract Task<IActionResult> Create(TEntity entity);
    public abstract Task<IActionResult> Edit(Guid? id, TEntity entity);
    public abstract Task<IActionResult> Delete(Guid? id);

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAllAsync()
    {
        if (NoHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.HeaderMissing);

        return Ok(await _context.Set<TEntity>().ToListAsync());
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAsync(Func<TEntity, bool> predicate)
    {
        if (NoHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.HeaderMissing);

        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(e => predicate(e));

        return entity is not null
            ? Ok(entity)
            : NotFound();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public virtual async Task<IActionResult> CreateAsync(TEntity entity)
    {
        if (NoHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.HeaderMissing);

        if (InvalidModelState)
            return ValidationProblem();

        entity.Id = Guid.NewGuid();

        _context.Add(entity);
        await _context.SaveChangesAsync();
        return Ok(entity);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> EditAsync(Guid? id, TEntity entity)
    {
        if (NoHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        if (id != entity.Id)
            return BadRequest(ErrorMessages.IdMismatch);

        if (InvalidModelState)
            return ValidationProblem();

        try
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await EntityExistsAsync(entity.Id) == false)
                return NotFound();
            else
                throw;
        }

        return Ok(entity);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> DeleteAsync(Guid? id)
    {
        if (NoHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.HeaderMissing);

        if (id is null)
            return NotFound();

        var entity = await _context.Set<TEntity>().FindAsync(id);

        if (entity is null)
            return NotFound();

        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return Ok();
    }

    protected async Task<bool> EntityExistsAsync(Guid id)
        => await _context.Set<TEntity>().AnyAsync(entity => entity.Id == id);

    protected void SetRequiredHeaders(IEnumerable<string> fromCollection)
    {
        _requiredHeaders.Clear();
        _requiredHeaders.AddRange(fromCollection);
    }

    protected bool NoHeader(IEnumerable<string> headers)
    {
        if (headers.IsEmpty())
            return false;

        if (HttpContext is null || HttpContext.Request is null || HttpContext.Request.Headers is null)
            return true;

        foreach (var header in headers)
        {
            if (HttpContext.Request.Headers.ContainsKey(header))
                return false;
        }

        return true;
    }
}
