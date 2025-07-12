using System.Numerics;

namespace PoleRotation.Model;

public class Snapping
{
    public required string Name { get; init; }
    public required float Distance { get; init; }
    public required string Mod { get; init; }
    public required uint HousingItemId { get; init; }
}
