// <auto-generated/>
#pragma warning disable
using Marten.Internal;
using Marten.Internal.Storage;
using Marten.Schema;
using Marten.Schema.Arguments;
using Npgsql;
using System;
using System.Collections.Generic;
using Weasel.Core;
using Weasel.Postgresql;
using snowcoreBlog.Backend.IAM.Entities;

namespace Marten.Generated.DocumentStorage
{
    // START: UpsertApplicationAdminOperation1940053739
    public class UpsertApplicationAdminOperation1940053739 : Marten.Internal.Operations.StorageOperation<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public UpsertApplicationAdminOperation1940053739(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
        {
            _document = document;
            _id = id;
            _versions = versions;
            _mapping = mapping;
        }



        public override void Postprocess(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions)
        {
            storeVersion();
        }


        public override System.Threading.Tasks.Task PostprocessAsync(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions, System.Threading.CancellationToken token)
        {
            storeVersion();
            // Nothing
            return System.Threading.Tasks.Task.CompletedTask;
        }


        public override Marten.Internal.Operations.OperationRole Role()
        {
            return Marten.Internal.Operations.OperationRole.Upsert;
        }


        public override NpgsqlTypes.NpgsqlDbType DbType()
        {
            return NpgsqlTypes.NpgsqlDbType.Text;
        }


        public override void ConfigureParameters(Weasel.Postgresql.IGroupedParameterBuilder parameterBuilder, Weasel.Postgresql.ICommandBuilder builder, snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session)
        {
            builder.Append("select public.mt_upsert_applicationadmin(");
            var parameter0 = parameterBuilder.AppendParameter(session.Serializer.ToJson(_document));
            parameter0.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            // .Net Class Type
            var parameter1 = parameterBuilder.AppendParameter(_document.GetType().FullName);
            parameter1.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;

            if (document.Id != null)
            {
                var parameter2 = parameterBuilder.AppendParameter(document.Id);
                parameter2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            }

            else
            {
                var parameter2 = parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            }

            setVersionParameter(parameterBuilder);
            builder.Append(')');
        }

    }

    // END: UpsertApplicationAdminOperation1940053739
    
    
    // START: InsertApplicationAdminOperation1940053739
    public class InsertApplicationAdminOperation1940053739 : Marten.Internal.Operations.StorageOperation<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public InsertApplicationAdminOperation1940053739(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
        {
            _document = document;
            _id = id;
            _versions = versions;
            _mapping = mapping;
        }



        public override void Postprocess(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions)
        {
            storeVersion();
        }


        public override System.Threading.Tasks.Task PostprocessAsync(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions, System.Threading.CancellationToken token)
        {
            storeVersion();
            // Nothing
            return System.Threading.Tasks.Task.CompletedTask;
        }


        public override Marten.Internal.Operations.OperationRole Role()
        {
            return Marten.Internal.Operations.OperationRole.Insert;
        }


        public override NpgsqlTypes.NpgsqlDbType DbType()
        {
            return NpgsqlTypes.NpgsqlDbType.Text;
        }


        public override void ConfigureParameters(Weasel.Postgresql.IGroupedParameterBuilder parameterBuilder, Weasel.Postgresql.ICommandBuilder builder, snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session)
        {
            builder.Append("select public.mt_insert_applicationadmin(");
            var parameter0 = parameterBuilder.AppendParameter(session.Serializer.ToJson(_document));
            parameter0.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            // .Net Class Type
            var parameter1 = parameterBuilder.AppendParameter(_document.GetType().FullName);
            parameter1.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;

            if (document.Id != null)
            {
                var parameter2 = parameterBuilder.AppendParameter(document.Id);
                parameter2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            }

            else
            {
                var parameter2 = parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            }

            setVersionParameter(parameterBuilder);
            builder.Append(')');
        }

    }

    // END: InsertApplicationAdminOperation1940053739
    
    
    // START: UpdateApplicationAdminOperation1940053739
    public class UpdateApplicationAdminOperation1940053739 : Marten.Internal.Operations.StorageOperation<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public UpdateApplicationAdminOperation1940053739(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
        {
            _document = document;
            _id = id;
            _versions = versions;
            _mapping = mapping;
        }



        public override void Postprocess(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions)
        {
            storeVersion();
            postprocessUpdate(reader, exceptions);
        }


        public override async System.Threading.Tasks.Task PostprocessAsync(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions, System.Threading.CancellationToken token)
        {
            storeVersion();
            await postprocessUpdateAsync(reader, exceptions, token);
        }


        public override Marten.Internal.Operations.OperationRole Role()
        {
            return Marten.Internal.Operations.OperationRole.Update;
        }


        public override NpgsqlTypes.NpgsqlDbType DbType()
        {
            return NpgsqlTypes.NpgsqlDbType.Text;
        }


        public override void ConfigureParameters(Weasel.Postgresql.IGroupedParameterBuilder parameterBuilder, Weasel.Postgresql.ICommandBuilder builder, snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session)
        {
            builder.Append("select public.mt_update_applicationadmin(");
            var parameter0 = parameterBuilder.AppendParameter(session.Serializer.ToJson(_document));
            parameter0.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            // .Net Class Type
            var parameter1 = parameterBuilder.AppendParameter(_document.GetType().FullName);
            parameter1.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;

            if (document.Id != null)
            {
                var parameter2 = parameterBuilder.AppendParameter(document.Id);
                parameter2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            }

            else
            {
                var parameter2 = parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            }

            setVersionParameter(parameterBuilder);
            builder.Append(')');
        }

    }

