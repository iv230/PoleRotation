using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PolePosition.Service;

namespace PolePosition.Windows;

public class ConfigWindow(Configuration configuration, PenumbraService penumbraService)
    : Window("Pole Position config###With a constant ID"), IDisposable
{
    private List<string> collections = [];
    private int selectedCollectionIndex = -1;
    private bool collectionsLoaded;

    private PenumbraService PenumbraService => penumbraService;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        // Charger les collections Penumbra au premier affichage
        if (!collectionsLoaded)
        {
            collectionsLoaded = true;
            _ = Task.Run(async () =>
            {
                collections = await PenumbraService.GetAllCollectionsAsync();
                selectedCollectionIndex = collections.FindIndex(c => c == configuration.PenumbraCollection);
                if (selectedCollectionIndex < 0 && collections.Count > 0)
                    selectedCollectionIndex = 0;
            });
        }

        var verticalIncrease = configuration.DisplayDistance;
        if (ImGui.Checkbox("Display distance", ref verticalIncrease))
        {
            configuration.DisplayDistance = verticalIncrease;
            configuration.Save();
        }
        DrawTooltip("(?)", "Whenever or not you want the distance to be displayed in the middle of the position circle");

        ImGui.Separator();

        ImGui.Text("Penumbra Collection");

        if (collections.Count > 0 && selectedCollectionIndex >= 0)
        {
            var current = collections[selectedCollectionIndex];
            if (ImGui.BeginCombo("##PenumbraCollection", current))
            {
                for (var i = 0; i < collections.Count; i++)
                {
                    var isSelected = (i == selectedCollectionIndex);
                    if (ImGui.Selectable(collections[i], isSelected))
                    {
                        selectedCollectionIndex = i;
                        configuration.PenumbraCollection = collections[i];
                        configuration.Save();
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
        }
        else
        {
            ImGui.TextDisabled("Loading collections...");
        }
    }

    /// <summary>
    /// Draws a tooltip element
    /// </summary>
    /// <param name="tooltip">The text that will show the tooltip if hovered</param>
    /// <param name="text">The tooltip content</param>
    private static void DrawTooltip(string tooltip, string text)
    {
        ImGui.SameLine();
        ImGui.TextDisabled(tooltip);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetNextWindowSize(new Vector2(300, 0), ImGuiCond.Always);
            ImGui.BeginTooltip();
            ImGui.TextWrapped(text);
            ImGui.EndTooltip();
        }
    }
}
