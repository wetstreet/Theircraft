using protocol.cs_enum;
using protocol.cs_theircraft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalServer : MonoBehaviour
{
    delegate void LocalServerCallback(object T, Action<object> callback);

    static Dictionary<ENUM_CMD, LocalServerCallback> dict = new Dictionary<ENUM_CMD, LocalServerCallback>
    {
        [ENUM_CMD.CS_LOGIN_REQ] = Single_OnLoginReq,
        [ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ] = Single_OnChunksEnterLeaveViewReq,
        [ENUM_CMD.CS_HERO_MOVE_REQ] = Single_OnHeroMoveReq,
        [ENUM_CMD.CS_DELETE_BLOCK_REQ] = Single_OnDeleteBlockReq,
        [ENUM_CMD.CS_ADD_BLOCK_REQ] = Single_OnAddBlockReq,
    };

    public static bool ProcessRequest<T>(ENUM_CMD cmdID, T obj, Action<object> callback = null)
    {
        if (dict.ContainsKey(cmdID))
        {
            dict[cmdID](obj, callback);
            return true;
        }
        return false;
    }

    static void Single_OnLoginReq(object obj, Action<object> callback)
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
                Position = new CSVector3 { x = 0, y = 20, z = 0 },
                Rotation = new CSVector3 { x = 0, y = 0, z = 0 },
            }
        };
        
        callback(rsp);
    }

    static void Single_OnChunksEnterLeaveViewReq(object obj, Action<object> callback)
    {
        CSChunksEnterLeaveViewReq req = obj as CSChunksEnterLeaveViewReq;
        CSChunksEnterLeaveViewRes res = new CSChunksEnterLeaveViewRes();
        res.RetCode = 0;
        foreach (CSVector2Int chunk in req.EnterViewChunks)
        {
            byte[] blocks = TerrainGenerator.GenerateChunkData(chunk);
            CSChunk c = new CSChunk();
            c.Position = chunk;
            c.BlocksInBytes = blocks;
            res.EnterViewChunks.Add(c);
        }
        foreach (CSVector2Int chunk in req.LeaveViewChunks)
        {
            res.LeaveViewChunks.Add(chunk);
        }
        callback(res);
    }

    static void Single_OnHeroMoveReq(object obj, Action<object> callback)
    {
        CSHeroMoveReq req = obj as CSHeroMoveReq;
    }

    static void Single_OnDeleteBlockReq(object obj, Action<object> callback)
    {
        CSDeleteBlockReq req = obj as CSDeleteBlockReq;
        CSDeleteBlockRes res = new CSDeleteBlockRes();
        res.RetCode = 0;
        res.position = req.position;
        callback(res);
    }

    static void Single_OnAddBlockReq(object obj, Action<object> callback)
    {
        CSAddBlockReq req = obj as CSAddBlockReq;
        CSAddBlockRes res = new CSAddBlockRes();
        res.RetCode = 0;
        res.block = req.block;
        callback(res);
    }
}
