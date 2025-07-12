using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Math;

namespace PoleRotation.Service;

public static class HousingService
{
    public static unsafe System.Numerics.Vector3 GetClosestItemPosition(uint itemBaseId)
    {
        var allFurniture = HousingManager.Instance()->IndoorTerritory->HousingObjectManager.Objects;
        var currentCharacter = PlayerService.GetCurrentPlayer();
    
        Vector3 closestPosition = new(float.MaxValue, float.MaxValue, float.MaxValue);
        var minDistanceSqr = float.MaxValue;
    
        foreach (var housingFurniturePtr in allFurniture)
        {
            var housingFurniture = housingFurniturePtr.Value;
            if (housingFurniture == null || housingFurniture->BaseId != itemBaseId)
                continue;

            var distanceSqr = (currentCharacter->Position - housingFurniture->Position).SqrMagnitude;

            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                closestPosition = housingFurniture->Position;
            }
        }

        return closestPosition;
    }
}
