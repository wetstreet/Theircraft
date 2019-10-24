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
        [ENUM_CMD.CS_LOGIN_REQ] = Single_OnLoginReq,
        [ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ] = Single_OnChunksEnterLeaveViewReq,
        [ENUM_CMD.CS_HERO_MOVE_REQ] = Single_OnHeroMoveReq,
        [ENUM_CMD.CS_DELETE_BLOCK_REQ] = Single_OnDeleteBlockReq,
        [ENUM_CMD.CS_ADD_BLOCK_REQ] = Single_OnAddBlockReq,
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
                Position = new CSVector3 { x = 0, y = 50, z = 0 },
                Rotation = new CSVector3 { x = 0, y = 0, z = 0 },
            }
        };
        
        callback(NetworkManager.Serialize(rsp));
    }

    static byte[] CompressChunkBlocksData(CSVector2Int chunk, List<CSBlock> blocks)
    {
        List<byte> blocksInBytes = new List<byte>();
        //需要用Vector3Int来做值比较（辣鸡）
        Dictionary<Vector3Int, CSBlockType> pos2type = new Dictionary<Vector3Int, CSBlockType>();
        foreach (CSBlock block in blocks)
        {
            Vector3Int pos = Utilities.CSVector3Int_To_Vector3Int(block.position);
            pos2type[pos] = block.type;
        }
        Vector3Int tempPos = new Vector3Int();
        for (int k = 0; k < 256; k++)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    tempPos.x = chunk.x * 16 + i;
                    tempPos.y = k;
                    tempPos.z = chunk.y * 16 + j;
                    CSBlockType type = pos2type.ContainsKey(tempPos) ? pos2type[tempPos] : CSBlockType.None;
                    blocksInBytes.Add((byte)type);
                }
            }
        }
        return blocksInBytes.ToArray();
    }

    static void Single_OnChunksEnterLeaveViewReq(object obj, Action<byte[]> callback)
    {
        CSChunksEnterLeaveViewReq req = obj as CSChunksEnterLeaveViewReq;
        CSChunksEnterLeaveViewRes res = new CSChunksEnterLeaveViewRes();
        res.RetCode = 0;
        foreach (CSVector2Int chunk in req.EnterViewChunks)
        {
            List<CSBlock> blocks = TerrainGenerator.GetChunkBlocks(chunk);
            CSChunk c = new CSChunk();
            c.Position = chunk;
            c.BlocksInBytes = CompressChunkBlocksData(chunk, blocks);
            res.EnterViewChunks.Add(c);
        }
        foreach (CSVector2Int chunk in req.LeaveViewChunks)
        {
            res.LeaveViewChunks.Add(chunk);
        }
        callback(NetworkManager.Serialize(res));
    }

    static void Single_OnHeroMoveReq(object obj, Action<byte[]> callback)
    {
        CSHeroMoveReq req = obj as CSHeroMoveReq;
    }

    static void Single_OnDeleteBlockReq(object obj, Action<byte[]> callback)
    {
        CSDeleteBlockReq req = obj as CSDeleteBlockReq;
        CSDeleteBlockRes res = new CSDeleteBlockRes();
        res.RetCode = 0;
        res.position = req.position;
        callback(NetworkManager.Serialize(res));
    }

    static void Single_OnAddBlockReq(object obj, Action<byte[]> callback)
    {
        CSAddBlockReq req = obj as CSAddBlockReq;
        CSAddBlockRes res = new CSAddBlockRes();
        res.RetCode = 0;
        res.block = req.block;
        callback(NetworkManager.Serialize(res));
    }
}
