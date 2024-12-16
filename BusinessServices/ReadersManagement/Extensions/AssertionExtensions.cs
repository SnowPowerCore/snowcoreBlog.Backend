using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class AssertionExtensions
{
    public static partial ValidateAndCreateAssertion ToValidate(this RequestAssertionOptionsDto requestAssertionOptions);
}