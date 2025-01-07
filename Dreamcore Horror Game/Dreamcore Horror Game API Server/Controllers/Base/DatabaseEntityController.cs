using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Models.DTO;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Controllers.Base;

[ApiController]
public abstract class DatabaseEntityController<TEntity>
(
    DreamcoreHorrorGameContext context,
    IPropertyPredicateValidator propertyPredicateValidator,
    ILogger<DatabaseEntityController<TEntity>> logger,
    IJsonSerializerOptionsProvider jsonSerializerOptionsProvider,
    IRabbitMqProducer rabbitMqProducer,
    Expression<Func<TEntity, object?>> orderBySelectorExpression,
    IComparer<object?>? orderByComparer,
    Func<DreamcoreHorrorGameContext, Task<IQueryable<TEntity>>> getAllWithFirstLevelRelationsFunction,
    Func<DreamcoreHorrorGameContext, TEntity, Task>? setRelationsFromForeignKeysFunction
)
: ControllerBase
where TEntity : class, IDatabaseEntity
{
    private event Action<DateTime> RequestSenderValidated = (requestReceptionDateTime) => { };

    protected readonly DreamcoreHorrorGameContext _context = context;
    protected readonly IPropertyPredicateValidator _propertyPredicateValidator = propertyPredicateValidator;
    protected readonly ILogger<DatabaseEntityController<TEntity>> _logger = logger;
    protected readonly IJsonSerializerOptionsProvider _jsonSerializerOptionsProvider = jsonSerializerOptionsProvider;
    protected readonly IRabbitMqProducer _rabbitMqProducer = rabbitMqProducer;

    protected readonly Func<string, Exception?, string> _customLoggingFormatter = (message, exception) =>
    {
        StringBuilder sb = new(message);

        if (exception is not null)
        {
            sb.Append($"{Environment.NewLine}{exception.GetType()}: {exception.Message}");

            if (!string.IsNullOrEmpty(exception.StackTrace))
                sb.Append($"{Environment.NewLine}{exception.StackTrace}");
        }

        return sb.ToString();
    };

    protected readonly List<string> _allowedRequestSenders = [];
    protected string? _requestSender = null;

    protected readonly Expression<Func<TEntity, object?>> _orderBySelectorExpression = orderBySelectorExpression;
    protected readonly IComparer<object?>? _orderByComparer = orderByComparer;

    // This function may contain logic that manually clears unnecessary references in IQueryables to save traffic
    // and data loading times. DO NOT SAVE any data returned by this function to the database!
    protected readonly Func<DreamcoreHorrorGameContext, Task<IQueryable<TEntity>>> _getAllWithFirstLevelRelations = getAllWithFirstLevelRelationsFunction;

    protected readonly Func<DreamcoreHorrorGameContext, TEntity, Task>? _setRelationsFromForeignKeys = setRelationsFromForeignKeysFunction;

    //private readonly Func<DreamcoreHorrorGameContext, DatabaseController<TEntity>> _derivedClassConstructor;

    protected string EntityType => typeof(TEntity).Name;

    protected string AuthorizationToken
        => HttpContext.Request.Headers[HeaderNames.Authorization]
            .ToString()
            .Replace("Bearer ", string.Empty);

    protected bool InvalidModelState => ModelState.IsValid is false;

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

    protected virtual DatabaseEntityController<TEntity> AllowRequestSenders(params string[] senders)
    {
        SetAllowedRequestSenders(senders);
        return this;
    }

    protected void SetAllowedRequestSenders(IEnumerable<string> fromCollection)
    {
        _allowedRequestSenders.Clear();
        _allowedRequestSenders.AddRange(fromCollection);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetCountAsync()
        => await ValidateRequestSenderAndHandleErrorsAsync(async () =>
        {
            _logger.LogInformation("GetCount was called for {EntityType}.", EntityType);
            PublishStatistics("GetCount");

            return Ok(_context.Set<TEntity>().Count());
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAllEntitiesAsync(int page, int showBy)
        => await ValidateRequestSenderAndHandleErrorsAsync(async () =>
        {
            _logger.LogInformation("GetAll was called for {EntityType}.", EntityType);
            PublishStatistics("GetAll");

            var entities = await _context.Set<TEntity>()
                .OrderBy(_orderBySelectorExpression)
                .ToListAsync();

            return Ok(entities.PaginatedAndWithPageCount(page, showBy));
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetAllEntitiesWithRelationsAsync(int page, int showBy)
        => await ValidateRequestSenderAndHandleErrorsAsync(async () =>
        {
            _logger.LogInformation("GetAllWithRelations was called for {EntityType}.", EntityType);
            PublishStatistics("GetAllWithRelations");

            var entities = await (await _getAllWithFirstLevelRelations(_context))
                .OrderBy(_orderBySelectorExpression)
                .ToListAsync();

            return Ok(entities.PaginatedAndWithPageCount(page, showBy));
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> GetEntityAsync(Guid? id)
        => await ValidateRequestSenderAndHandleErrorsAsync(id, async id =>
        {
            _logger.LogInformation("Get was called for {EntityType}.", EntityType);
            PublishStatistics("Get");

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
    public async Task<IActionResult> GetEntity(Func<TEntity, bool> predicate)
        => await ValidateRequestSenderAndHandleErrorsAsync(predicate, async predicate =>
        {
            _logger.LogInformation("Get (with predicate) was called for {EntityType}.", EntityType);
            PublishStatistics("Get");

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
        => await ValidateRequestSenderAndHandleErrorsAsync(id, async id =>
        {
            _logger.LogInformation("GetWithRelations was called for {EntityType}.", EntityType);
            PublishStatistics("GetWithRelations");

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
        => await ValidateRequestSenderAndHandleErrorsAsync(predicate, async predicate =>
        {
            _logger.LogInformation("GetWithRelations (with predicate) was called for {EntityType}.", EntityType);
            PublishStatistics("GetWithRelations");

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
        => await ValidateRequestSenderAndHandleErrorsAsync(propertyPredicateCollection, async propertyPredicateCollection =>
        {
            _logger.LogInformation("GetWhere was called for {EntityType}.", EntityType);
            PublishStatistics("GetWhere");

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

            try
            {
                propertyPredicateCollection.ForEach(predicate => entities = FilterEntities(entities, predicate));
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new UnknownOperatorResult().Message);
            }

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
        => await ValidateRequestSenderAndHandleErrorsAsync(entity, async entity =>
        {
            _logger.LogInformation("Create was called for {EntityType}.", EntityType);
            PublishStatistics("Create");

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
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError
                (
                    eventId: new EventId("CreateEntityAsync".GetHashCode() + ex.GetType().GetHashCode()),
                    message: "Database conflict occured while creating {EntityType} with id = {id}.", EntityType, entity.Id
                );

                return Conflict(ErrorMessages.CreateConflict);
            }

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> EditEntityAsync(Guid? id, TEntity entity)
        => await ValidateRequestSenderAndHandleErrorsAsync(id, entity, async (id, entity) =>
        {
            _logger.LogInformation("Edit was called for {EntityType}.", EntityType);
            PublishStatistics("Edit");

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
            catch (DbUpdateConcurrencyException ex)
            {
                if (await EntityExistsAsync(entity.Id) is false)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError
                    (
                        eventId: new EventId("EditEntityAsync".GetHashCode() + ex.GetType().GetHashCode()),
                        message: "Database conflict occured while editing {EntityType} with id = {id}.", EntityType, entity.Id
                    );

                    return Conflict(ErrorMessages.EditConflict);
                }
            }

            return Ok(entity);
        });

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> DeleteEntityAsync(Guid? id)
        => await ValidateRequestSenderAndHandleErrorsAsync(id, async id =>
        {
            _logger.LogInformation("Delete was called for {EntityType}.", EntityType);
            PublishStatistics("Delete");

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
        => await ValidateRequestSenderAndHandleErrorsAsync(function());

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> ExecuteAsync<T>(T arg, Func<T, Task<IActionResult>> function)
        => await ValidateRequestSenderAndHandleErrorsAsync(arg, async arg => await function(arg));

    [ApiExplorerSettings(IgnoreApi = true)]
    [NonAction]
    public async Task<IActionResult> ExecuteAsync<T1, T2>(T1 arg1, T2 arg2, Func<T1, T2, Task<IActionResult>> function)
        => await ValidateRequestSenderAndHandleErrorsAsync(arg1, arg2, async (arg1, arg2) => await function(arg1, arg2));

    protected async Task<IActionResult> ValidateRequestSenderAndHandleErrorsAsync(Func<Task<IActionResult>> function)
        => await ValidateRequestSenderAndHandleErrorsAsync(function());

    protected async Task<IActionResult> ValidateRequestSenderAndHandleErrorsAsync<T>(T arg, Func<T, Task<IActionResult>> function)
        => await ValidateRequestSenderAndHandleErrorsAsync(function(arg));

    protected async Task<IActionResult> ValidateRequestSenderAndHandleErrorsAsync<T1, T2>(T1 arg1, T2 arg2, Func<T1, T2, Task<IActionResult>> function)
        => await ValidateRequestSenderAndHandleErrorsAsync(function(arg1, arg2));

    protected async Task<IActionResult> ValidateRequestSenderAndHandleErrorsAsync(Task<IActionResult> task)
    {
        _requestSender = GetRequestSenderOrDefault();

        if (_allowedRequestSenders.Count is > 0 && string.Equals(_requestSender, default))
            return this.Forbidden(ErrorMessages.CorsHeaderMissing);

        RequestSenderValidated.Invoke(DateTime.Now);

        try
        {
            return await task;
        }
        catch (InvalidConstraintException ex)
        {
            _logger.Log
            (
                logLevel: LogLevel.Error,
                eventId: new EventId("ValidateRequestSenderAndHandleErrorsAsync".GetHashCode() + ex.GetType().GetHashCode()),
                exception: ex,
                state: "Invalid constraint exception was caught while handling general errors.",
                formatter: _customLoggingFormatter
            );

            return UnprocessableEntity(ErrorMessages.RelatedEntityDoesNotExist);
        }
        catch (DbUpdateException ex)
        {
            _logger.Log
            (
                logLevel: LogLevel.Error,
                eventId: new EventId("ValidateRequestSenderAndHandleErrorsAsync".GetHashCode() + ex.GetType().GetHashCode()),
                exception: ex,
                state: "Database update exception was caught while handling general errors.",
                formatter: _customLoggingFormatter
            );

            return UnprocessableEntity(ErrorMessages.UnknownDatabaseError);
        }
        catch (Exception ex)
        {
            _logger.Log
            (
                logLevel: LogLevel.Error,
                eventId: new EventId("ValidateRequestSenderAndHandleErrorsAsync".GetHashCode() + ex.GetType().GetHashCode()),
                exception: ex,
                state: "General exception was caught while handling general errors.",
                formatter: _customLoggingFormatter
            );

            return this.InternalServerError(ErrorMessages.UnknownServerError);
        }
    }

    protected string? GetRequestSenderOrDefault()
    {
        if (HttpContext is null || HttpContext.Request is null || HttpContext.Request.Headers is null)
            return default;

        var headers = HttpContext.Request.Headers;

        var validRequestSenderHeaders = headers.Where(header => _allowedRequestSenders.Contains(header.Key.ToLower())
            && header.Value.Equals(RequestSenders.RequiredHeaderValue));

        if (validRequestSenderHeaders.IsEmpty())
        {
            _logger.LogWarning
            (
                eventId: new EventId("GetRequestSender".GetHashCode() + 1),
                message: "Detected a request attempt with no valid request sender headers."
            );

            return default;
        }

        if (validRequestSenderHeaders.Count() > 1)
        {
            _logger.LogWarning
            (
                eventId: new EventId("GetRequestSender".GetHashCode() + 2),
                message: "Detected a request attempt with more than one valid request sender header."
            );

            return default;
        }

        return validRequestSenderHeaders.First().Key.ToLower();
    }

    protected void PublishStatistics(string methodName)
    {
        if (string.IsNullOrEmpty(_requestSender))
            RequestSenderValidated += (requestReceptionDateTime) => StartPublishStatisticsTask(requestReceptionDateTime, methodName);
        else
            StartPublishStatisticsTask(DateTime.Now, methodName);
    }

    protected void StartPublishStatisticsTask(DateTime receptionDateTime, string methodName)
        => Task.Run(async () => await PublishStatisticsAsync(receptionDateTime, methodName));

    protected async Task PublishStatisticsAsync(DateTime receptionDateTime, string methodName)
    {
        string controllerFullName = GetType().Name;
        string controllerName = controllerFullName[0..controllerFullName.LastIndexOf("Controller")];

        RequestStatisticsDTO requestStatisticsDTO = new()
        {
            Id = Guid.Empty,
            ReceptionTimestamp = receptionDateTime,
            SenderName = _requestSender ?? string.Empty,
            ControllerName = controllerName,
            MethodName = methodName
        };

        string json = JsonSerializer.Serialize(requestStatisticsDTO, _jsonSerializerOptionsProvider.Default);
        byte[] data = Encoding.UTF8.GetBytes(json);

        bool success = await _rabbitMqProducer.PublishMessageAsync(exchange: RabbitMqExchangeNames.Statistics, message: data);

        _logger.LogInformation("Completed publishing statistics.{newline}{sender} called {controllerName}/{methodName} at {receptionDateTime}.{newline}Status: {status}.",
            Environment.NewLine, requestStatisticsDTO.SenderName, requestStatisticsDTO.ControllerName, requestStatisticsDTO.MethodName, requestStatisticsDTO.ReceptionTimestamp,
            Environment.NewLine, success ? "success" : "fail");
    }
}
