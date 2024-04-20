using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class ControllerBaseExtensions
{
    public static StatusCodeResult Forbidden(this ControllerBase controller)
        => controller.StatusCode(403);

    public static ObjectResult Forbidden(this ControllerBase controller, object? error)
        => controller.StatusCode(403, error);

    public static StatusCodeResult InternalServerError(this ControllerBase controller)
        => controller.StatusCode(500);

    public static ObjectResult InternalServerError(this ControllerBase controller, object? error)
        => controller.StatusCode(500, error);
}
