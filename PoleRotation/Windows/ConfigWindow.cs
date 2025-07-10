using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace PoleRotation.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(PoleRotation poleRotation) : base("A Wonderful Configuration Window###With a constant ID")
    {

    }

    public void Dispose() { }

    public override void PreDraw()
    {

    }

    public override void Draw()
    {

    }
}
