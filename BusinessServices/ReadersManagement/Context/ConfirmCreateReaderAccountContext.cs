using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class ConfirmCreateReaderAccountContext(ConfirmCreateReaderAccountDto request) : BaseGenericContext
{
    public ConfirmCreateReaderAccountDto ConfirmRequest { get; } = request;
}