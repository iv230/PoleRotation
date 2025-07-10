using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ImGuiNET;

namespace PoleRotation.Service;

public class WorldOverlayService(SnappingService snappingService)
{
    private const int PointCount = 360;

    public void Initialize()
    {
        PoleRotation.PluginInterface.UiBuilder.Draw += DrawOverlay;
    }

    public void Dispose()
    {
        PoleRotation.PluginInterface.UiBuilder.Draw -= DrawOverlay;
    }

    private unsafe void DrawOverlay()
    {
        var snapping = snappingService.Selected;
        if (snapping == null) return;

        var player = PlayerService.GetCurrentPlayer();
        if (player == null) return;
        
        var radius = snapping.Distance;
        var center = HousingService.GetClosestItemPosition(197799u);
        center.Y = player->Position.Y;

        var distance = Math.Abs(snapping.Distance - SnappingService.GetSnappingDistance());
        var finePosition = distance < 0.005f;

        var yaw = player->Rotation; // en radians, tourne autour de l’axe Y

        var drawList = ImGui.GetBackgroundDrawList();
        
        if (PoleRotation.GameGui.WorldToScreen(center, out var centerScreenPos))
        {
            drawList.AddText(
                new Vector2(centerScreenPos.X + 6, centerScreenPos.Y - 6),
                ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)),
                distance.ToString(CultureInfo.InvariantCulture)
            );
        }

        for (var i = 0; i < PointCount; i++)
        {
            var angle = MathF.Tau * i / PointCount;

            // Point dans le repère joueur (avant rotation)
            var offsetX = MathF.Cos(angle) * radius;
            var offsetZ = MathF.Sin(angle) * radius;

            // Appliquer la rotation du joueur
            var rotatedX = (offsetX * MathF.Cos(yaw)) - (offsetZ * MathF.Sin(yaw));
            var rotatedZ = (offsetX * MathF.Sin(yaw)) + (offsetZ * MathF.Cos(yaw));

            // Position du point dans le monde
            var worldPoint = new Vector3(
                center.X + rotatedX,
                center.Y,
                center.Z + rotatedZ
            );

            // Convertir en coordonnées écran
            if (PoleRotation.GameGui.WorldToScreen(worldPoint, out var screenPos))
            {
                var color = finePosition ? new Vector4(0f, 1f, 0f, 1f) : new Vector4(1f, 0f, 0f, 1f);
                
                // Dessiner le point à l’écran
                drawList.AddCircleFilled(screenPos, 3f, ImGui.GetColorU32(color));
            }
        }
    }
}