    // END: UpdateApplicationAdminOperation1940053739
    
    
    // START: QueryOnlyApplicationAdminSelector1940053739
    public class QueryOnlyApplicationAdminSelector1940053739 : Marten.Internal.CodeGeneration.DocumentSelectorWithOnlySerializer, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public QueryOnlyApplicationAdminSelector1940053739(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin Resolve(System.Data.Common.DbDataReader reader)
        {

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 0);
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 0, token).ConfigureAwait(false);
            return document;
        }

    }

    // END: QueryOnlyApplicationAdminSelector1940053739
    
    
    // START: LightweightApplicationAdminSelector1940053739
    public class LightweightApplicationAdminSelector1940053739 : Marten.Internal.CodeGeneration.DocumentSelectorWithVersions<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public LightweightApplicationAdminSelector1940053739(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            return document;
        }

    }

    // END: LightweightApplicationAdminSelector1940053739
    
    
    // START: IdentityMapApplicationAdminSelector1940053739
    public class IdentityMapApplicationAdminSelector1940053739 : Marten.Internal.CodeGeneration.DocumentSelectorWithIdentityMap<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public IdentityMapApplicationAdminSelector1940053739(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            return document;
        }

    }

    // END: IdentityMapApplicationAdminSelector1940053739
    
    
    // START: DirtyTrackingApplicationAdminSelector1940053739
    public class DirtyTrackingApplicationAdminSelector1940053739 : Marten.Internal.CodeGeneration.DocumentSelectorWithDirtyChecking<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public DirtyTrackingApplicationAdminSelector1940053739(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            StoreTracker(_session, document);
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            StoreTracker(_session, document);
            return document;
        }

    }

    // END: DirtyTrackingApplicationAdminSelector1940053739
    
    
    // START: QueryOnlyApplicationAdminDocumentStorage1940053739
    public class QueryOnlyApplicationAdminDocumentStorage1940053739 : Marten.Internal.Storage.QueryOnlyDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public QueryOnlyApplicationAdminDocumentStorage1940053739(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.QueryOnlyApplicationAdminSelector1940053739(session, _document);
        }


        public override object RawIdentityValue(string id)
        {
            return id;
        }


        public override Npgsql.NpgsqlParameter BuildManyIdParameter(System.String[] ids)
        {
            return base.BuildManyIdParameter(ids);
        }

    }

    // END: QueryOnlyApplicationAdminDocumentStorage1940053739
    
    
    // START: LightweightApplicationAdminDocumentStorage1940053739
    public class LightweightApplicationAdminDocumentStorage1940053739 : Marten.Internal.Storage.LightweightDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public LightweightApplicationAdminDocumentStorage1940053739(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.LightweightApplicationAdminSelector1940053739(session, _document);
        }


        public override object RawIdentityValue(string id)
        {
            return id;
        }


        public override Npgsql.NpgsqlParameter BuildManyIdParameter(System.String[] ids)
        {
            return base.BuildManyIdParameter(ids);
        }

    }

    // END: LightweightApplicationAdminDocumentStorage1940053739
    
    
    // START: IdentityMapApplicationAdminDocumentStorage1940053739
    public class IdentityMapApplicationAdminDocumentStorage1940053739 : Marten.Internal.Storage.IdentityMapDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public IdentityMapApplicationAdminDocumentStorage1940053739(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.IdentityMapApplicationAdminSelector1940053739(session, _document);
        }


        public override object RawIdentityValue(string id)
        {
            return id;
        }


        public override Npgsql.NpgsqlParameter BuildManyIdParameter(System.String[] ids)
        {
            return base.BuildManyIdParameter(ids);
        }

    }

    // END: IdentityMapApplicationAdminDocumentStorage1940053739
    
    
    // START: DirtyTrackingApplicationAdminDocumentStorage1940053739
    public class DirtyTrackingApplicationAdminDocumentStorage1940053739 : Marten.Internal.Storage.DirtyCheckedDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public DirtyTrackingApplicationAdminDocumentStorage1940053739(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationAdminOperation1940053739
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.DirtyTrackingApplicationAdminSelector1940053739(session, _document);
        }


        public override object RawIdentityValue(string id)
        {
            return id;
        }


        public override Npgsql.NpgsqlParameter BuildManyIdParameter(System.String[] ids)
        {
            return base.BuildManyIdParameter(ids);
        }

    }

    // END: DirtyTrackingApplicationAdminDocumentStorage1940053739
    
    
    // START: ApplicationAdminBulkLoader1940053739
    public class ApplicationAdminBulkLoader1940053739 : Marten.Internal.CodeGeneration.BulkLoader<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string>
    {
        private readonly Marten.Internal.Storage.IDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string> _storage;

        public ApplicationAdminBulkLoader1940053739(Marten.Internal.Storage.IDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin, string> storage) : base(storage)
        {
            _storage = storage;
        }


        public const string MAIN_LOADER_SQL = "COPY public.mt_doc_applicationadmin(\"mt_dotnet_type\", \"id\", \"mt_version\", \"data\") FROM STDIN BINARY";

        public const string TEMP_LOADER_SQL = "COPY mt_doc_applicationadmin_temp(\"mt_dotnet_type\", \"id\", \"mt_version\", \"data\") FROM STDIN BINARY";

        public const string COPY_NEW_DOCUMENTS_SQL = "insert into public.mt_doc_applicationadmin (\"id\", \"data\", \"mt_version\", \"mt_dotnet_type\", \"mt_deleted\", \"mt_deleted_at\", mt_last_modified) (select mt_doc_applicationadmin_temp.\"id\", mt_doc_applicationadmin_temp.\"data\", mt_doc_applicationadmin_temp.\"mt_version\", mt_doc_applicationadmin_temp.\"mt_dotnet_type\", mt_doc_applicationadmin_temp.\"mt_deleted\", mt_doc_applicationadmin_temp.\"mt_deleted_at\", transaction_timestamp() from mt_doc_applicationadmin_temp left join public.mt_doc_applicationadmin on mt_doc_applicationadmin_temp.id = public.mt_doc_applicationadmin.id where public.mt_doc_applicationadmin.id is null)";

        public const string OVERWRITE_SQL = "update public.mt_doc_applicationadmin target SET data = source.data, mt_version = source.mt_version, mt_dotnet_type = source.mt_dotnet_type, mt_deleted = source.mt_deleted, mt_deleted_at = source.mt_deleted_at, mt_last_modified = transaction_timestamp() FROM mt_doc_applicationadmin_temp source WHERE source.id = target.id";

        public const string CREATE_TEMP_TABLE_FOR_COPYING_SQL = "create temporary table mt_doc_applicationadmin_temp (like public.mt_doc_applicationadmin including defaults)";


        public override string CreateTempTableForCopying()
        {
            return CREATE_TEMP_TABLE_FOR_COPYING_SQL;
        }


        public override string CopyNewDocumentsFromTempTable()
        {
            return COPY_NEW_DOCUMENTS_SQL;
        }


        public override string OverwriteDuplicatesFromTempTable()
        {
            return OVERWRITE_SQL;
        }


        public override void LoadRow(Npgsql.NpgsqlBinaryImporter writer, snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Storage.Tenant tenant, Marten.ISerializer serializer)
        {
            writer.Write(document.GetType().FullName, NpgsqlTypes.NpgsqlDbType.Varchar);
            writer.Write(document.Id, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(Marten.Schema.Identity.CombGuidIdGeneration.NewGuid(), NpgsqlTypes.NpgsqlDbType.Uuid);
            writer.Write(serializer.ToJson(document), NpgsqlTypes.NpgsqlDbType.Jsonb);
        }


        public override async System.Threading.Tasks.Task LoadRowAsync(Npgsql.NpgsqlBinaryImporter writer, snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin document, Marten.Storage.Tenant tenant, Marten.ISerializer serializer, System.Threading.CancellationToken cancellation)
        {
            await writer.WriteAsync(document.GetType().FullName, NpgsqlTypes.NpgsqlDbType.Varchar, cancellation);
            await writer.WriteAsync(document.Id, NpgsqlTypes.NpgsqlDbType.Text, cancellation);
            await writer.WriteAsync(Marten.Schema.Identity.CombGuidIdGeneration.NewGuid(), NpgsqlTypes.NpgsqlDbType.Uuid, cancellation);
            await writer.WriteAsync(serializer.ToJson(document), NpgsqlTypes.NpgsqlDbType.Jsonb, cancellation);
        }


        public override string MainLoaderSql()
        {
            return MAIN_LOADER_SQL;
        }


        public override string TempLoaderSql()
        {
            return TEMP_LOADER_SQL;
        }

    }

    // END: ApplicationAdminBulkLoader1940053739
    
    
    // START: ApplicationAdminProvider1940053739
    public class ApplicationAdminProvider1940053739 : Marten.Internal.Storage.DocumentProvider<snowcoreBlog.Backend.IAM.Entities.ApplicationAdmin>
    {
        private readonly Marten.Schema.DocumentMapping _mapping;

        public ApplicationAdminProvider1940053739(Marten.Schema.DocumentMapping mapping) : base(new ApplicationAdminBulkLoader1940053739(new QueryOnlyApplicationAdminDocumentStorage1940053739(mapping)), new QueryOnlyApplicationAdminDocumentStorage1940053739(mapping), new LightweightApplicationAdminDocumentStorage1940053739(mapping), new IdentityMapApplicationAdminDocumentStorage1940053739(mapping), new DirtyTrackingApplicationAdminDocumentStorage1940053739(mapping))
        {
            _mapping = mapping;
        }


    }

    // END: ApplicationAdminProvider1940053739
    
    
}
