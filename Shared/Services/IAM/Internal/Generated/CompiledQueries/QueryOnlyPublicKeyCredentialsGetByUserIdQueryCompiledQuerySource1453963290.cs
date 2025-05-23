// <auto-generated/>
#pragma warning disable
using Marten.Linq;
using Marten.Linq.QueryHandlers;
using System;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

namespace Marten.Generated.CompiledQueries
{
    // START: QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuery1453963290
    public class QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuery1453963290 : Marten.Internal.CompiledQueries.ClonedCompiledQuery<System.Collections.Generic.IEnumerable<snowcoreBlog.Backend.IAM.Core.Entities.Fido2PublicKeyCredentialEntity>, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.PublicKeyCredentialsGetByUserIdQuery>
    {
        private readonly Marten.Linq.QueryHandlers.IMaybeStatefulHandler _inner;
        private readonly snowcoreBlog.Backend.IAM.CompiledQueries.Marten.PublicKeyCredentialsGetByUserIdQuery _query;
        private readonly Marten.Linq.QueryStatistics _statistics;

        public QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuery1453963290(Marten.Linq.QueryHandlers.IMaybeStatefulHandler inner, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.PublicKeyCredentialsGetByUserIdQuery query, Marten.Linq.QueryStatistics statistics) : base(inner, query, statistics)
        {
            _inner = inner;
            _query = query;
            _statistics = statistics;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters1 = builder.AppendWithParameters(@"select d.data from public.mt_doc_fido2publickeycredentialentity as d where (d.mt_deleted = False and CAST(d.data ->> 'UserId' as uuid) = ^);", '^');

            parameters1[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            parameters1[0].Value = _query.UserId;
        }

    }

    // END: QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuery1453963290
    
    
    // START: QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuerySource1453963290
    public class QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuerySource1453963290 : Marten.Internal.CompiledQueries.CompiledQuerySource<System.Collections.Generic.IEnumerable<snowcoreBlog.Backend.IAM.Core.Entities.Fido2PublicKeyCredentialEntity>, snowcoreBlog.Backend.IAM.CompiledQueries.Marten.PublicKeyCredentialsGetByUserIdQuery>
    {
        private readonly Marten.Linq.QueryHandlers.IMaybeStatefulHandler _maybeStatefulHandler;

        public QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuerySource1453963290(Marten.Linq.QueryHandlers.IMaybeStatefulHandler maybeStatefulHandler)
        {
            _maybeStatefulHandler = maybeStatefulHandler;
        }



        public override Marten.Linq.QueryHandlers.IQueryHandler<System.Collections.Generic.IEnumerable<snowcoreBlog.Backend.IAM.Core.Entities.Fido2PublicKeyCredentialEntity>> BuildHandler(snowcoreBlog.Backend.IAM.CompiledQueries.Marten.PublicKeyCredentialsGetByUserIdQuery query, Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.CompiledQueries.QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuery1453963290(_maybeStatefulHandler, query, null);
        }

    }

    // END: QueryOnlyPublicKeyCredentialsGetByUserIdQueryCompiledQuerySource1453963290
    
    
}

