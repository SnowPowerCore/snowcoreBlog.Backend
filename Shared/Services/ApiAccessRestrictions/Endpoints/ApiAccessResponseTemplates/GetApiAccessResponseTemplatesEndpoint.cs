using FastSharp.Modules.Core;
using MaybeResults;
using snowcoreBlog.Backend.ApiAccessRestrictions.Extensions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

/// <summary>
/// Lists all response templates. Short and stable by design, so it consumes the
/// repository directly (non-stepified).
/// </summary>
public sealed class GetApiAccessResponseTemplatesEndpoint(IApiAccessResponseTemplateRepository templateRepository) : IEndpoint
{
    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/v1", async (CancellationToken ct) =>
        {
            var templates = await templateRepository.GetAllAsync(ct);

            var result = Maybe.Create<IReadOnlyList<ApiAccessResponseTemplateDto>>(
                templates.Select(static t => t.ToDto()).ToList());

            return result.ToEnvelopeResult();
        });
    }
}
