using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Math;
using PoleRotation.Model;
using HousingFurniture = Lumina.Excel.Sheets.HousingFurniture;

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

    public static List<FurnitureItem> GetAllHousingObjects()
    {
        var sheet = PoleRotation.DataManager.GetExcelSheet<HousingFurniture>()!
            .Select(row => new FurnitureItem
            {
                Id = row.RowId,
                Name = row.Item.Value!.Name.ToString()
            })
            .ToList();
        return sheet.ToList();
    }
}
