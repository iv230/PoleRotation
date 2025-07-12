using PoleRotation.Model;

namespace PoleRotation.Service;

public class SnappingService(Configuration configuration)
{
    public Snapping? Selected;

    public void Select(Snapping? snapping)
    {
        Selected = snapping;
    }

    // public void NewSnapping(string? name)
    // {
    //     var distance = GetSnappingDistance();
    //     var snapping = new Snapping
    //     {
    //         Name = name,
    //         Distance = distance
    //     };
    //
    //     PoleRotation.Log.Debug($"Computed new {snapping.Name} snapping at {snapping.Distance}");
    //     SaveSnapping(snapping);
    // }

    public static float GetSnappingDistance(uint housingItemId)
    {
        var playerPos = PlayerService.GetPlayerPosition();
        var objectPos = HousingService.GetClosestItemPosition(housingItemId);

        return (objectPos - playerPos).Length();
    }

    public void SaveSnapping(Snapping? snapping)
    {
        configuration.Snappings.Add(snapping);
        configuration.Save();
    }
}
