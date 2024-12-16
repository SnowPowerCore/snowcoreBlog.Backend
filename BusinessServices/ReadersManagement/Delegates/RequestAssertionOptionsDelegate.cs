using Fido2NetLib;
using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IResult<AssertionOptions>> RequestAssertionOptionsDelegate(RequestAssertionOptionsContext context, CancellationToken token = default);