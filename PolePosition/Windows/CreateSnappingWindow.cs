using System;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PolePosition.Model;
using PolePosition.Model.Penumbra;
using PolePosition.Service;
using PolePosition.Windows.Components;

namespace PolePosition.Windows;

public class CreateSnappingWindow : Window, IDisposable
{
    // Dependencies
    private readonly PolePosition polePosition;
    private readonly SnappingService snappingService;

    // Snapping to create
    private string name = string.Empty;
    private string modName = string.Empty;
    private uint housingItemId;
    private float distance;

    private readonly ImGuiSearchableCombo<PenumbraMod> modCombo =
        new(m => m.Name);

    private readonly ImGuiSearchableCombo<FurnitureItem> housingCombo =
        new(h => h.Name, h => h.Id);

    public CreateSnappingWindow(PolePosition polePosition, SnappingService snappingService, PenumbraService penumbraService) : base("Create a new Snapping")
    {
        this.polePosition = polePosition;
        this.snappingService = snappingService;

        _ = Task.Run(() => modCombo.Items = penumbraService.LoadPenumbraMods() ?? []);
        _ = Task.Run(() => housingCombo.Items = HousingService.GetAllHousingObjects());
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        // Name
        ImGui.InputText("Name", ref name, 64);

        // Mod
        if (modCombo.Draw("Associated mod", out var selectedMod, out _))
            modName = selectedMod?.BasePath ?? "";

        // Object
        if (housingCombo.Draw("Associated object", out _, out var housingId))
            housingItemId = housingId;

        // Distance (readonly)
        distance = SnappingService.GetSnappingDistance(housingItemId);
        if (housingItemId != 0)
            ImGui.Text($"Distance: {distance:F3}");

        // Validation
        ImGui.BeginDisabled(!CanSave());
        if (ImGui.Button("Validate"))
            Save();
        ImGui.EndDisabled();
    }

    private bool CanSave()
    {
        return name != string.Empty &&
               modName != string.Empty &&
               housingItemId != 0;
    }

    private void Save()
    {
        var newSnapping = new Snapping
        {
            Name = name,
            Mod = modName,
            HousingItemId = housingItemId,
            Distance = distance
        };

        snappingService.SaveSnapping(newSnapping);
        
        name = string.Empty;
        modCombo.Clear();
        housingCombo.Clear();
        polePosition.ToggleCreateUi();
    }
}
