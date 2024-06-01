using Marten.Linq;
using snowcoreBlog.Backend.Core.Interfaces.CompiledQueries;

namespace snowcoreBlog.Backend.Infrastructure;

public class MartenCompiledQueryProvider<TDoc, TOut> : ICompiledQueriesProvider
{
    private ICompiledQuery<TDoc, TOut>? _compiledQuery;

    public TCompiledQuery? GetQuery<TCompiledQuery>() where TCompiledQuery : notnull =>
        (TCompiledQuery)_compiledQuery;

    public static MartenCompiledQueryProvider<TDoc, TOut> Create(ICompiledQuery<TDoc, TOut> queryInstance) =>
        new() { _compiledQuery = queryInstance };
}