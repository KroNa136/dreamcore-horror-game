using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class ArtifactsController : DatabaseEntityController<Artifact>
{
    public ArtifactsController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetAsync(artifact => artifact.Id == id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([Bind(
        nameof(Artifact.Id),
        nameof(Artifact.AssetName),
        nameof(Artifact.RarityLevelId)
    )] Artifact artifact)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateAsync(artifact);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(Artifact.Id),
        nameof(Artifact.AssetName),
        nameof(Artifact.RarityLevelId)
    )] Artifact artifact)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditAsync(id, artifact);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteAsync(id);
}
