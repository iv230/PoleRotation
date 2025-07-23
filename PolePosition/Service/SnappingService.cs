using PolePosition.Model;

namespace PolePosition.Service;

public class SnappingService(Configuration configuration, PenumbraService penumbraService)
{
    public Snapping? Selected;

    public void Select(Snapping? snapping)
    {
        PolePosition.Log.Information($"Selected snapping {snapping?.Name}");
        PolePosition.Log.Verbose($"{snapping?.ToJson()}");

        if (snapping != null)
        {
            if (Selected != null)
            {
                var oldMod = penumbraService.GetPenumbraMod(Selected.Mod);
                
                if (oldMod != null)
                    penumbraService.SetModState(oldMod, false);
                else
                    PolePosition.Log.Warning($"Mod {Selected.Mod} not found, could not disable.");
            }

            var newMod = penumbraService.GetPenumbraMod(snapping.Mod);
            
            if (newMod != null)
                penumbraService.SetModState(newMod, true);
            else
                PolePosition.Log.Warning($"Mod {snapping.Mod} not found, could not enable.");
        }

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

    public void DeleteSnapping(Snapping snapping)
    {
        PolePosition.Log.Information($"Deleting snapping {snapping.Name}");
        PolePosition.Log.Verbose($"{snapping.ToJson()}");
        
        configuration.Snappings.Remove(snapping);
        configuration.Save();
    }
}
