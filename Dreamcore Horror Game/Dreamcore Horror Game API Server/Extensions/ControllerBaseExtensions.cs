using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class ControllerBaseExtensions
{
    public static StatusCodeResult Forbidden(this ControllerBase controller)
        => controller.StatusCode(403);

    public static ObjectResult Forbidden(this ControllerBase controller, object? error)
        => controller.StatusCode(403, error);
}
