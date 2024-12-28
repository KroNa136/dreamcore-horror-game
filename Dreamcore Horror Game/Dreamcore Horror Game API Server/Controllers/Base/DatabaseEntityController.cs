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
using System.Linq.Expressions;
using System.Reflection;

namespace DreamcoreHorrorGameApiServer.Controllers.Base;

[ApiController]
public abstract class DatabaseEntityController<TEntity> : ControllerBase
    where TEntity : class, IDatabaseEntity
{
    protected string AuthorizationToken
        => HttpContext.Request.Headers[HeaderNames.Authorization]
            .ToString()
            .Replace("Bearer ", string.Empty);

    protected bool InvalidModelState => ModelState.IsValid is false;

    protected readonly DreamcoreHorrorGameContext _context;
    protected readonly IPropertyPredicateValidator _propertyPredicateValidator;

    protected readonly List<string> _requiredRequestHeaders;

    protected readonly Expression<Func<TEntity, object?>> _orderBySelectorExpression;
    protected readonly IComparer<object?>? _orderByComparer;

    // This function may contain logic that manually clears unnecessary references in IQueryables to save traffic
    // and data loading times. DO NOT SAVE any data returned by this function to the database!
    protected readonly Func<DreamcoreHorrorGameContext, Task<IQueryable<TEntity>>> _getAllWithFirstLevelRelations;

    protected readonly Func<DreamcoreHorrorGameContext, TEntity, Task>? _setRelationsFromForeignKeys;

    //private readonly Func<DreamcoreHorrorGameContext, DatabaseController<TEntity>> _derivedClassConstructor;

    public DatabaseEntityController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator,
        Expression<Func<TEntity, object?>> orderBySelectorExpression,
        IComparer<object?>? orderByComparer,
        Func<DreamcoreHorrorGameContext, Task<IQueryable<TEntity>>> getAllWithFirstLevelRelationsFunction,
        Func<DreamcoreHorrorGameContext, TEntity, Task>? setRelationsFromForeignKeysFunction
    )
    {
        _context = context;
        _propertyPredicateValidator = propertyPredicateValidator;

        _requiredRequestHeaders = new List<string>();

        _orderBySelectorExpression = orderBySelectorExpression;
        _orderByComparer = orderByComparer;

        _getAllWithFirstLevelRelations = getAllWithFirstLevelRelationsFunction;
        _setRelationsFromForeignKeys = setRelationsFromForeignKeysFunction;
    }

    // POST note:
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    public abstract Task<IActionResult> GetCount();
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
        SetRequiredRequestHeaders(headers);
        return this;
    }

    protected void SetRequiredRequestHeaders(IEnumerable<string> fromCollection)
    {
        _requiredRequestHeaders.Clear();
        _requiredRequestHeaders.AddRange(fromCollection);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetCountAsync()
        => await ValidateHeadersAndHandleErrorsAsync(async () => Ok(_context.Set<TEntity>().Count()));

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAllEntitiesAsync(int page, int showBy)
        => await ValidateHeadersAndHandleErrorsAsync(async () =>
        {
            var entities = await _context.Set<TEntity>()
                .OrderBy(_orderBySelectorExpression)
                .ToListAsync();

            return Ok(entities.PaginatedAndWithPageCount(page, showBy));
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAllEntitiesWithRelationsAsync(int page, int showBy)
        => await ValidateHeadersAndHandleErrorsAsync(async () =>
        {
            var entities = await (await _getAllWithFirstLevelRelations(_context))
                .OrderBy(_orderBySelectorExpression)
                .ToListAsync();

            return Ok(entities.PaginatedAndWithPageCount(page, showBy));
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
    public async Task<IActionResult> GetEntitiesWhereAsync
    (
        IEnumerable<PropertyPredicate> propertyPredicateCollection,
        int page,
        int showBy
    )
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

            propertyPredicateCollection.ForEach(predicate => entities = FilterEntities(entities, predicate));

            var entitiesEnumerable = entities.AsEnumerable();

            var orderedEntities = _orderByComparer is not null
                ? entitiesEnumerable.OrderBy(_orderBySelectorExpression.Compile(), _orderByComparer)
                : entitiesEnumerable.OrderBy(_orderBySelectorExpression.Compile());

            return Ok(orderedEntities.PaginatedAndWithPageCount(page, showBy));
        });

    protected ParallelQuery<TEntity> FilterEntities(ParallelQuery<TEntity> entities, PropertyPredicate predicate)
    {
        var property = GetProperty(typeof(TEntity), predicate.Property)!;
        var _operator = _propertyPredicateValidator.GetOperator(predicate.Operator);

        return _operator switch
        {
            UnaryOperator unaryOperator => FilterEntitiesWithUnaryOperator
            (
                entities,
                property,
                unaryOperator
            ),
            BinaryOperator binaryOperator => FilterEntitiesWithBinaryOperator
            (
                entities,
                property,
                binaryOperator,
                (predicate as BinaryPropertyPredicate)!.Value!
            ),
            TernaryOperator ternaryOperator => FilterEntitiesWithTernaryOperator
            (
                entities,
                property,
                ternaryOperator,
                (predicate as TernaryPropertyPredicate)!.FirstValue!,
                (predicate as TernaryPropertyPredicate)!.SecondValue!
            ),
            _ => throw new InvalidOperationException()
        };
    }

    protected static PropertyInfo? GetProperty(Type type, string name)
        => type.GetProperties()
            .FirstOrDefault(property => property.Name.ToLower().Equals(name.ToLower().Replace("_", string.Empty)));

    protected static ParallelQuery<TEntity> FilterEntitiesWithUnaryOperator
    (
        ParallelQuery<TEntity> entities,
        PropertyInfo property,
        UnaryOperator unaryOperator
    )
    {
        return entities.Where(entity => unaryOperator.Operation(property.GetValue(entity)));
    }

    protected ParallelQuery<TEntity> FilterEntitiesWithBinaryOperator
    (
        ParallelQuery<TEntity> entities,
        PropertyInfo property,
        BinaryOperator binaryOperator,
        object value
    )
    {
        object validValue = _propertyPredicateValidator
            .GetValidValue(value, property.PropertyType, binaryOperator.IgnoreTypes);

        return entities.Where(entity => binaryOperator.Operation(property.GetValue(entity), validValue));
    }

    protected ParallelQuery<TEntity> FilterEntitiesWithTernaryOperator
    (
        ParallelQuery<TEntity> entities,
        PropertyInfo property,
        TernaryOperator ternaryOperator,
        object firstValue,
        object secondValue
    )
    {
        object validFirstValue = _propertyPredicateValidator
            .GetValidValue(firstValue, property.PropertyType, ternaryOperator.IgnoreTypes);

        object validSecondValue = _propertyPredicateValidator
            .GetValidValue(secondValue, property.PropertyType, ternaryOperator.IgnoreTypes);

        return entities.Where(entity => ternaryOperator.Operation(property.GetValue(entity), validFirstValue, validSecondValue));
    }

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

            var entity = await _context.Set<TEntity>()
                .FirstOrDefaultAsync(entity => entity.Id == id);

            if (entity is null)
                return NotFound();

            _context.Remove(entity);

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

                if (await EntityExistsAsync(entity.Id) is false)
                    // The most possible scenario for this case is when two users try to delete the same entity
                    // one right after the other, so it makes sense to return Ok here because the user's goal
                    // is accomplished - the entity doesn't exist anymore.
                    return Ok();

                return Conflict(ErrorMessages.DeleteConflict);
            }

            return Ok();
        });

    protected async Task<bool> EntityExistsAsync(Guid id)
        => await _context.Set<TEntity>().AnyAsync(entity => entity.Id == id);

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

    protected IActionResult ValidateHeadersAndHandleErrors<T>(T arg, Func<T, IActionResult> function)
        => ValidateHeadersAndHandleErrors(() => function(arg));

    protected IActionResult ValidateHeadersAndHandleErrors<T1, T2>(T1 arg1, T2 arg2, Func<T1, T2, IActionResult> function)
        => ValidateHeadersAndHandleErrors(() => function(arg1, arg2));

    protected IActionResult ValidateHeadersAndHandleErrors(Func<IActionResult> function)
    {
        if (NoValidRequestHeader())
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
        if (NoValidRequestHeader())
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

    protected bool NoValidRequestHeader()
    {
        if (_requiredRequestHeaders.IsEmpty())
            return false;

        if (HttpContext is null || HttpContext.Request is null || HttpContext.Request.Headers is null)
            return true;

        var currentRequestHeaders = HttpContext.Request.Headers;

        var validHeaders = currentRequestHeaders.Where(header => _requiredRequestHeaders.Contains(header.Key.ToLower())
            && header.Value.Equals(CorsHeaders.RequiredHeaderValue));

        return validHeaders.IsEmpty();
    }
}
