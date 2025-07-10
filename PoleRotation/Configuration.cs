using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using PoleRotation.Model;

namespace PoleRotation;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public List<Snapping?> Snappings { get; set; } = [];

    public void Save()
    {
        PoleRotation.PluginInterface.SavePluginConfig(this);
        PoleRotation.Log.Information("Configuration saved.");
    }
}
