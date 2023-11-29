using Npgsql;
using NpgsqlTypes;

namespace EventManagementService.Application.ProcessExternalEvents.Util;

internal static class BinaryWriterHelper<T>
{
    internal static async Task WriteNullableAsync(NpgsqlBinaryImporter writer, T? value, NpgsqlDbType type)
    {
        if (value != null) await writer.WriteAsync(value, type);
        else await writer.WriteNullAsync();
    }
    internal static async Task WriteNullableDatesAsync(NpgsqlBinaryImporter writer, DateTimeOffset? value)
    {
        if (value != null && value.HasValue && DateTimeOffset.MinValue != value) await writer.WriteAsync(value.Value, NpgsqlDbType.TimestampTz);
        else await writer.WriteNullAsync();
    }
}