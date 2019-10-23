using protocol.cs_enum;
using protocol.cs_theircraft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalServer : MonoBehaviour
{
    delegate void LocalServerCallback(object T, Action<byte[]> callback);

    static Dictionary<ENUM_CMD, LocalServerCallback> dict = new Dictionary<ENUM_CMD, LocalServerCallback>
    {
        [ENUM_CMD.CS_LOGIN_REQ] = Single_OnLoginReq
    };

    public static bool ProcessRequest<T>(ENUM_CMD cmdID, T obj, Action<byte[]> callback = null)
    {
        if (dict.ContainsKey(cmdID))
        {
            dict[cmdID](obj, callback);
            return true;
        }
        return false;
    }

    static void Single_OnLoginReq(object obj, Action<byte[]> callback)
    {
        Debug.Log("OnSingleLoginReq");
        CSLoginReq req = obj as CSLoginReq;
        CSLoginRes rsp = new CSLoginRes()
        {
            RetCode = 0,
            PlayerData = new CSPlayer
            {
                PlayerID = 0,
                Name = "Local Player",
                Position = new CSVector3 { x = 0, y = 0, z = 0 },
                Rotation = new CSVector3 { x = 0, y = 0, z = 0 },
            }
        };
        
        callback(NetworkManager.Serialize(rsp));
    }
}
