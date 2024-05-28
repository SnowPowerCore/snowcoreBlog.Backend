using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.ReadersManagement;

public class CreateReaderAccountContext(CreateReaderAccountDto request) : BaseGenericContext
{
    public CreateReaderAccountDto Request { get; } = request;
}