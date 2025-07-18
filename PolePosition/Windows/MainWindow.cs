using System;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PolePosition.Service;

namespace PolePosition.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly PolePosition polePosition;

    public MainWindow(PolePosition polePosition)
        : base("Pole Animation##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.polePosition = polePosition;

        TitleBarButtons = [
            new TitleBarButton()
            {
                Icon = FontAwesomeIcon.Cog,
                Click = _ => polePosition.ToggleConfigUi(),
                IconOffset = new Vector2(2,1),
                ShowTooltip = () =>
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Open settings");
                    ImGui.EndTooltip();
                }
            },
            new TitleBarButton()
            {
                Icon = FontAwesomeIcon.Plus,
                Click = _ => polePosition.ToggleCreateUi(),
                IconOffset = new Vector2(2,1),
                ShowTooltip = () =>
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Open creation window");
                    ImGui.EndTooltip();
                }
            },
            new TitleBarButton()
            {
                Icon = FontAwesomeIcon.Heart,
                Click = _ => {
                    Process.Start(new ProcessStartInfo {FileName = "https://ko-fi.com/iriswhm", UseShellExecute = true});
                },
                IconOffset = new Vector2(2, 1),
                ShowTooltip = () =>
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Support Iris on Ko-fi :3");
                    ImGui.EndTooltip();
                }
            },
        ];
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public override void Draw()
    {
        ImGui.Text("Select a Snapping:");

        if (polePosition.Configuration.Snappings.Count > 0)
        {
            DrawSnappingsTable();
        }
        else
        {
            ImGui.Text("No snappings available.");
        }

        if (ImGui.Button("Create"))
        {
            polePosition.ToggleCreateUi();
        }
    }

    private void DrawSnappingsTable()
    {
        var snappings = polePosition.Configuration.Snappings;
        var selected = polePosition.SnappingService.Selected;

        if (ImGui.BeginTable("SnappingTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY, new Vector2(0, 200)))
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Mod");
            ImGui.TableSetupColumn("Object");
            ImGui.TableSetupColumn("Distance");
            ImGui.TableHeadersRow();

            for (var i = 0; i < snappings.Count; i++)
            {
                var s = snappings[i];
                if (s == null) continue;

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);

                var label = $"##Selectable{i}";
                var isSelected = (s == selected);

                if (ImGui.Selectable(label, isSelected, ImGuiSelectableFlags.SpanAllColumns))
                {
                    polePosition.SnappingService.Select(s);
                }

                ImGui.SameLine(); ImGui.Text(s.Name);
                ImGui.TableSetColumnIndex(1); ImGui.Text(s.Mod);
                ImGui.TableSetColumnIndex(2); ImGui.Text(HousingService.GetObjectName(s.HousingItemId));
                ImGui.TableSetColumnIndex(3); ImGui.Text($"{s.Distance:F3}");
            }

            ImGui.EndTable();
        }
    }
}
