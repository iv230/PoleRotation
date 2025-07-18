using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PolePosition.Service;

public static unsafe class PlayerService
{
    public static BattleChara* GetCurrentPlayer()
    {
        var currentCharacter = (BattleChara*)(PolePosition.ClientState.LocalPlayer?.Address ?? 0);
        return currentCharacter;
    }

    public static Vector3 GetPlayerPosition()
    {
        return GetCurrentPlayer()->Position;
    }
}
