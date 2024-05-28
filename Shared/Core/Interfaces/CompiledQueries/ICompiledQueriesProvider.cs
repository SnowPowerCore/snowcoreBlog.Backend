namespace snowcoreBlog.Backend.Core.Interfaces.CompiledQueries;

public interface ICompiledQueriesProvider
{
    TOutQueryType GetQuery<TOutQueryType>();
}