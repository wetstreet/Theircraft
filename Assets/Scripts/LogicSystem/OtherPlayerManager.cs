using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OtherPlayerManager
{
    static Dictionary<uint, Player> playerDict = new Dictionary<uint, Player>();

    public static void AddPlayer(CSPlayer player)
    {
        playerDict.Add(player.PlayerID, new Player(player));
    }

    public static void MovePlayer(uint id, CSVector3 pos, CSVector3 rot)
    {
        if (playerDict.ContainsKey(id))
        {
            Player p = playerDict[id];
            p.Move(pos, rot);
        }
    }
}
