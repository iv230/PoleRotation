using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Math;
using PolePosition.Model;
using HousingFurniture = Lumina.Excel.Sheets.HousingFurniture;

namespace PolePosition.Service;

public static class HousingService
{
    private static List<FurnitureItem>? Cache;

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

    public static string GetObjectName(uint itemBaseId)
    {
        if (Cache == null)
            FetchAllHousingObjects();

        return Cache!
               .Where(obj => obj.Id == itemBaseId)
               .Select(obj => obj.Name)
               .FirstOrDefault() ?? "Unknown";
    }

    public static List<FurnitureItem> GetAllHousingObjects()
    {
        return Cache ?? FetchAllHousingObjects();
    }

    private static List<FurnitureItem> FetchAllHousingObjects()
    {
        var sheet = PolePosition.DataManager.GetExcelSheet<HousingFurniture>()!
            .Select(row => new FurnitureItem
            {
                Id = row.RowId,
                Name = row.Item.Value!.Name.ToString()
            })
            .ToList();
        var list = sheet.ToList();
        Cache = list;
        return list;
    }
}
