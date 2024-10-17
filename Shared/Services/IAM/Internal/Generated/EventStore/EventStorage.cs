// <auto-generated/>
#pragma warning disable
using Marten;
using Marten.Events;
using System;

namespace Marten.Generated.EventStore
{
    // START: GeneratedEventDocumentStorage
    public class GeneratedEventDocumentStorage : Marten.Events.EventDocumentStorage
    {
        private readonly Marten.StoreOptions _options;

        public GeneratedEventDocumentStorage(Marten.StoreOptions options) : base(options)
        {
            _options = options;
        }



        public override Marten.Internal.Operations.IStorageOperation AppendEvent(Marten.Events.EventGraph events, Marten.Internal.IMartenSession session, Marten.Events.StreamAction stream, Marten.Events.IEvent e)
        {
            return new Marten.Generated.EventStore.AppendEventOperation(stream, e);
        }


        public override Marten.Internal.Operations.IStorageOperation InsertStream(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedInsertStream(stream);
        }


        public override Marten.Linq.QueryHandlers.IQueryHandler<Marten.Events.StreamState> QueryForStream(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedStreamStateQueryHandler(stream.Id);
        }


        public override Marten.Internal.Operations.IStorageOperation UpdateStreamVersion(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedStreamVersionOperation(stream);
        }


        public override void ApplyReaderDataToEvent(System.Data.Common.DbDataReader reader, Marten.Events.IEvent e)
        {
            if (!reader.IsDBNull(3))
            {
            var sequence = reader.GetFieldValue<long>(3);
            e.Sequence = sequence;
            }
            if (!reader.IsDBNull(4))
            {
            var id = reader.GetFieldValue<System.Guid>(4);
            e.Id = id;
            }
            var streamId = reader.GetFieldValue<System.Guid>(5);
            e.StreamId = streamId;
            if (!reader.IsDBNull(6))
            {
            var version = reader.GetFieldValue<long>(6);
            e.Version = version;
            }
            if (!reader.IsDBNull(7))
            {
            var timestamp = reader.GetFieldValue<System.DateTimeOffset>(7);
            e.Timestamp = timestamp;
            }
            if (!reader.IsDBNull(8))
            {
            var tenantId = reader.GetFieldValue<string>(8);
            e.TenantId = tenantId;
            }
            var isArchived = reader.GetFieldValue<bool>(9);
            e.IsArchived = isArchived;
        }


        public override async System.Threading.Tasks.Task ApplyReaderDataToEventAsync(System.Data.Common.DbDataReader reader, Marten.Events.IEvent e, System.Threading.CancellationToken token)
        {
            if (!(await reader.IsDBNullAsync(3, token).ConfigureAwait(false)))
            {
            var sequence = await reader.GetFieldValueAsync<long>(3, token).ConfigureAwait(false);
            e.Sequence = sequence;
            }
            if (!(await reader.IsDBNullAsync(4, token).ConfigureAwait(false)))
            {
            var id = await reader.GetFieldValueAsync<System.Guid>(4, token).ConfigureAwait(false);
            e.Id = id;
            }
            var streamId = await reader.GetFieldValueAsync<System.Guid>(5, token).ConfigureAwait(false);
            e.StreamId = streamId;
            if (!(await reader.IsDBNullAsync(6, token).ConfigureAwait(false)))
            {
            var version = await reader.GetFieldValueAsync<long>(6, token).ConfigureAwait(false);
            e.Version = version;
            }
            if (!(await reader.IsDBNullAsync(7, token).ConfigureAwait(false)))
            {
            var timestamp = await reader.GetFieldValueAsync<System.DateTimeOffset>(7, token).ConfigureAwait(false);
            e.Timestamp = timestamp;
            }
            if (!(await reader.IsDBNullAsync(8, token).ConfigureAwait(false)))
            {
            var tenantId = await reader.GetFieldValueAsync<string>(8, token).ConfigureAwait(false);
            e.TenantId = tenantId;
            }
            var isArchived = await reader.GetFieldValueAsync<bool>(9, token).ConfigureAwait(false);
            e.IsArchived = isArchived;
        }


        public override Marten.Internal.Operations.IStorageOperation QuickAppendEventWithVersion(Marten.Events.EventGraph events, Marten.Internal.IMartenSession session, Marten.Events.StreamAction stream, Marten.Events.IEvent e)
        {
            return new Marten.Generated.EventStore.AppendEventOperationQuickWithVersion(stream, e);
        }


