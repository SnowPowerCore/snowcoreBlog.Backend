﻿namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record UserCreationResult
{
    public required Guid Id { get; init; }
}