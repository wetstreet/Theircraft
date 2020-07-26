using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerManager : MonoBehaviour
{
    static Dictionary<uint, Player> playerDict = new Dictionary<uint, Player>();

    static OtherPlayerManager instance;

    public static void Init()
    {
        GameObject go = new GameObject("OtherPlayer");
        instance = go.AddComponent<OtherPlayerManager>();

        NetworkManager.Register(ENUM_CMD.CS_PLAYER_MOVE_NOTIFY, OnPlayerMoveNotify);
    }

    static void OnPlayerMoveNotify(object data)
    {
        CSPlayerMoveNotify notify = NetworkManager.Deserialize<CSPlayerMoveNotify>(data);
        //Debug.Log("CSPlayerMoveNotify,id=" + notify.PlayerID + ",(" + notify.Position.x + "," + notify.Position.y + "," + notify.Position.z + ")");
        MovePlayer(notify.PlayerID, notify.Position, notify.Rotation);
    }

    public static void AddPlayer(CSPlayer csplayer)
    {
        Player player = Player.CreatePlayer(csplayer);
        player.transform.parent = instance.transform;
        playerDict.Add(csplayer.PlayerID, player);
    }

    public static Player GetPlayer(uint id)
    {
        if (playerDict.ContainsKey(id))
        {
            return playerDict[id];
        }
        return null;
    }

    public static void MovePlayer(uint id, CSVector3 pos, CSVector3 rot)
    {
        if (playerDict.ContainsKey(id))
        {
            Player p = playerDict[id];
            //p.Move(pos, rot);
        }
    }
}
