using Marten;
using Microsoft.AspNetCore.Identity;

namespace snowcoreBlog.Backend.IAM.Stores
{
    public class MartenRoleStore<TRole> : IRoleStore<TRole>,
                                          IQueryableRoleStore<TRole> where TRole : IdentityRole
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger _logger;

        public IQueryable<TRole> Roles
        {
            get
            {
                IDocumentSession session = _documentStore.LightweightSession();
                return session.Query<TRole>();
            }
        }

        public MartenRoleStore(IDocumentStore documentStore, ILogger<MartenRoleStore<TRole>> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                using IDocumentSession session = _documentStore.IdentitySession();
                session.Store(role);
                await session.SaveChangesAsync(cancellationToken);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create the role in Marten.");
                return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong saving the role." });
            }
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                using IDocumentSession session = _documentStore.IdentitySession();
                session.Update(role);
                await session.SaveChangesAsync(cancellationToken);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update the role in Marten.");
                return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong saving the role." });
            }
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            try
            {
                using IDocumentSession session = _documentStore.IdentitySession();
                session.Delete(role);
                await session.SaveChangesAsync(cancellationToken);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete the role in Marten.");
                return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong deleting the role." });
            }
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) =>
        Task.FromResult(role.Id);

        public Task<string?> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) =>
            Task.FromResult(role.Name);

        public Task SetRoleNameAsync(TRole role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken) =>
            Task.FromResult(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(TRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            using IDocumentSession session = _documentStore.LightweightSession();
            return session.Query<TRole>().FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);
        }

        public Task<TRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            using IDocumentSession session = _documentStore.LightweightSession();
            return session.Query<TRole>().FirstOrDefaultAsync(x => x.NormalizedName == normalizedRoleName, cancellationToken);
        }

        public void Dispose() { }
    }
}