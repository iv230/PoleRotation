using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.Sheets;
using PoleRotation.Service;

namespace PoleRotation.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly PoleRotation poleRotation;

    public MainWindow(PoleRotation poleRotation)
        : base("Pole Animation##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.poleRotation = poleRotation;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text("Select a Snapping:");

        if (poleRotation.Configuration.Snappings.Count > 0)
        {
            var snappingNames = poleRotation.Configuration.Snappings.Select(s => s?.Name).ToArray();
            var selectedIndex = poleRotation.Configuration.Snappings.IndexOf(poleRotation.SnappingService.Selected);

            if (ImGui.ListBox("##SnappingList", ref selectedIndex, snappingNames, snappingNames.Length))
            {
                poleRotation.SnappingService.Select(poleRotation.Configuration.Snappings[selectedIndex]);
            }
        }
        else
        {
            ImGui.Text("No snappings available.");
        }
        
        ImGui.Separator();
        if (ImGui.Button("Create"))
        {
            poleRotation.ToggleCreateUi();
        }
    }
}
