using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Identity;
using snowcoreBlog.Backend.IAM.Stores;

namespace snowcoreBlog.Backend.IAM.Extensions;

public static class MartenIdentityExtensions
{
    public static IdentityBuilder AddMartenStores<TUser, TRole>(this IdentityBuilder builder)
                                                                where TUser : IdentityUser, IClaimsUser
                                                                where TRole : IdentityRole
    {
        builder
            .AddRoleStore<MartenRoleStore<TRole>>()
            .AddRoleManager<RoleManager<TRole>>();
        builder
            .AddUserStore<MartenUserStore<TUser>>()
            .AddUserManager<UserManager<TUser>>();
        return builder;
    }
}