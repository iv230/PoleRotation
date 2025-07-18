using System.Text.Json;

namespace PolePosition.Model;

public class Snapping
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    public required string Name { get; init; }
    public required float Distance { get; init; }
    public required string Mod { get; init; }
    public required uint HousingItemId { get; init; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, JsonSerializerOptions);
    }
}
