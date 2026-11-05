using MaybeResults;
using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Core.Interfaces.Steps;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
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

public class ConfirmCreateReaderAccountContext(ConfirmCreateReaderAccountDto request) : BaseGenericContext, IStepCheckpointState
{
    public ConfirmCreateReaderAccountDto ConfirmRequest { get; } = request;

    public object CaptureCheckpoint() => new ConfirmCreateReaderAccountCheckpoint(
        ConfirmRequest,
        GetFromData<DataResult<UserCreationResult>>(ReaderAccountUserConstants.CreateUserForReaderAccountResult),
        GetFromData<IMaybe<ReaderAccountUserCreated>>(ReaderAccountConstants.CreateReaderAccountUserResult)
            is Some<ReaderAccountUserCreated> userCreated ? userCreated.Value : null,
        GetFromData<IMaybe<ReaderAccountCreated>>(ReaderAccountConstants.CreateReaderAccountResult)
            is Some<ReaderAccountCreated> accountCreated ? accountCreated.Value : null);

    public void RestoreCheckpoint(object snapshot)
    {
        var state = (ConfirmCreateReaderAccountCheckpoint)snapshot;

        if (state.CreateUserForReaderAccountResult is { } createUserResult)
        {
            SetDataWith(ReaderAccountUserConstants.CreateUserForReaderAccountResult, createUserResult);
        }

        if (state.ReaderAccountUserCreated is { } userCreated)
        {
            SetDataWith(ReaderAccountConstants.CreateReaderAccountUserResult, Maybe.Create(userCreated));
        }

        if (state.ReaderAccountCreated is { } accountCreated)
        {
            SetDataWith(ReaderAccountConstants.CreateReaderAccountResult, Maybe.Create(accountCreated));
        }
    }
}