        public override Marten.Internal.Operations.IStorageOperation QuickAppendEvents(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.QuickAppendEventsOperation(stream);
        }

    }

    // END: GeneratedEventDocumentStorage
    
    
    // START: AppendEventOperation
    public class AppendEventOperation : Marten.Events.Operations.AppendEventOperationBase
    {
        private readonly Marten.Events.StreamAction _stream;
        private readonly Marten.Events.IEvent _e;

        public AppendEventOperation(Marten.Events.StreamAction stream, Marten.Events.IEvent e) : base(stream, e)
        {
            _stream = stream;
            _e = e;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            builder.Append("insert into public.mt_events (data, type, mt_dotnet_type, id, stream_id, version, timestamp, tenant_id, seq_id) values (");
            var parameterBuilder = builder.CreateGroupedParameterBuilder(',');
            var parameter0 = parameterBuilder.AppendParameter(session.Serializer.ToJson(Event.Data));
            parameter0.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            var parameter1 = Event.EventTypeName != null ? parameterBuilder.AppendParameter(Event.EventTypeName) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter1.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            var parameter2 = Event.DotNetTypeName != null ? parameterBuilder.AppendParameter(Event.DotNetTypeName) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            var parameter3 = parameterBuilder.AppendParameter(Event.Id);
            parameter3.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            var parameter4 = parameterBuilder.AppendParameter(Stream.Id);
            parameter4.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            var parameter5 = parameterBuilder.AppendParameter(Event.Version);
            parameter5.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            var parameter6 = parameterBuilder.AppendParameter(Event.Timestamp);
            parameter6.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.TimestampTz;
            var parameter7 = Stream.TenantId != null ? parameterBuilder.AppendParameter(Stream.TenantId) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter7.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            var parameter8 = parameterBuilder.AppendParameter(Event.Sequence);
            parameter8.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            builder.Append(')');
        }

    }

    // END: AppendEventOperation
    
    
    // START: AppendEventOperationQuickWithVersion
    public class AppendEventOperationQuickWithVersion : Marten.Events.Operations.AppendEventOperationBase
    {
        private readonly Marten.Events.StreamAction _stream;
        private readonly Marten.Events.IEvent _e;

        public AppendEventOperationQuickWithVersion(Marten.Events.StreamAction stream, Marten.Events.IEvent e) : base(stream, e)
        {
            _stream = stream;
            _e = e;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            builder.Append("insert into public.mt_events (data, type, mt_dotnet_type, id, stream_id, version, timestamp, tenant_id, seq_id) values (");
            var parameterBuilder = builder.CreateGroupedParameterBuilder(',');
            var parameter0 = parameterBuilder.AppendParameter(session.Serializer.ToJson(Event.Data));
            parameter0.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            var parameter1 = Event.EventTypeName != null ? parameterBuilder.AppendParameter(Event.EventTypeName) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter1.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            var parameter2 = Event.DotNetTypeName != null ? parameterBuilder.AppendParameter(Event.DotNetTypeName) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            var parameter3 = parameterBuilder.AppendParameter(Event.Id);
            parameter3.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            var parameter4 = parameterBuilder.AppendParameter(Stream.Id);
            parameter4.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            var parameter5 = parameterBuilder.AppendParameter(Event.Version);
            parameter5.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            var parameter6 = parameterBuilder.AppendParameter(Event.Timestamp);
            parameter6.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.TimestampTz;
            var parameter7 = Stream.TenantId != null ? parameterBuilder.AppendParameter(Stream.TenantId) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter7.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            builder.Append(",nextval('public.mt_events_sequence')");
            builder.Append(')');
        }

    }

    // END: AppendEventOperationQuickWithVersion
    
    
    // START: QuickAppendEventsOperation
    public class QuickAppendEventsOperation : Marten.Events.Operations.QuickAppendEventsOperationBase
    {
        private readonly Marten.Events.StreamAction _stream;

        public QuickAppendEventsOperation(Marten.Events.StreamAction stream) : base(stream)
        {
            _stream = stream;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            builder.Append("select public.mt_quick_append_events(");
            var parameterBuilder = builder.CreateGroupedParameterBuilder(',');
            writeId(parameterBuilder);
            writeBasicParameters(parameterBuilder, session);
            builder.Append(')');
        }

    }

    // END: QuickAppendEventsOperation
    
    
    // START: GeneratedInsertStream
    public class GeneratedInsertStream : Marten.Events.Operations.InsertStreamBase
    {
        private readonly Marten.Events.StreamAction _stream;

