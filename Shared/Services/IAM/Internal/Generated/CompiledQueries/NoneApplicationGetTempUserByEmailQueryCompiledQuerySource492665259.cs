// <auto-generated/>
#pragma warning disable
using Marten.Linq;
using Marten.Linq.QueryHandlers;
using System;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

namespace Marten.Generated.CompiledQueries
{
    // START: NoneApplicationGetTempUserByEmailQueryCompiledQuery492665259
    public class NoneApplicationGetTempUserByEmailQueryCompiledQuery492665259 : Marten.Internal.CompiledQueries.ClonedCompiledQuery<snowcoreBlog.Backend.IAM.Core.Entities.ApplicationTempUserEntity, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationGetTempUserByEmailQuery>
    {
        private readonly Marten.Linq.QueryHandlers.IMaybeStatefulHandler _inner;
        private readonly snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationGetTempUserByEmailQuery _query;
        private readonly Marten.Linq.QueryStatistics _statistics;

        public NoneApplicationGetTempUserByEmailQueryCompiledQuery492665259(Marten.Linq.QueryHandlers.IMaybeStatefulHandler inner, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationGetTempUserByEmailQuery query, Marten.Linq.QueryStatistics statistics) : base(inner, query, statistics)
        {
            _inner = inner;
            _query = query;
            _statistics = statistics;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters1 = builder.AppendWithParameters(@"select d.id, d.data from public.mt_doc_applicationtempuserentity as d where d.data ->> 'Email' = ^ LIMIT ^;", '^');

            parameters1[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters1[0].Value = _query.Email;
            parameters1[1].Value = 1;
            parameters1[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
        }

    }

    // END: NoneApplicationGetTempUserByEmailQueryCompiledQuery492665259
    
    
    // START: NoneApplicationGetTempUserByEmailQueryCompiledQuerySource492665259
    public class NoneApplicationGetTempUserByEmailQueryCompiledQuerySource492665259 : Marten.Internal.CompiledQueries.CompiledQuerySource<snowcoreBlog.Backend.IAM.Core.Entities.ApplicationTempUserEntity, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationGetTempUserByEmailQuery>
    {
        private readonly Marten.Linq.QueryHandlers.IMaybeStatefulHandler _maybeStatefulHandler;

        public NoneApplicationGetTempUserByEmailQueryCompiledQuerySource492665259(Marten.Linq.QueryHandlers.IMaybeStatefulHandler maybeStatefulHandler)
        {
            _maybeStatefulHandler = maybeStatefulHandler;
        }



        public override Marten.Linq.QueryHandlers.IQueryHandler<snowcoreBlog.Backend.IAM.Core.Entities.ApplicationTempUserEntity> BuildHandler(snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationGetTempUserByEmailQuery query, Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.CompiledQueries.NoneApplicationGetTempUserByEmailQueryCompiledQuery492665259(_maybeStatefulHandler, query, null);
        }

    }

    // END: NoneApplicationGetTempUserByEmailQueryCompiledQuerySource492665259
    
    
}

