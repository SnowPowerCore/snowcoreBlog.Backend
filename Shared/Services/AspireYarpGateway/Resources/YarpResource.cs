using Aspire.Hosting.ApplicationModel;
using Microsoft.AspNetCore.Authorization;
using snowcoreBlog.PublicApi.Utilities.Dictionary;
using Yarp.ReverseProxy.Configuration;

namespace Aspire.Hosting;

/// <summary>
/// Represents a YARP resource.
/// </summary>
/// <param name="name">The name of the resource in the application model.</param>
public class YarpResource(string name) : Resource(name), IResourceWithServiceDiscovery, IResourceWithEnvironment, IResourceWithWaitSupport
{
    // YARP configuration
    internal DictionaryWithDefault<string, RouteConfig> RouteConfigs { get; } = new(defaultValue: new(), 0);
    internal DictionaryWithDefault<string, ClusterConfig> ClusterConfigs { get; } = new(defaultValue: new(), 0);
    internal List<EndpointAnnotation> Endpoints { get; } = [];
    internal string? ConfigurationSectionName { get; set; }
    internal (string, Action<AuthorizationPolicyBuilder>)[] PoliciesConfigs { get; set; } = [];
}
