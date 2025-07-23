using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PolePosition.Service;

namespace PolePosition.Windows;

public class ConfigWindow(Configuration configuration)
    : Window("Pole Position config###With a constant ID"), IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        var displayDistance = configuration.DisplayDistance;
        if (ImGui.Checkbox("Display distance", ref displayDistance))
        {
            configuration.DisplayDistance = displayDistance;
            configuration.Save();
        }
        DrawTooltip("(?)", "Whenever or not you want the distance to be displayed in the middle of the position circle");
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
