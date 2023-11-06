using System.Text.Json;

namespace OrderService.Extensions;

public static class StreamExtensions
{
    public static Stream ToStream<T>(this T instance)
    {
        var serialized = JsonSerializer.Serialize(instance);
        var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        writer.Write(serialized);
        writer.Flush();
        memoryStream.Position = 0;

        return memoryStream;
    }
}