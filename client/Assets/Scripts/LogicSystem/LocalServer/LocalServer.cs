using protocol.cs_enum;
using protocol.cs_theircraft;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerData
{
    public CSVector3 Position;
    public CSVector3 Rotation;
    public uint SelectIndex;
    public ServerItem[] SelectItems;
    public ServerItem[] BagItems;
}

[Serializable]
public struct ServerItem
{
    public CSBlockType type;
    public uint count;
}

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
        [ENUM_CMD.CS_HERO_CHANGE_SELECT_INDEX_REQ] = Single_OnHeroChangeSelectIndexReq,
        [ENUM_CMD.CS_HERO_CHANGE_SELECT_ITEM_REQ] = Single_OnHeroChangeSelectItemReq,
        [ENUM_CMD.CS_SEND_MESSAGE_REQ] = Single_OnSendMessageReq,
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

    static PlayerData playerData;

    static void Single_OnLoginReq(object obj, Action<object> callback)
    {
        CSLoginReq req = obj as CSLoginReq;
        CSLoginRes rsp = new CSLoginRes()
        {
            RetCode = 0,
            PlayerData = new CSPlayer
            {
                PlayerID = 0,
                Name = "Steve",
                Position = playerData.Position,
                Rotation = playerData.Rotation,
                SelectIndex = playerData.SelectIndex,
            }
        };

        for (int i = 0; i < 18; i++)
        {
            rsp.PlayerData.BagItems.Add(new CSItem {
                Type = playerData.BagItems[i].type,
                Count = playerData.BagItems[i].count
            });
        }

        for (int i = 0; i < 9; i++)
        {
            rsp.PlayerData.SelectItems.Add(new CSItem
            {
                Type = playerData.SelectItems[i].type,
                Count = playerData.SelectItems[i].count
            });
        }
        
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
    static List<CSBlockType> selectItems;

    public static void SaveData()
    {
        DatabaseHelper.SaveGenerateFlag(chunkGenerateFlagSet);
        DatabaseHelper.SaveChunkData(chunkDataDict);
        DatabaseHelper.SaveDependence(dependenceDict);
        DatabaseHelper.SaveOrientation(orientationDict);
        DatabaseHelper.Save(DatabaseHelper.KEY_PLAYER_DATA, playerData);
    }

    public static void InitData()
    {
        chunkGenerateFlagSet = DatabaseHelper.LoadGenerateFlag();
        chunkDataDict = DatabaseHelper.LoadChunkData();
        dependenceDict = DatabaseHelper.LoadDependence();
        orientationDict = DatabaseHelper.LoadOrientation();

        if (DatabaseHelper.CanLoad(DatabaseHelper.KEY_PLAYER_DATA))
        {
            playerData = DatabaseHelper.Load<PlayerData>(DatabaseHelper.KEY_PLAYER_DATA);
        }
        else
        {
            playerData = new PlayerData
            {
                Position = new CSVector3 { x = 0, y = 20, z = 0 },
                Rotation = new CSVector3 { x = 0, y = 0, z = 0 },
                SelectIndex = 0,
                BagItems = new ServerItem[18],
                SelectItems = new ServerItem[9],
            };
        }
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
        //Debug.Log("Single_OnHeroMoveReq,req=" + req.Position.x + "," + req.Position.y + "," + req.Position.z);
        playerData.Position = req.Position;
        playerData.Rotation = req.Rotation;
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

    static void Single_OnHeroChangeSelectIndexReq(object obj, Action<object> callback)
    {
        CSHeroChangeSelectIndexReq req = obj as CSHeroChangeSelectIndexReq;
        playerData.SelectIndex = req.Index;
    }

    static void Single_OnHeroChangeSelectItemReq(object obj, Action<object> callback)
    {
        CSHeroChangeSelectItemReq req = obj as CSHeroChangeSelectItemReq;
        playerData.SelectItems[req.Index].type = req.Item;
        playerData.SelectItems[req.Index].count = req.Count;
    }

    static void Single_OnSendMessageReq(object obj, Action<object> callback)
    {
        CSSendMessageReq req = obj as CSSendMessageReq;
        CSSendMessageRes res = new CSSendMessageRes();
        res.RetCode = 0;
        callback(res);
        CSMessageNotify notify = new CSMessageNotify();
        notify.Name = DataCenter.name;
        notify.Content = req.Content;
        NetworkManager.Notify(ENUM_CMD.CS_MESSAGE_NOTIFY, notify);
    }
}
