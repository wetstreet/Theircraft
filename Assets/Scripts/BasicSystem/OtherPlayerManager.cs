using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerManager : MonoBehaviour
{
    static Dictionary<uint, Player> playerDict = new Dictionary<uint, Player>();

    static OtherPlayerManager instance;
    public static void Init()
    {
        GameObject obj = new GameObject("OtherPlayerManager");
        obj.transform.localPosition = Vector3.zero;
        instance = obj.AddComponent<OtherPlayerManager>();
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
            //p.SetPosAndRot(pos, rot);
            p.Move(pos, rot);
        }
    }

    void Update()
    {
        foreach (Player p in playerDict.Values)
        {
            p.MoveLerp();
        }
    }
}
