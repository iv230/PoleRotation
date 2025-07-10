using System;
using System.Numerics;
using PoleRotation.Model;

namespace PoleRotation.Service;

public class SnappingService(Configuration configuration)
{
    public Snapping? Selected;

    public void Select(Snapping? snapping)
    {
        Selected = snapping;
    }

    public void NewSnapping(string? name)
    {
        var distance = GetSnappingDistance();
        var snapping = new Snapping
        {
            Name = name,
            Distance = distance
        };

        PoleRotation.Log.Debug($"Computed new {snapping.Name} snapping at {snapping.Distance}");
        SaveSnapping(snapping);
    }

    public static float GetSnappingDistance()
    {
        var playerPos = PlayerService.GetPlayerPosition();
        var objectPos = HousingService.GetClosestItemPosition(197799u);

        return (objectPos - playerPos).Length();
    }

    public float GetDelta()
    {
        var currentPlayerSnapping = GetSnappingDistance();
        var targetSnapping = configuration.Snappings[^1]?.Distance ?? 0;
        
        return currentPlayerSnapping - targetSnapping;
    }

    public void SaveSnapping(Snapping? snapping)
    {
        configuration.Snappings.Add(snapping);
        configuration.Save();
    }
}
