using MinimalStepifiedSystem.Interfaces;

namespace snowcoreBlog.Backend.ReadersManagement;

public class SendEmailToNewReaderAccountStep : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext>
{
    public Task InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        return next(context, token);
    }
}