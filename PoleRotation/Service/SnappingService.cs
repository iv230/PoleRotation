using PoleRotation.Model;

namespace PoleRotation.Service;

public class SnappingService(Configuration.Configuration configuration)
{
    public Snapping? Selected;

    public void Select(Snapping? snapping)
    {
        PoleRotation.Log.Information($"Selected snapping {snapping?.Name}");
        PoleRotation.Log.Verbose($"{snapping?.ToJson()}");

        Selected = snapping;
    }

    public static float GetSnappingDistance(uint housingItemId)
    {
        var playerPos = PlayerService.GetPlayerPosition();
        var objectPos = HousingService.GetClosestItemPosition(housingItemId);

        return (objectPos - playerPos).Length();
    }

    public void SaveSnapping(Snapping snapping)
    {
        PoleRotation.Log.Information($"Saving snapping {snapping.Name}");
        PoleRotation.Log.Verbose($"{snapping.ToJson()}");

        configuration.Snappings.Add(snapping);
        configuration.Save();
    }
}
