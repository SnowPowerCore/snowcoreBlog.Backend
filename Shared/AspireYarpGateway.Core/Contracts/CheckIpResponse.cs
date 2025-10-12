namespace snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

public record CheckIpResponse(bool IsAllowed, string? Reason = null);
