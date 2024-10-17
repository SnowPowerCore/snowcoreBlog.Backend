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
    // START: UpsertApplicationUserOperation1908805755
    public class UpsertApplicationUserOperation1908805755 : Marten.Internal.Operations.StorageOperation<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly snowcoreBlog.Backend.IAM.Entities.ApplicationUser _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public UpsertApplicationUserOperation1908805755(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
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


        public override void ConfigureParameters(Weasel.Postgresql.IGroupedParameterBuilder parameterBuilder, Weasel.Postgresql.ICommandBuilder builder, snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session)
        {
            builder.Append("select public.mt_upsert_applicationuser(");
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

    // END: UpsertApplicationUserOperation1908805755
    
    
    // START: InsertApplicationUserOperation1908805755
    public class InsertApplicationUserOperation1908805755 : Marten.Internal.Operations.StorageOperation<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly snowcoreBlog.Backend.IAM.Entities.ApplicationUser _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public InsertApplicationUserOperation1908805755(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
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


        public override void ConfigureParameters(Weasel.Postgresql.IGroupedParameterBuilder parameterBuilder, Weasel.Postgresql.ICommandBuilder builder, snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session)
        {
            builder.Append("select public.mt_insert_applicationuser(");
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

    // END: InsertApplicationUserOperation1908805755
    
    
    // START: UpdateApplicationUserOperation1908805755
    public class UpdateApplicationUserOperation1908805755 : Marten.Internal.Operations.StorageOperation<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly snowcoreBlog.Backend.IAM.Entities.ApplicationUser _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public UpdateApplicationUserOperation1908805755(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
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


        public override void ConfigureParameters(Weasel.Postgresql.IGroupedParameterBuilder parameterBuilder, Weasel.Postgresql.ICommandBuilder builder, snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session)
        {
            builder.Append("select public.mt_update_applicationuser(");
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

    // END: UpdateApplicationUserOperation1908805755
    
    
    // START: QueryOnlyApplicationUserSelector1908805755
    public class QueryOnlyApplicationUserSelector1908805755 : Marten.Internal.CodeGeneration.DocumentSelectorWithOnlySerializer, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public QueryOnlyApplicationUserSelector1908805755(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationUser Resolve(System.Data.Common.DbDataReader reader)
        {

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 0);
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationUser> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 0, token).ConfigureAwait(false);
            return document;
        }

    }

    // END: QueryOnlyApplicationUserSelector1908805755
    
    
    // START: LightweightApplicationUserSelector1908805755
    public class LightweightApplicationUserSelector1908805755 : Marten.Internal.CodeGeneration.DocumentSelectorWithVersions<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public LightweightApplicationUserSelector1908805755(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationUser Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationUser> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            return document;
        }

    }

    // END: LightweightApplicationUserSelector1908805755
    
    
    // START: IdentityMapApplicationUserSelector1908805755
    public class IdentityMapApplicationUserSelector1908805755 : Marten.Internal.CodeGeneration.DocumentSelectorWithIdentityMap<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public IdentityMapApplicationUserSelector1908805755(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationUser Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationUser> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            return document;
        }

    }

    // END: IdentityMapApplicationUserSelector1908805755
    
    
    // START: DirtyTrackingApplicationUserSelector1908805755
    public class DirtyTrackingApplicationUserSelector1908805755 : Marten.Internal.CodeGeneration.DocumentSelectorWithDirtyChecking<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>, Marten.Linq.Selectors.ISelector<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public DirtyTrackingApplicationUserSelector1908805755(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public snowcoreBlog.Backend.IAM.Entities.ApplicationUser Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = _serializer.FromJson<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            StoreTracker(_session, document);
            return document;
        }


        public async System.Threading.Tasks.Task<snowcoreBlog.Backend.IAM.Entities.ApplicationUser> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            snowcoreBlog.Backend.IAM.Entities.ApplicationUser document;
            document = await _serializer.FromJsonAsync<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            StoreTracker(_session, document);
            return document;
        }

    }

    // END: DirtyTrackingApplicationUserSelector1908805755
    
    
    // START: QueryOnlyApplicationUserDocumentStorage1908805755
    public class QueryOnlyApplicationUserDocumentStorage1908805755 : Marten.Internal.Storage.QueryOnlyDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public QueryOnlyApplicationUserDocumentStorage1908805755(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.QueryOnlyApplicationUserSelector1908805755(session, _document);
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

    // END: QueryOnlyApplicationUserDocumentStorage1908805755
    
    
    // START: LightweightApplicationUserDocumentStorage1908805755
    public class LightweightApplicationUserDocumentStorage1908805755 : Marten.Internal.Storage.LightweightDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public LightweightApplicationUserDocumentStorage1908805755(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.LightweightApplicationUserSelector1908805755(session, _document);
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

    // END: LightweightApplicationUserDocumentStorage1908805755
    
    
    // START: IdentityMapApplicationUserDocumentStorage1908805755
    public class IdentityMapApplicationUserDocumentStorage1908805755 : Marten.Internal.Storage.IdentityMapDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public IdentityMapApplicationUserDocumentStorage1908805755(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.IdentityMapApplicationUserSelector1908805755(session, _document);
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

    // END: IdentityMapApplicationUserDocumentStorage1908805755
    
    
    // START: DirtyTrackingApplicationUserDocumentStorage1908805755
    public class DirtyTrackingApplicationUserDocumentStorage1908805755 : Marten.Internal.Storage.DirtyCheckedDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public DirtyTrackingApplicationUserDocumentStorage1908805755(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.Id)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.Id;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertApplicationUserOperation1908805755
            (
                document, Identity(document),
                session.Versions.ForType<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(snowcoreBlog.Backend.IAM.Entities.ApplicationUser document)
        {
            return document.Id;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.DirtyTrackingApplicationUserSelector1908805755(session, _document);
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

    // END: DirtyTrackingApplicationUserDocumentStorage1908805755
    
    
    // START: ApplicationUserBulkLoader1908805755
    public class ApplicationUserBulkLoader1908805755 : Marten.Internal.CodeGeneration.BulkLoader<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string>
    {
        private readonly Marten.Internal.Storage.IDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string> _storage;

        public ApplicationUserBulkLoader1908805755(Marten.Internal.Storage.IDocumentStorage<snowcoreBlog.Backend.IAM.Entities.ApplicationUser, string> storage) : base(storage)
        {
            _storage = storage;
        }


        public const string MAIN_LOADER_SQL = "COPY public.mt_doc_applicationuser(\"mt_dotnet_type\", \"id\", \"mt_version\", \"data\") FROM STDIN BINARY";

        public const string TEMP_LOADER_SQL = "COPY mt_doc_applicationuser_temp(\"mt_dotnet_type\", \"id\", \"mt_version\", \"data\") FROM STDIN BINARY";

        public const string COPY_NEW_DOCUMENTS_SQL = "insert into public.mt_doc_applicationuser (\"id\", \"data\", \"mt_version\", \"mt_dotnet_type\", \"mt_deleted\", \"mt_deleted_at\", mt_last_modified) (select mt_doc_applicationuser_temp.\"id\", mt_doc_applicationuser_temp.\"data\", mt_doc_applicationuser_temp.\"mt_version\", mt_doc_applicationuser_temp.\"mt_dotnet_type\", mt_doc_applicationuser_temp.\"mt_deleted\", mt_doc_applicationuser_temp.\"mt_deleted_at\", transaction_timestamp() from mt_doc_applicationuser_temp left join public.mt_doc_applicationuser on mt_doc_applicationuser_temp.id = public.mt_doc_applicationuser.id where public.mt_doc_applicationuser.id is null)";

        public const string OVERWRITE_SQL = "update public.mt_doc_applicationuser target SET data = source.data, mt_version = source.mt_version, mt_dotnet_type = source.mt_dotnet_type, mt_deleted = source.mt_deleted, mt_deleted_at = source.mt_deleted_at, mt_last_modified = transaction_timestamp() FROM mt_doc_applicationuser_temp source WHERE source.id = target.id";

        public const string CREATE_TEMP_TABLE_FOR_COPYING_SQL = "create temporary table mt_doc_applicationuser_temp (like public.mt_doc_applicationuser including defaults)";


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


        public override void LoadRow(Npgsql.NpgsqlBinaryImporter writer, snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Storage.Tenant tenant, Marten.ISerializer serializer)
        {
            writer.Write(document.GetType().FullName, NpgsqlTypes.NpgsqlDbType.Varchar);
            writer.Write(document.Id, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(Marten.Schema.Identity.CombGuidIdGeneration.NewGuid(), NpgsqlTypes.NpgsqlDbType.Uuid);
            writer.Write(serializer.ToJson(document), NpgsqlTypes.NpgsqlDbType.Jsonb);
        }


        public override async System.Threading.Tasks.Task LoadRowAsync(Npgsql.NpgsqlBinaryImporter writer, snowcoreBlog.Backend.IAM.Entities.ApplicationUser document, Marten.Storage.Tenant tenant, Marten.ISerializer serializer, System.Threading.CancellationToken cancellation)
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

    // END: ApplicationUserBulkLoader1908805755
    
    
    // START: ApplicationUserProvider1908805755
    public class ApplicationUserProvider1908805755 : Marten.Internal.Storage.DocumentProvider<snowcoreBlog.Backend.IAM.Entities.ApplicationUser>
    {
        private readonly Marten.Schema.DocumentMapping _mapping;

        public ApplicationUserProvider1908805755(Marten.Schema.DocumentMapping mapping) : base(new ApplicationUserBulkLoader1908805755(new QueryOnlyApplicationUserDocumentStorage1908805755(mapping)), new QueryOnlyApplicationUserDocumentStorage1908805755(mapping), new LightweightApplicationUserDocumentStorage1908805755(mapping), new IdentityMapApplicationUserDocumentStorage1908805755(mapping), new DirtyTrackingApplicationUserDocumentStorage1908805755(mapping))
        {
            _mapping = mapping;
        }


    }

    // END: ApplicationUserProvider1908805755
    
    
}