        public GeneratedInsertStream(Marten.Events.StreamAction stream) : base(stream)
        {
            _stream = stream;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            builder.Append("insert into public.mt_streams (id, type, version, tenant_id) values (");
            var parameterBuilder = builder.CreateGroupedParameterBuilder(',');
            var parameter0 = parameterBuilder.AppendParameter(Stream.Id);
            parameter0.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            var parameter1 = Stream.AggregateTypeName != null ? parameterBuilder.AppendParameter(Stream.AggregateTypeName) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter1.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            var parameter2 = parameterBuilder.AppendParameter(Stream.Version);
            parameter2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            var parameter3 = Stream.TenantId != null ? parameterBuilder.AppendParameter(Stream.TenantId) : parameterBuilder.AppendParameter<object>(System.DBNull.Value);
            parameter3.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            builder.Append(')');
        }

    }

    // END: GeneratedInsertStream
    
    
    // START: GeneratedStreamStateQueryHandler
    public class GeneratedStreamStateQueryHandler : Marten.Events.Querying.StreamStateQueryHandler
    {
        private readonly System.Guid _streamId;

        public GeneratedStreamStateQueryHandler(System.Guid streamId)
        {
            _streamId = streamId;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            builder.Append("select id, version, type, timestamp, created as timestamp, is_archived from public.mt_streams where id = ");
            var parameter1 = builder.AppendParameter(_streamId);
            parameter1.DbType = System.Data.DbType.Guid;
        }


        public override Marten.Events.StreamState Resolve(Marten.Internal.IMartenSession session, System.Data.Common.DbDataReader reader)
        {
            var streamState = new Marten.Events.StreamState();
            var id = reader.GetFieldValue<System.Guid>(0);
            streamState.Id = id;
            var version = reader.GetFieldValue<long>(1);
            streamState.Version = version;
            SetAggregateType(streamState, reader, session);
            var lastTimestamp = reader.GetFieldValue<System.DateTimeOffset>(3);
            streamState.LastTimestamp = lastTimestamp;
            var created = reader.GetFieldValue<System.DateTimeOffset>(4);
            streamState.Created = created;
            var isArchived = reader.GetFieldValue<bool>(5);
            streamState.IsArchived = isArchived;
            return streamState;
        }


        public override async System.Threading.Tasks.Task<Marten.Events.StreamState> ResolveAsync(Marten.Internal.IMartenSession session, System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var streamState = new Marten.Events.StreamState();
            var id = await reader.GetFieldValueAsync<System.Guid>(0, token).ConfigureAwait(false);
            streamState.Id = id;
            var version = await reader.GetFieldValueAsync<long>(1, token).ConfigureAwait(false);
            streamState.Version = version;
            await SetAggregateTypeAsync(streamState, reader, session, token).ConfigureAwait(false);
            var lastTimestamp = await reader.GetFieldValueAsync<System.DateTimeOffset>(3, token).ConfigureAwait(false);
            streamState.LastTimestamp = lastTimestamp;
            var created = await reader.GetFieldValueAsync<System.DateTimeOffset>(4, token).ConfigureAwait(false);
            streamState.Created = created;
            var isArchived = await reader.GetFieldValueAsync<bool>(5, token).ConfigureAwait(false);
            streamState.IsArchived = isArchived;
            return streamState;
        }

    }

    // END: GeneratedStreamStateQueryHandler
    
    
    // START: GeneratedStreamVersionOperation
    public class GeneratedStreamVersionOperation : Marten.Events.Operations.UpdateStreamVersion
    {
        private readonly Marten.Events.StreamAction _stream;

        public GeneratedStreamVersionOperation(Marten.Events.StreamAction stream) : base(stream)
        {
            _stream = stream;
        }



        public override void ConfigureCommand(Weasel.Postgresql.ICommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            builder.Append("update public.mt_streams ");
            var parameterBuilder = builder.CreateGroupedParameterBuilder();
            builder.Append("set version = ");
            var parameter0 = parameterBuilder.AppendParameter(Stream.Version);
            parameter0.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            builder.Append(" where id = ");
            var parameter1 = parameterBuilder.AppendParameter(Stream.Id);
            parameter1.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            builder.Append(" and version = ");
            var parameter2 = parameterBuilder.AppendParameter(Stream.ExpectedVersionOnServer);
            parameter2.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            builder.Append(" returning version");
        }

    }

    // END: GeneratedStreamVersionOperation
    
    
}

