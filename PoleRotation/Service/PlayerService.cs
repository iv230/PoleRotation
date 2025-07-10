using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PoleRotation.Service;

public static unsafe class PlayerService
{
    public static BattleChara* GetCurrentPlayer()
    {
        var currentCharacter = (BattleChara*)(PoleRotation.ClientState.LocalPlayer?.Address ?? 0);
        return currentCharacter;
    }

    public static System.Numerics.Vector3 GetPlayerPosition()
    {
        return GetCurrentPlayer()->Position;
    }
    
    public static float GetPlayerRotation()
    {
        return GetCurrentPlayer()->Rotation;
    }
}
