using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace PolePosition.Windows;

public class ConfigWindow(Configuration.Configuration configuration)
    : Window("Pole Position config###With a constant ID"), IDisposable
{
    public void Dispose() { }

    public override void Draw()
    {
        var verticalIncrease = configuration.DisplayDistance;
        if (ImGui.Checkbox("Display distance", ref verticalIncrease))
        {
            configuration.DisplayDistance = verticalIncrease;
            configuration.Save();
        }
        DrawTooltip("(?)", "Whenever or not you want the distance to be displayed in the middle of the position circle");
    }
    
    /// <summary>
    /// Draws a tooltip element
    /// </summary>
    /// <param name="tooltip">The text that will show the tooltip if hovered</param>
    /// <param name="text">The tooltip content</param>
    private static void DrawTooltip(String tooltip, String text)
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
