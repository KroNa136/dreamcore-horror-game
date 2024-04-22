using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Data;
using System.Reflection;

namespace DreamcoreHorrorGameApiServer.Controllers.Base;

[ApiController]
public abstract class DatabaseEntityController<TEntity> : ControllerBase
    where TEntity : class, IDatabaseEntity
{
    protected string AuthorizationToken => HttpContext.Request.Headers[HeaderNames.Authorization]
        .ToString().Replace("Bearer ", string.Empty);

    protected bool InvalidModelState => ModelState.IsValid is false;

    protected readonly DreamcoreHorrorGameContext _context;
    protected readonly IPropertyPredicateValidator _propertyPredicateValidator;

    protected readonly List<string> _requiredHeaders;

    protected readonly System.Linq.Expressions.Expression<Func<TEntity, object?>> _orderBySelector;

    // This function may contain logic that manually clears unnecessary references in IQueryables to save traffic
    // and data loading times. DO NOT SAVE any data returned by this function to the database!
    protected readonly Func<DreamcoreHorrorGameContext, Task<IQueryable<TEntity>>> _getAllWithFirstLevelRelations;

    protected readonly Func<DreamcoreHorrorGameContext, TEntity, Task>? _setRelationsFromForeignKeys;

    //private readonly Func<DreamcoreHorrorGameContext, DatabaseController<TEntity>> _derivedClassConstructor;

    public DatabaseEntityController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator,
        System.Linq.Expressions.Expression<Func<TEntity, object?>> orderBySelector,
        Func<DreamcoreHorrorGameContext, Task<IQueryable<TEntity>>> getAllWithFirstLevelRelationsFunction,
        Func<DreamcoreHorrorGameContext, TEntity, Task>? setRelationsFromForeignKeysFunction
    )
    {
        _context = context;
        _propertyPredicateValidator = propertyPredicateValidator;

        _requiredHeaders = new List<string>();

        _orderBySelector = orderBySelector;
        _getAllWithFirstLevelRelations = getAllWithFirstLevelRelationsFunction;
        _setRelationsFromForeignKeys = setRelationsFromForeignKeysFunction;
    }

    // POST note:
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    public abstract Task<IActionResult> GetAll(int page, int showBy);
    public abstract Task<IActionResult> GetAllWithRelations(int page, int showBy);
    public abstract Task<IActionResult> Get(Guid? id);
    public abstract Task<IActionResult> GetWithRelations(Guid? id);
    public abstract Task<IActionResult> GetWhere(PropertyPredicate[] propertyPredicateCollection, int page, int showBy);
    public abstract Task<IActionResult> Create(TEntity entity);
    public abstract Task<IActionResult> Edit(Guid? id, TEntity entity);
    public abstract Task<IActionResult> Delete(Guid? id);

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public virtual DatabaseEntityController<TEntity> RequireHeaders(params string[] headers)
    {
        SetRequiredHeaders(headers);
        return this;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAllEntitiesAsync(int page, int showBy)
        => await ValidateHeadersAndHandleErrorsAsync(async () =>
        {
            var entities = await _context.Set<TEntity>()
                .OrderBy(_orderBySelector)
                .Paginate(page, showBy)
                .ToListAsync();

            return Ok(entities);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAllEntitiesWithRelationsAsync(int page, int showBy)
        => await ValidateHeadersAndHandleErrorsAsync(async () =>
        {
            var entities = await (await _getAllWithFirstLevelRelations(_context))
                .OrderBy(_orderBySelector)
                .Paginate(page, showBy)
                .ToListAsync();

            return Ok(entities);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetEntityAsync(Guid? id)
        => await ValidateHeadersAndHandleErrorsAsync(id, async id =>
        {
            if (id is null)
                return NotFound();

            var entity = await _context.Set<TEntity>()
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (entity is null)
                return NotFound();

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public IActionResult GetEntity(Func<TEntity, bool> predicate)
        => ValidateHeadersAndHandleErrors(predicate, predicate =>
        {
            var entity = _context.Set<TEntity>()
                .AsQueryable()
                .AsForceParallel()
                .FirstOrDefault(entity => predicate(entity));

            if (entity is null)
                return NotFound();

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetEntityWithRelationsAsync(Guid? id)
        => await ValidateHeadersAndHandleErrorsAsync(id, async id =>
        {
            if (id is null)
                return NotFound();

            var entity = await (await _getAllWithFirstLevelRelations(_context))
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (entity is null)
                return NotFound();

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetEntityWithRelationsAsync(Func<TEntity, bool> predicate)
        => await ValidateHeadersAndHandleErrorsAsync(predicate, async predicate =>
        {
            var entity = (await _getAllWithFirstLevelRelations(_context))
                .AsForceParallel()
                .FirstOrDefault(entity => predicate(entity));

            if (entity is null)
                return NotFound();

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetEntitiesWhereAsync(IEnumerable<PropertyPredicate> propertyPredicateCollection, int page, int showBy)
        => await ValidateHeadersAndHandleErrorsAsync(propertyPredicateCollection, async propertyPredicateCollection =>
        {
            var entities = (await _getAllWithFirstLevelRelations(_context))
                .AsForceParallel();

            bool validationResult = _propertyPredicateValidator.ValidatePropertyPredicateCollection
            (
                propertyPredicateCollection: propertyPredicateCollection,
                predicateTargetType: typeof(TEntity),
                errorMessage: out string errorMessage
            );

            if (validationResult is false)
                return BadRequest(errorMessage);

            foreach (PropertyPredicate predicate in propertyPredicateCollection)
            {
                var property = GetProperty(typeof(TEntity), predicate.Property)!;
                var _operator = _propertyPredicateValidator.GetOperator(predicate.Operator);

                entities = _operator switch
                {
                    UnaryOperator unaryOperator => FilterEntities(entities, property, unaryOperator),
                    BinaryOperator binaryOperator => FilterEntities(entities, property, binaryOperator, predicate),
                    TernaryOperator ternaryOperator => FilterEntities(entities, property, ternaryOperator, predicate),
                    _ => throw new InvalidOperationException()
                };
            }

            var orderedAndPaginatedEntities = entities
                .AsEnumerable()
                .OrderBy(_orderBySelector.Compile())
                .Paginate(page, showBy);

            return Ok(orderedAndPaginatedEntities);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public virtual async Task<IActionResult> CreateEntityAsync(TEntity entity)
        => await ValidateHeadersAndHandleErrorsAsync(entity, async entity =>
        {
            if (InvalidModelState)
                return ValidationProblem();

            entity.Id = Guid.NewGuid();

            if (_setRelationsFromForeignKeys is not null)
                await _setRelationsFromForeignKeys(_context, entity);

            _context.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // TODO: log error
                return Conflict(ErrorMessages.CreateConflict);
            }

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> EditEntityAsync(Guid? id, TEntity entity)
        => await ValidateHeadersAndHandleErrorsAsync(id, entity, async (id, entity) =>
        {
            if (id is null)
                return NotFound();

            if (id != entity.Id)
                return BadRequest(ErrorMessages.IdMismatch);

            if (InvalidModelState)
                return ValidationProblem();

            if (_setRelationsFromForeignKeys is not null)
                await _setRelationsFromForeignKeys(_context, entity);

            _context.Update(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await EntityExistsAsync(entity.Id) is false)
                {
                    return NotFound();
                }
                else
                {
                    // TODO: log error
                    return Conflict(ErrorMessages.EditConflict);
                }
            }

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> DeleteEntityAsync(Guid? id)
        => await ValidateHeadersAndHandleErrorsAsync(id, async id =>
        {
            if (id is null)
                return NotFound();

            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity is null)
                return NotFound();

            _context.Remove(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex is DbUpdateConcurrencyException && await EntityExistsAsync(entity.Id) is false)
                {
                    // The most possible scenario for this case is when two users try to delete
                    // the same entity one right after the other, so it makes sense to return Ok here
                    // because the user's goal is accomplished - the entity doesn't exist anymore.
                    return Ok();
                }
                else
                {
                    return UnprocessableEntity(ErrorMessages.DeleteConstraintViolation);
                }
            }

            return Ok();
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> function)
        => await ValidateHeadersAndHandleErrorsAsync(function());

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> ExecuteAsync<T>(T arg, Func<T, Task<IActionResult>> function)
        => await ValidateHeadersAndHandleErrorsAsync(arg, async arg => await function(arg));

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> ExecuteAsync<T1, T2>(T1 arg1, T2 arg2, Func<T1, T2, Task<IActionResult>> function)
        => await ValidateHeadersAndHandleErrorsAsync(arg1, arg2, async (arg1, arg2) => await function(arg1, arg2));

    protected void SetRequiredHeaders(IEnumerable<string> fromCollection)
    {
        _requiredHeaders.Clear();
        _requiredHeaders.AddRange(fromCollection);
    }

    protected IActionResult ValidateHeadersAndHandleErrors<T>(T arg, Func<T, IActionResult> function)
        => ValidateHeadersAndHandleErrors(() => function(arg));

    protected IActionResult ValidateHeadersAndHandleErrors<T1, T2>(T1 arg1, T2 arg2, Func<T1, T2, IActionResult> function)
        => ValidateHeadersAndHandleErrors(() => function(arg1, arg2));

    protected IActionResult ValidateHeadersAndHandleErrors(Func<IActionResult> function)
    {
        if (NoValidHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.CorsHeaderMissing);

        try
        {
            return function();
        }
        catch (InvalidConstraintException)
        {
            return UnprocessableEntity(ErrorMessages.RelatedEntityDoesNotExist);
        }
        catch (DbUpdateException)
        {
            // TODO: log error
            return this.InternalServerError();
        }
        catch (InvalidOperationException)
        {
            // TODO: log error
            return this.InternalServerError();
        }
        catch (Exception)
        {
            // TODO: log error
            return this.InternalServerError();
        }
    }

    protected async Task<IActionResult> ValidateHeadersAndHandleErrorsAsync(Func<Task<IActionResult>> function)
        => await ValidateHeadersAndHandleErrorsAsync(function());

    protected async Task<IActionResult> ValidateHeadersAndHandleErrorsAsync<T>(T arg, Func<T, Task<IActionResult>> function)
        => await ValidateHeadersAndHandleErrorsAsync(function(arg));

    protected async Task<IActionResult> ValidateHeadersAndHandleErrorsAsync<T1, T2>(T1 arg1, T2 arg2, Func<T1, T2, Task<IActionResult>> function)
        => await ValidateHeadersAndHandleErrorsAsync(function(arg1, arg2));

    protected async Task<IActionResult> ValidateHeadersAndHandleErrorsAsync(Task<IActionResult> task)
    {
        if (NoValidHeader(_requiredHeaders))
            return this.Forbidden(ErrorMessages.CorsHeaderMissing);

        try
        {
            return await task;
        }
        catch (InvalidConstraintException)
        {
            return UnprocessableEntity(ErrorMessages.RelatedEntityDoesNotExist);
        }
        catch (DbUpdateException)
        {
            // TODO: log error
            return this.InternalServerError();
        }
        catch (InvalidOperationException)
        {
            // TODO: log error
            return this.InternalServerError();
        }
        catch (Exception)
        {
            // TODO: log error
            return this.InternalServerError();
        }
    }

    protected bool NoValidHeader(IEnumerable<string> headers)
    {
        if (headers.IsEmpty())
            return false;

        if (HttpContext is null || HttpContext.Request is null || HttpContext.Request.Headers is null)
            return true;

        var requestHeaders = HttpContext.Request.Headers;

        foreach (var header in headers)
        {
            if (requestHeaders.ContainsKey(header) && requestHeaders[header].Equals(CorsHeaders.RequiredHeaderValue))
                return false;
        }

        return true;
    }

    protected static PropertyInfo? GetProperty(Type type, string name)
        => type.GetProperties()
            .FirstOrDefault(property => property.Name.ToLower().Equals(name.ToLower().Replace("_", string.Empty)));

    protected static ParallelQuery<TEntity> FilterEntities
    (
        ParallelQuery<TEntity> entities,
        PropertyInfo property,
        UnaryOperator unaryOperator
    )
    {
        return entities.Where(entity => unaryOperator.Operation(property.GetValue(entity)));
    }

    protected ParallelQuery<TEntity> FilterEntities
    (
        ParallelQuery<TEntity> entities,
        PropertyInfo property,
        BinaryOperator binaryOperator,
        PropertyPredicate predicate
    )
    {
        var binaryPredicate = (predicate as BinaryPropertyPredicate)!;

        object? value = _propertyPredicateValidator
            .GetValidValue(binaryPredicate.Value!, property.PropertyType, binaryOperator.IgnoreTypes);

        return entities.Where(entity => binaryOperator.Operation(property.GetValue(entity), value));
    }

    protected ParallelQuery<TEntity> FilterEntities
    (
        ParallelQuery<TEntity> entities,
        PropertyInfo property,
        TernaryOperator ternaryOperator,
        PropertyPredicate predicate
    )
    {
        var ternaryPredicate = (predicate as TernaryPropertyPredicate)!;

        object? firstValue = _propertyPredicateValidator
            .GetValidValue(ternaryPredicate.FirstValue!, property.PropertyType, ternaryOperator.IgnoreTypes);

        object? secondValue = _propertyPredicateValidator
            .GetValidValue(ternaryPredicate.SecondValue!, property.PropertyType, ternaryOperator.IgnoreTypes);

        return entities.Where(entity => ternaryOperator.Operation(property.GetValue(entity), firstValue, secondValue));
    }

    protected async Task<bool> EntityExistsAsync(Guid id)
        => await _context.Set<TEntity>().AnyAsync(entity => entity.Id == id);
}
