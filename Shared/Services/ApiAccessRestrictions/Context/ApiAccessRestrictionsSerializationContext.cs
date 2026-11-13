using System.Text.Json.Serialization;
using snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

/// <summary>
/// Service-local JSON serialization context for endpoint wire types that are owned by this
/// service and therefore cannot live in the shared <c>CoreSerializationContext</c>.
/// </summary>
[JsonSerializable(typeof(ErrorResponseWire))]
internal sealed partial class ApiAccessRestrictionsSerializationContext : JsonSerializerContext;
