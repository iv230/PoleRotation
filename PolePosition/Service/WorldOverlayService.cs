using System;
using System.Numerics;
using ImGuiNET;
using PolePosition.Model;

namespace PolePosition.Service;

public class WorldOverlayService(PolePosition polePosition, SnappingService snappingService)
{
    private const int PointCount = 360;
    private const float PlacementPrecision = 0.005f;

    public void Initialize()
    {
        PolePosition.PluginInterface.UiBuilder.Draw += DrawOverlay;
    }

    public void Dispose()
    {
        PolePosition.PluginInterface.UiBuilder.Draw -= DrawOverlay;
    }

    private unsafe OverlayDrawContext? TryPrepareDrawContext()
    {
        if (!polePosition.IsMainWindowOpen())
            return null;

        var snapping = snappingService.Selected;
        if (snapping == null)
            return null;

        var player = PlayerService.GetCurrentPlayer();
        if (player == null)
            return null;

        var center = HousingService.GetClosestItemPosition(snapping.HousingItemId);
        center.Y = player->Position.Y;

        var distance = Math.Abs(snapping.Distance - SnappingService.GetSnappingDistance(snapping.HousingItemId));
        var finePosition = distance < PlacementPrecision;

        return new OverlayDrawContext
        {
            Radius = snapping.Distance,
            Center = center,
            Distance = distance,
            FinePosition = finePosition,
            Yaw = player->Rotation,
            Player = player
        };
    }

    private void DrawOverlay()
    {
        var context = TryPrepareDrawContext();
        if (context == null) return;

        var drawList = ImGui.GetBackgroundDrawList();

        if (polePosition.Configuration.DisplayDistance && PolePosition.GameGui.WorldToScreen(context.Center, out var centerScreenPos))
        {
            drawList.AddText(
                new Vector2(centerScreenPos.X + 6, centerScreenPos.Y - 6),
                ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)),
                $"{context.Distance:F3}"
            );
        }

        for (var i = 0; i < PointCount; i++)
        {
            var angle = MathF.Tau * i / PointCount;

            var offsetX = MathF.Cos(angle) * context.Radius;
            var offsetZ = MathF.Sin(angle) * context.Radius;

            var rotatedX = (offsetX * MathF.Cos(context.Yaw)) - (offsetZ * MathF.Sin(context.Yaw));
            var rotatedZ = (offsetX * MathF.Sin(context.Yaw)) + (offsetZ * MathF.Cos(context.Yaw));

            var worldPoint = new Vector3(
                context.Center.X + rotatedX,
                context.Center.Y,
                context.Center.Z + rotatedZ
            );

            if (PolePosition.GameGui.WorldToScreen(worldPoint, out var screenPos))
            {
                var color = context.FinePosition
                                ? new Vector4(0f, 1f, 0f, 1f)
                                : new Vector4(1f, 0f, 0f, 1f);

                drawList.AddCircleFilled(screenPos, 3f, ImGui.GetColorU32(color));
            }
        }
    }
}
