// <auto-generated/>
#pragma warning disable
using Marten.Linq.QueryHandlers;
using System;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

namespace Marten.Generated.CompiledQueries
{
    // START: DirtyTrackingApplicationTempUserByEmailQueryCompiledQuery1131388199
    public class DirtyTrackingApplicationTempUserByEmailQueryCompiledQuery1131388199 : Marten.Internal.CompiledQueries.StatelessCompiledQuery<bool, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationTempUserByEmailQuery>
    {
        private readonly Marten.Linq.QueryHandlers.IQueryHandler<bool> _inner;
        private readonly snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationTempUserByEmailQuery _query;

        public DirtyTrackingApplicationTempUserByEmailQueryCompiledQuery1131388199(Marten.Linq.QueryHandlers.IQueryHandler<bool> inner, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationTempUserByEmailQuery query) : base(inner, query)
        {
            _inner = inner;
            _query = query;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters1 = builder.AppendWithParameters(@"select TRUE as result from public.mt_doc_applicationtempuserentity as d where d.data ->> 'Email' = ^ LIMIT ^;", '^');

            parameters1[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters1[0].Value = _query.Email;
            parameters1[1].Value = 1;
            parameters1[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
        }

    }

    // END: DirtyTrackingApplicationTempUserByEmailQueryCompiledQuery1131388199
    
    
    // START: DirtyTrackingApplicationTempUserByEmailQueryCompiledQuerySource1131388199
    public class DirtyTrackingApplicationTempUserByEmailQueryCompiledQuerySource1131388199 : Marten.Internal.CompiledQueries.CompiledQuerySource<bool, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationTempUserByEmailQuery>
    {
        private readonly Marten.Linq.QueryHandlers.IQueryHandler<bool> _queryHandler;

        public DirtyTrackingApplicationTempUserByEmailQueryCompiledQuerySource1131388199(Marten.Linq.QueryHandlers.IQueryHandler<bool> queryHandler)
        {
            _queryHandler = queryHandler;
        }



        public override Marten.Linq.QueryHandlers.IQueryHandler<bool> BuildHandler(snowcoreBlog.Backend.IAM.CompiledQueries.Marten.ApplicationTempUserByEmailQuery query, Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.CompiledQueries.DirtyTrackingApplicationTempUserByEmailQueryCompiledQuery1131388199(_queryHandler, query);
        }

    }

    // END: DirtyTrackingApplicationTempUserByEmailQueryCompiledQuerySource1131388199
    
    
}

