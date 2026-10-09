using MaybeResults;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IMaybe<ReaderAccountCreatedDto>> ConfirmCreateReaderAccountWithAuthenticatorDelegate(ConfirmCreateReaderAccountWithAuthenticatorContext context, CancellationToken token = default);
