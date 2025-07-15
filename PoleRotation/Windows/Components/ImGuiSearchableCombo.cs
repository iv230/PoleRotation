using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace PoleRotation.Windows.Components;

public class ImGuiSearchableCombo<T>(Func<T, string> getDisplayName, Func<T, uint>? getId = null, float height = 200f)
{
    public List<T> Items = [];
    public int SelectedIndex = -1;
    public string SearchText = "";

    public bool Draw(string label, out T? selectedItem, out uint selectedId)
    {
        selectedItem = default;
        selectedId = 0;

        if (Items.Count == 0)
        {
            ImGui.TextColored(new Vector4(1f, 0.6f, 0.2f, 1f), $"No items for {label}");
            return false;
        }

        var previewValue = SelectedIndex >= 0 && SelectedIndex < Items.Count
                               ? getDisplayName(Items[SelectedIndex])
                               : $"Select a {label}";

        var changed = false;

        if (ImGui.BeginCombo(label, previewValue, ImGuiComboFlags.HeightLargest))
        {
            ImGui.PushItemWidth(-1);
            ImGui.InputText($"##Search_{label}", ref SearchText, 100);
            ImGui.PopItemWidth();
            ImGui.Separator();

            ImGui.BeginChild($"List_{label}", new Vector2(0, height), false);

            for (var i = 0; i < Items.Count; i++)
            {
                var display = getDisplayName(Items[i]);

                if (!string.IsNullOrEmpty(SearchText) &&
                    !display.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    continue;

                var isSelected = i == SelectedIndex;
                if (ImGui.Selectable(display, isSelected))
                {
                    SelectedIndex = i;
                    selectedItem = Items[i];
                    selectedId = getId?.Invoke(Items[i]) ?? 0;
                    changed = true;
                }

                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndChild();
            ImGui.EndCombo();
        }

        return changed;
    }
    
    public void Clear()
    {
        SelectedIndex = -1;
        SearchText = string.Empty;
    }
}
