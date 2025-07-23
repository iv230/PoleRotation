using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;
using PolePosition.Model.Penumbra;

namespace PolePosition.Service;

public sealed class PenumbraService(IDalamudPluginInterface pluginInterface)
{
    private readonly GetModList getMods = new(pluginInterface);
    private readonly GetCollection getCurrentCollection = new(pluginInterface);
    private readonly TrySetMod setMod = new(pluginInterface);
    private List<PenumbraMod> mods = [];

    public List<PenumbraMod> LoadPenumbraMods()
    {
        var foundMods = getMods.Invoke().Select(kvp => new PenumbraMod
        {
            Name = kvp.Key,
            BasePath = kvp.Value
        }).ToList();

        mods = foundMods;
        return foundMods;
    }

    public PenumbraMod? GetPenumbraMod(string name)
    {
        mods.ForEach(m => PolePosition.Log.Verbose($"{m.Name} - {m.BasePath}"));
        return mods.Find(m => m.BasePath == name);
    }

    public (Guid id, string name)? GetCurrentCollection()
    {
        try
        {
            return getCurrentCollection.Invoke(ApiCollectionType.Current);
        }
        catch (Exception ex)
        {
            PolePosition.Log.Error($"Failed to get current Penumbra collection: {ex}");
            return null;
        }
    }

    public void SetModState(PenumbraMod mod, bool enable)
    {
        PolePosition.Log.Information($"Set mod '{mod.Name}' to enable=${enable} in collection '{GetCurrentCollection()!.Value.name}'.");
        try
        {
            setMod.Invoke(GetCurrentCollection()!.Value.id, mod.BasePath, enable, mod.Name);
        }
        catch (Exception ex)
        {
            PolePosition.Log.Error($"Failed to enable mod '{mod}': {ex}");
        }
    }
}
