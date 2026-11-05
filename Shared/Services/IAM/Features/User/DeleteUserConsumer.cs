using Marten;
using MassTransit;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class DeleteUserConsumer(IDocumentStore documentStore) : IConsumer<DeleteUser>
{
    public async Task Consume(ConsumeContext<DeleteUser> context)
    {
        // Rollback of a just-created user: user + FIDO credentials live in the same database,
        // so a single Marten session commit keeps the removal atomic on the IAM side.
        // A missing user is treated as success, which keeps compensation retries idempotent.
        await using IDocumentSession session = documentStore.LightweightSession();

        var user = await session.LoadAsync<ApplicationUserEntity>(
            context.Message.UserId.ToString(), context.CancellationToken);
        if (user is not default(ApplicationUserEntity))
        {
            var credentials = await session.Query<Fido2PublicKeyCredentialEntity>()
                .Where(credential => credential.UserId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            foreach (var credential in credentials)
            {
                session.Delete(credential);
            }

            session.Delete(user);
            await session.SaveChangesAsync(context.CancellationToken);
        }

        await context.RespondAsync(new DataResult<Guid>(context.Message.UserId));
    }
}
