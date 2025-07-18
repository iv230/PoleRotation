using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PolePosition.Model;
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
            MinimumSize = new Vector2(650, 500),
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

        if (ImGui.BeginTable("SnappingTable", 6, ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, new Vector2(0, 400)))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 25f);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Mod");
            ImGui.TableSetupColumn("Object");
            ImGui.TableSetupColumn("Distance");
            ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, 70f);
            ImGui.TableHeadersRow();

            for (var i = 0; i < snappings.Count; i++)
            {
                DrawSnappingTableLine(snappings, i, selected);
            }

            ImGui.EndTable();
        }
    }

    private void DrawSnappingTableLine(List<Snapping?> snappings, int lineNumber, Snapping? selected)
    {
        var s = snappings[lineNumber];
        if (s == null) return;

        var isSelected = s == selected;

        ImGui.TableNextRow();

        ImGui.TableSetColumnIndex(0);

        var checkedState = isSelected;
        var checkboxId = $"##Checkbox{lineNumber}";
        if (ImGui.Checkbox(checkboxId, ref checkedState))
        {
            polePosition.SnappingService.Select(checkedState ? s : null);
        }

        ImGui.TableSetColumnIndex(1);
        ImGui.Text(s.Name);

        ImGui.TableSetColumnIndex(2);
        ImGui.Text(s.Mod);

        ImGui.TableSetColumnIndex(3);
        ImGui.Text(HousingService.GetObjectName(s.HousingItemId));

        ImGui.TableSetColumnIndex(4);
        ImGui.Text($"{s.Distance:F3}");

        ImGui.TableSetColumnIndex(5);
        var canDelete = ImGui.GetIO().KeyShift;

        if (!canDelete) ImGui.BeginDisabled();

        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconString()}##Delete{lineNumber}"))
        {
            polePosition.SnappingService.DeleteSnapping(s);
        }
        ImGui.PopFont();

        if (!canDelete) ImGui.EndDisabled();

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Hold SHIFT to delete");
            ImGui.EndTooltip();
        }
    }
}
