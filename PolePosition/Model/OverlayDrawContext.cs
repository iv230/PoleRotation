using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PolePosition.Model;

internal sealed unsafe class OverlayDrawContext
{
    public float Radius { get; init; }
    public Vector3 Center { get; init; }
    public float Distance { get; init; }
    public bool FinePosition { get; init; }
    public float Yaw { get; init; }
    public BattleChara* Player { get; init; }
}
