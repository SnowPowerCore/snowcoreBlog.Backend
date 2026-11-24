namespace snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;

public enum RestrictionLevel
{
    Block = 0,
    RequireCaptcha = 1,
    AllowWithWarning = 2
}