using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Collections.Generic;

public static class OtherPlayerManager
{
    static Dictionary<uint, Player> playerDict = new Dictionary<uint, Player>();

    public static void Init()
    {
        NetworkManager.Register(ENUM_CMD.CS_PLAYER_MOVE_NOTIFY, OnPlayerMoveNotify);
    }

    static void OnPlayerMoveNotify(byte[] data)
    {
        CSPlayerMoveNotify notify = NetworkManager.Deserialize<CSPlayerMoveNotify>(data);
        //Debug.Log("CSPlayerMoveNotify,id=" + notify.PlayerID + ",(" + notify.Position.x + "," + notify.Position.y + "," + notify.Position.z + ")");
        MovePlayer(notify.PlayerID, notify.Position, notify.Rotation);
    }

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
