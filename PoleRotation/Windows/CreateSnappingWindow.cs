using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PoleRotation.Model;
using PoleRotation.Service;

namespace PoleRotation.Windows;

public class CreateSnappingWindow(PoleRotation poleRotation, SnappingService snappingService) : Window("Create a new Snapping"), IDisposable
{
    private string name = string.Empty;
    private string modName = string.Empty;
    private string housingItemString = string.Empty;
    private uint housingItemId = 197799u;
    private float distance;
    
    public void Dispose() { }

    public override void Draw()
    {
        // Name
        ImGui.InputText("Name", ref name, 64);

        // Mod
        ImGui.InputText("Mod", ref modName, 64);

        // Object
        ImGui.InputText("Objet Housing", ref housingItemString, 64);
        if (uint.TryParse(housingItemString, out var id))
            housingItemId = id;

        // Distance (readonly)
        distance = SnappingService.GetSnappingDistance(housingItemId);
        ImGui.Text($"Distance: {distance:F3}");

        // Validation
        if (ImGui.Button("Validate"))
        {
            var newSnapping = new Snapping
            {
                Name = name,
                Mod = modName,
                HousingItemId = housingItemId,
                Distance = distance
            };

            snappingService.SaveSnapping(newSnapping);
            poleRotation.ToggleCreateUi();
        }
    }
}
