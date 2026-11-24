using snowcoreBlog.Backend.ReadersManagement.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

/// <summary>
/// Typed, AOT-safe snapshot of the confirm-by-email pipeline's checkpointable state: the request
/// plus every data-bag entry a later step consumes. Intermediate <c>IMaybe</c> wrappers are
/// captured as plain values and re-wrapped on restore.
/// </summary>
public sealed record ConfirmCreateReaderAccountCheckpoint(
    ConfirmCreateReaderAccountDto ConfirmRequest,
    DataResult<UserCreationResult>? CreateUserForReaderAccountResult,
    ReaderAccountUserCreated? ReaderAccountUserCreated,
    ReaderAccountCreated? ReaderAccountCreated);
