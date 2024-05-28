using Microsoft.AspNetCore.Identity;
using Results;

namespace snowcoreBlog.Backend.IAM;

[ErrorResult]
public partial record ApplicationIdentityStoreCreationError<T>
{
    public ApplicationIdentityStoreCreationError(List<IdentityError> errors)
    {
        Message = typeof(T).Name;

        var errorList = new List<ErrorResultDetail>(errors.Count);
        errorList.AddRange(errors.Select(e => new ErrorResultDetail(e.Code, e.Description)));
        Errors = errorList;
    }
}