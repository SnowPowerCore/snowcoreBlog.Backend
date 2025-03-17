using Fido2NetLib;
using MaybeResults;
using snowcoreBlog.Backend.ReadersManagement.Context;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IMaybe<CredentialCreateOptions>> RequestAttestationOptionsDelegate(RequestAttestationOptionsContext context, CancellationToken token = default);