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

        foreach (KeyValuePair<Vector3Int, Vector3Int> kvPair in dependenceDict)
        {
            rsp.BlockAttrs.Add(new CSBlockAttrs {
                pos = kvPair.Key.ToCSVector3Int(),
                depentPos = kvPair.Value.ToCSVector3Int(),
            });
        }

        foreach (KeyValuePair<Vector3Int, CSBlockOrientation> kvPair in orientationDict)
        {
            rsp.BlockAttrs.Add(new CSBlockAttrs
            {
                pos = kvPair.Key.ToCSVector3Int(),
                orient = kvPair.Value,
            });
        }

        callback(rsp);
    }

    static byte[] GetChunkData(Vector2Int pos)
    {
        if (!chunkDataDict.ContainsKey(pos))
        {
            chunkDataDict.Add(pos, new byte[65536]);
        }
        return chunkDataDict[pos];
    }

    static Dictionary<Vector3Int, CSBlockOrientation> orientationDict;
    static Dictionary<Vector3Int, Vector3Int> dependenceDict;
    static HashSet<Vector2Int> chunkGenerateFlagSet;
    static Dictionary<Vector2Int, byte[]> chunkDataDict;

    public static void SaveData()
    {
        DatabaseHelper.SaveGenerateFlag(chunkGenerateFlagSet);
        DatabaseHelper.SaveChunkData(chunkDataDict);
        DatabaseHelper.SaveDependence(dependenceDict);
        DatabaseHelper.SaveOrientation(orientationDict);
    }

    public static void InitData()
    {
        chunkGenerateFlagSet = DatabaseHelper.LoadGenerateFlag();
        chunkDataDict = DatabaseHelper.LoadChunkData();
        dependenceDict = DatabaseHelper.LoadDependence();
        orientationDict = DatabaseHelper.LoadOrientation();
    }

    public static void ClearData()
    {
        DatabaseHelper.ClearAll();
    }

    static Vector2Int keyVector = new Vector2Int();
    public static void SetBlockType(int x, int y, int z, CSBlockType type)
    {
        keyVector.x = Chunk.GetChunkPosByGlobalPos(x);
        keyVector.y = Chunk.GetChunkPosByGlobalPos(z);
        byte[] chunkData = GetChunkData(keyVector);

        int xInChunk = x - keyVector.x * 16;
        int zInChunk = z - keyVector.y * 16;

        chunkData[256 * y + 16 * xInChunk + zInChunk] = (byte)type;
    }

    static void Single_OnChunksEnterLeaveViewReq(object obj, Action<object> callback)
    {
        CSChunksEnterLeaveViewReq req = obj as CSChunksEnterLeaveViewReq;
        CSChunksEnterLeaveViewRes res = new CSChunksEnterLeaveViewRes();
        res.RetCode = 0;

        foreach (CSVector2Int chunk in req.EnterViewChunks)
        {
            Vector2Int chunkPos = chunk.ToVector2Int();
            byte[] blocks = GetChunkData(chunkPos);
            if (!chunkGenerateFlagSet.Contains(chunkPos))
            {
                TerrainGenerator.GenerateChunkData(chunk, blocks);
                chunkGenerateFlagSet.Add(chunkPos);
            }
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
        if (dependenceDict.ContainsKey(req.position.ToVector3Int()))
        {
            dependenceDict.Remove(req.position.ToVector3Int());
        }
        if (orientationDict.ContainsKey(req.position.ToVector3Int()))
        {
            orientationDict.Remove(req.position.ToVector3Int());
        }
        callback(res);
    }

    static void Single_OnAddBlockReq(object obj, Action<object> callback)
    {
        CSAddBlockReq req = obj as CSAddBlockReq;
        CSAddBlockRes res = new CSAddBlockRes();
        res.RetCode = 0;
        res.block = req.block;
        if (req.block.depentPos != null)
        {
            dependenceDict.Add(req.block.position.ToVector3Int(), req.block.depentPos.ToVector3Int());
        }
        if (req.block.orient != CSBlockOrientation.Default)
        {
            orientationDict.Add(req.block.position.ToVector3Int(), req.block.orient);
        }
        callback(res);
    }
}
