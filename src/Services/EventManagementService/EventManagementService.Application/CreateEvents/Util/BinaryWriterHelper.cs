using Npgsql;
using NpgsqlTypes;

namespace EventManagementService.Application.CreateEvents.Util;

internal static class BinaryWriterHelper<T>
{
    internal static async Task WriteNullableAsync(NpgsqlBinaryImporter writer, T? value, NpgsqlDbType type)
    {
        if (value != null) await writer.WriteAsync(value);
        else await writer.WriteNullAsync();
    }
}