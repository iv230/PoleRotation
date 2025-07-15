using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PoleRotation.Model;
using PoleRotation.Model.Penumbra;
using PoleRotation.Service;

namespace PoleRotation.Windows;

public class CreateSnappingWindow : Window, IDisposable
{
    // Dependencies
    private readonly PoleRotation poleRotation;
    private readonly SnappingService snappingService;
    private readonly PenumbraService penumbraService;

    // SNapping to create
    private string name = string.Empty;
    private string modName = string.Empty;
    private string housingItemString = string.Empty;
    private uint housingItemId = 197799u;
    private float distance;

    // Mod selector
    private List<PenumbraMod> availableMods = [];
    private int selectedModIndex = -1;
    private string modSearch = "";

    public CreateSnappingWindow(PoleRotation poleRotation, SnappingService snappingService, PenumbraService penumbraService) : base("Create a new Snapping")
    {
        this.poleRotation = poleRotation;
        this.snappingService = snappingService;
        this.penumbraService = penumbraService;
        
        _ = Task.Run(async () => availableMods = await penumbraService.LoadPenumbraModsAsync());
    }

    public void Dispose() { }

    public override void Draw()
    {
        // Name
        ImGui.InputText("Name", ref name, 64);

        // Mod
        DrawModSelector();

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

    private void DrawModSelector()
    {
        if (availableMods.Count == 0)
        {
            ImGui.TextColored(new Vector4(1f, 0.6f, 0.2f, 1f), "No mod detected");
            return;
        }

        var previewValue = selectedModIndex >= 0 && selectedModIndex < availableMods.Count
                               ? availableMods[selectedModIndex].Name
                               : "Select a mod";

        if (ImGui.BeginCombo("Associated mod", previewValue, ImGuiComboFlags.HeightLargest))
        {
            // Searchbar
            ImGui.PushItemWidth(-1);
            ImGui.InputText("##ModSearch", ref modSearch, 100);
            ImGui.PopItemWidth();
            ImGui.Separator();

            // Mod list
            ImGui.BeginChild("ModListChild", new Vector2(0, 200), false);

            for (var i = 0; i < availableMods.Count; i++)
            {
                var selectedName = availableMods[i].Name;

                if (!string.IsNullOrEmpty(modSearch) && !selectedName.Contains(modSearch, StringComparison.OrdinalIgnoreCase))
                    continue;

                var isSelected = (i == selectedModIndex);
                if (ImGui.Selectable(selectedName, isSelected))
                {
                    selectedModIndex = i;
                    modName = availableMods[i].BasePath;
                }

                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndChild();
            ImGui.EndCombo();
        }
    }
}
