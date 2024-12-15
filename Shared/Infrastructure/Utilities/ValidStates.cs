namespace snowcoreBlog.Backend.Infrastructure.Utilities;

public class ValidStates<T> where T : new()
{
    public List<T> States { get; set; } = [];
}