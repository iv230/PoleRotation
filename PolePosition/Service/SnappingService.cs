using PolePosition.Model;

namespace PolePosition.Service;

public class SnappingService(Configuration.Configuration configuration)
{
    public Snapping? Selected;

    public void Select(Snapping? snapping)
    {
        PolePosition.Log.Information($"Selected snapping {snapping?.Name}");
        PolePosition.Log.Verbose($"{snapping?.ToJson()}");

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
        PolePosition.Log.Information($"Saving snapping {snapping.Name}");
        PolePosition.Log.Verbose($"{snapping.ToJson()}");

        configuration.Snappings.Add(snapping);
        configuration.Save();
    }
}
