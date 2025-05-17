using NtfyCator.Messages;
using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Push.Core.Contracts;

namespace snowcoreBlog.Backend.Push.Extensions;

[Mapper]
public static partial class NtfyCatorExtensions
{
    public static partial NtfyPriority ToNtfyPriority(this NotificationPriority priority);

    public static partial NtfyHttpMethod ToNtfyHttpMethod(this NotificationHttpMethod priority);
    
    [MapDerivedType(typeof(NotificationViewAction), typeof(NtfyViewAction))]
    [MapDerivedType(typeof(NotificationBroadcastAction), typeof(NtfyBroadcastAction))]
    [MapDerivedType(typeof(NotificationHttpAction), typeof(NtfyHttpAction))]
    public static partial NtfyAction ToNtfyAction(this NotificationAction action);
}