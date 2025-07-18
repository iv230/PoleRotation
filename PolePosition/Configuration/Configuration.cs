using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using PolePosition.Model;

namespace PolePosition;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public List<Snapping?> Snappings { get; set; } = [];
    public bool DisplayDistance { get; set; }
    public string PenumbraCollection { get; set; } = "Default";

    public void Save()
    {
        PolePosition.PluginInterface.SavePluginConfig(this);
        PolePosition.Log.Information("Configuration saved.");
    }
}
