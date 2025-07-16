using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PoleRotation.Model.Penumbra;

namespace PoleRotation.Service;

public sealed class PenumbraService
{
    private static readonly HttpClient Client = new();

    public static async Task<List<PenumbraMod>?> LoadPenumbraModsAsync()
    {
        PoleRotation.Log.Info("Fetching Penumbra mods...");

        try
        {
            var response = await Client.GetAsync("http://localhost:42069/api/mods");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            PoleRotation.Log.Debug(json);

            var modsDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();

            var availableMods = modsDict.Select(kvp => new PenumbraMod
            {
                Name = kvp.Key,
                BasePath = kvp.Value
            }).ToList();

            return availableMods ?? [];
        }
        catch (Exception ex)
        {
            PoleRotation.Log.Error($"Error while fetching Penumbra mod list: {ex}");
            return [];
        }
    }
}
