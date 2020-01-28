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

    static byte[] GetChunkData(Vector2Int pos)
    {
        if (!chunkDataDict.ContainsKey(pos))
        {
            chunkDataDict.Add(pos, new byte[65536]);
        }
        return chunkDataDict[pos];
    }

    static readonly string KEY_GENERATE_FLAG = "KEY_GENERATE_FLAG";
    static HashSet<Vector2IntSerializable> _chunkGenerateFlagSet;

    static readonly string KEY_CHUNK_DATA = "KEY_CHUNK_DATA";
    static Dictionary<Vector2IntSerializable, byte[]> _chunkDataDict;

    public static void SaveData()
    {
        _chunkGenerateFlagSet = new HashSet<Vector2IntSerializable>();
        foreach (Vector2Int chunk in chunkGenerateFlagSet)
        {
            _chunkGenerateFlagSet.Add(new Vector2IntSerializable(chunk));
        }
        DatabaseHelper.Save(KEY_GENERATE_FLAG, _chunkGenerateFlagSet);

        _chunkDataDict = new Dictionary<Vector2IntSerializable, byte[]>();
        foreach (KeyValuePair<Vector2Int, byte[]> keyValue in chunkDataDict)
        {
            _chunkDataDict.Add(new Vector2IntSerializable(keyValue.Key), keyValue.Value);
        }
        DatabaseHelper.Save(KEY_CHUNK_DATA, _chunkDataDict);
    }

    public static void InitData()
    {
        chunkGenerateFlagSet = new HashSet<Vector2Int>();
        chunkDataDict = new Dictionary<Vector2Int, byte[]>();

        if (DatabaseHelper.CanLoad(KEY_GENERATE_FLAG))
        {
            _chunkGenerateFlagSet = (HashSet<Vector2IntSerializable>)DatabaseHelper.Load<HashSet<Vector2IntSerializable>>(KEY_GENERATE_FLAG);

            foreach (Vector2IntSerializable chunk in _chunkGenerateFlagSet)
            {
                chunkGenerateFlagSet.Add(chunk.ToVector2Int());
            }
        }

        if (DatabaseHelper.CanLoad(KEY_CHUNK_DATA))
        {
            _chunkDataDict = (Dictionary<Vector2IntSerializable, byte[]>)DatabaseHelper.Load<Dictionary<Vector2IntSerializable, byte[]>>(KEY_CHUNK_DATA);

            foreach (KeyValuePair<Vector2IntSerializable, byte[]> keyValue in _chunkDataDict)
            {
                chunkDataDict.Add(keyValue.Key.ToVector2Int(), keyValue.Value);
            }
        }
    }

    public static void ClearData()
    {
        DatabaseHelper.Clear(KEY_GENERATE_FLAG);
        DatabaseHelper.Clear(KEY_CHUNK_DATA);
    }

    [Serializable]
    struct Vector2IntSerializable
    {
        int x;
        int y;

        public Vector2IntSerializable(Vector2Int v)
        {
            x = v.x;
            y = v.y;
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(x, y);
        }
    }

    static HashSet<Vector2Int> chunkGenerateFlagSet;
    static Dictionary<Vector2Int, byte[]> chunkDataDict;
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
