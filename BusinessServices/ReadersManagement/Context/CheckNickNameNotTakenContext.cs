using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class CheckNickNameNotTakenContext(string nickName) : BaseGenericContext
{
    public string NickName { get; } = nickName;
}