using System.Security.Claims;
using Marten;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Identity;

namespace snowcoreBlog.Backend.IAM.Stores
{
    public class MartenUserStore<TUser> : IUserStore<TUser>,
                                          IUserPasswordStore<TUser>,
                                          IUserEmailStore<TUser>,
                                          IUserPhoneNumberStore<TUser>,
                                          IUserTwoFactorStore<TUser>,
                                          IUserAuthenticatorKeyStore<TUser>,
                                          IUserTwoFactorRecoveryCodeStore<TUser>,
                                          IQueryableUserStore<TUser>,
                                          IUserClaimStore<TUser>
                                          where TUser : IdentityUser, IClaimsUser
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger _logger;

        public IQueryable<TUser> Users
        {
            get
            {
                IDocumentSession session = _documentStore.LightweightSession();
                return session.Query<TUser>();
            }
        }

        public MartenUserStore(IDocumentStore documentStore, ILogger<MartenUserStore<TUser>> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id);

        public Task<string?> GetUserNameAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.UserName);

        public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(TUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using IDocumentSession session = _documentStore.IdentitySession();
                session.Store(user);
                await session.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create the user in Marten.");
                return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong saving the user." });
            }
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                using IDocumentSession session = _documentStore.IdentitySession();
                session.Update(user);
                await session.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update the user in Marten.");
                return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong saving the user." });
            }
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            try
            {
                using IDocumentSession session = _documentStore.IdentitySession();
                session.Delete(user);
                await session.SaveChangesAsync(cancellationToken);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete the user in Marten.");
                return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong deleting the user." });
            }
        }

        public Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using IDocumentSession session = _documentStore.LightweightSession();
            return session.Query<TUser>().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using IDocumentSession session = _documentStore.LightweightSession();
            return session.Query<TUser>().FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
        }

        public Task SetPasswordHashAsync(TUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string?> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            bool hasPassword = !string.IsNullOrEmpty(user.PasswordHash);
            return Task.FromResult(hasPassword);
        }

        public Task<string?> GetEmailAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Email);

        public Task SetEmailAsync(TUser user, string? email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.EmailConfirmed);

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<TUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            using IDocumentSession session = _documentStore.LightweightSession();
            return session.Query<TUser>().FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public Task<string?> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.NormalizedEmail);

        public Task SetNormalizedEmailAsync(TUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task<string?> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PhoneNumber);

        public Task SetPhoneNumberAsync(TUser user, string? phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.PhoneNumberConfirmed);

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(user.TwoFactorEnabled);

        public Task<string?> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult("1234123412")!;

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken) =>
            Task.FromResult(false);

        public Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken) =>
            Task.FromResult(5);

        public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            using IDocumentSession session = _documentStore.LightweightSession();
            var resolvedUser = await session.Query<TUser>().FirstOrDefaultAsync(x => x.NormalizedEmail == user.NormalizedEmail, cancellationToken);

            var claimsList = new List<Claim>();
            if (resolvedUser?.RoleClaims != null)
            {
                foreach (string roleClaim in resolvedUser.RoleClaims)
                {
                    claimsList.Add(new Claim(ClaimTypes.Role, roleClaim));
                }
            }

            return claimsList;
        }

        public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            try
            {
                var userRoleClaims = new List<string>();
                foreach (Claim claimItem in claims)
                {
                    if (claimItem.Type == ClaimTypes.Role)
                    {
                        userRoleClaims.Add(claimItem.Value);
                    }
                }

                user.RoleClaims = userRoleClaims;

                using IDocumentSession session = _documentStore.IdentitySession();
                session.Store(user);
                await session.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add claims to the user {user.Email} in Marten.");
            }
        }

        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (claim.Type != ClaimTypes.Role || newClaim.Type != ClaimTypes.Role)
            {
                return;
            }

            var existingClaims = await GetClaimsAsync(user, cancellationToken);
            if (existingClaims != null)
            {
                List<Claim> claimsList = existingClaims.ToList();
                int index = claimsList.FindIndex(x => x.Value == claim.Value);
                claimsList.RemoveAt(index);
                claimsList.Add(newClaim);

                await AddClaimsAsync(user, claimsList, cancellationToken);
            }
        }

        public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            try
            {
                var existingClaims = await GetClaimsAsync(user, cancellationToken);
                if (existingClaims != null)
                {
                    var newClaims = existingClaims.ToList();
                    foreach (Claim claimToRemove in claims)
                    {
                        int index = newClaims.FindIndex(x => x.Type == claimToRemove.Type && x.Value == claimToRemove.Value);
                        newClaims.RemoveAt(index);
                    }

                    await AddClaimsAsync(user, newClaims, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add claims to the user {user.Email} in Marten.");
            }
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            using IDocumentSession session = _documentStore.LightweightSession();
            IReadOnlyList<TUser> readonlyList = await session.Query<TUser>()
                                                        .Where(x => x.RoleClaims.Contains(claim.Value))
                                                        .ToListAsync(cancellationToken);

            return [.. readonlyList];
        }

        public void Dispose() { }

        public void Wipe()
        {
            using IDocumentSession session = _documentStore.IdentitySession();
            session.DeleteWhere<TUser>(x => true);
            session.SaveChanges();
        }
    }
}