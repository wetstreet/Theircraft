using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;
using protocol.cs_enum;
using System.Linq;

public class ChunkManager : MonoBehaviour
{
    static readonly Dictionary<Vector2Int, Chunk> chunkDict = new Dictionary<Vector2Int, Chunk>();

    static void AddToChunkDict(Chunk chunk)
    {
        chunkDict.Add(chunk.pos, chunk);
    }

    static void RemoveFromChunkDict(Chunk chunk)
    {
        chunkDict.Remove(chunk.pos);
    }

    static Vector2Int keyVec = new Vector2Int();
    public static Chunk GetChunk(int x, int z)
    {
        keyVec.x = x;
        keyVec.y = z;
        if (chunkDict.ContainsKey(keyVec))
        {
            return chunkDict[keyVec];
        }
        return null;
    }

    // intput is global position
    public static Chunk GetChunk(int x, int y, int z)
    {
        int chunkX = Chunk.GetChunkPosByGlobalPos(x);
        int chunkZ = Chunk.GetChunkPosByGlobalPos(z);
        return GetChunk(chunkX, chunkZ);
    }

    // intput is global position
    public static byte GetBlockByte(int x, int y, int z)
    {
        if (y < 0 || y > 255)
        {
            return 0;
        }
        // get chunk position first
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            //calculate block position in chunk
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            //Debug.Log("GetBlockType,globalblockpos=(" + x + "," + y + "," + z + "),chunkpos=(" + chunk.x + "," + chunk.z + "),blockposinchunk=(" + xInChunk + "," + y + "," + zInChunk + ")");
            return chunk.GetBlockByte(xInChunk, y, zInChunk);
        }
        return 0;
    }

    // intput is global position
    public static CSBlockType GetBlockType(int x, int y, int z)
    {
        return (CSBlockType)GetBlockByte(x, y, z);
    }

    // intput is global position
    public static bool HasBlock(int x, int y, int z)
    {
        return GetBlockType(x, y, z) != CSBlockType.None;
    }

    //input is global position
    public static bool HasOpaqueBlock(int x, int y, int z)
    {
        byte type = GetBlockByte(x, y, z);
        return type > 0 && !ChunkMeshGenerator.type2texcoords[type].isTransparent;
    }

    public static void AddBlock(int x, int y, int z, CSBlockType type)
    {
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            chunk.SetBlockType(xInChunk, y, zInChunk, type);
            chunk.RebuildMesh();
        }
    }

    public static void RemoveBlock(int x, int y, int z)
    {
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            chunk.SetBlockType(xInChunk, y, zInChunk, CSBlockType.None);
            if (!chunk.HasCollidableBlock(xInChunk, y + 1, zInChunk))
            {
                chunk.SetBlockType(xInChunk, y + 1, zInChunk, CSBlockType.None);
            }
            chunk.RebuildMesh();
        }
    }

    // get the chunks to be load (chunks should be loaded - chunks already loaded)
    public static List<Vector2Int> GetLoadChunks(Vector2Int centerChunkPos)
    {
        List<Vector2Int> shouldLoadChunks = Utilities.GetSurroudingChunks(centerChunkPos);
        return shouldLoadChunks.Except(chunkDict.Keys).ToList();
    }

    // get the chunks to be unload (any chunks in the loaded chunks dict whose distance to the centerChunkPos is bigger than chunkRadius should be unloaded)
    public static List<Vector2Int> GetUnloadChunks(Vector2Int centerChunkPos, int chunkRadius)
    {
        return chunkDict.Keys.Where(chunkPos => 
            Mathf.Abs(chunkPos.x - centerChunkPos.x) > chunkRadius || Mathf.Abs(chunkPos.y - centerChunkPos.y) > chunkRadius
            ).ToList();
    }

    public static void LoadChunk(CSChunk csChunk)
    {
        //Debug.Log("loadChunk,x=" + csChunk.Position.x + ",z=" + csChunk.Position.y);
        Chunk chunk = ChunkPool.GetChunk();
        chunk.SetData(csChunk.Position.x, csChunk.Position.y, csChunk.BlocksInBytes);
        AddToChunkDict(chunk);
        ChunkRefresher.AddRefreshList(chunk);
    }

    public static void UnloadChunk(int x, int z)
    {
        //Debug.Log("UnloadChunk,x=" + x + ",z=" + z);
        Chunk chunk = GetChunk(x, z);
        if (chunk != null)
        {
            chunk.ClearData();
            RemoveFromChunkDict(chunk);
            ChunkPool.Recover(chunk);
        }
    }

    #region enter/leave view
    public static void ChunksEnterLeaveViewReq(List<Vector2Int> enterViewChunks, List<Vector2Int> leaveViewChunks = null)
    {
        CSChunksEnterLeaveViewReq req = new CSChunksEnterLeaveViewReq();

        List<CSVector2Int> enter = new List<CSVector2Int>();
        foreach (Vector2Int chunk in enterViewChunks)
        {
            CSVector2Int c = new CSVector2Int
            {
                x = chunk.x,
                y = chunk.y
            };
            enter.Add(c);
        }
        req.EnterViewChunks.AddRange(enter);

        if (leaveViewChunks != null)
        {
            List<CSVector2Int> leave = new List<CSVector2Int>();
            foreach (Vector2Int chunk in leaveViewChunks)
            {
                CSVector2Int c = new CSVector2Int
                {
                    x = chunk.x,
                    y = chunk.y
                };
                leave.Add(c);
            }
            req.LeaveViewChunks.AddRange(leave);
        }

        //Debug.Log("CS_CHUNKS_ENTER_LEVAE_VIEW_REQ," + req.EnterViewChunks.Count + "," + req.LeaveViewChunks.Count);
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ, req, ChunksEnterLeaveViewRes);
    }

    static void ChunksEnterLeaveViewRes(byte[] data)
    {
        CSChunksEnterLeaveViewRes rsp = NetworkManager.Deserialize<CSChunksEnterLeaveViewRes>(data);

        //Debug.Log("CSChunksEnterLeaveViewRes," + rsp.EnterViewChunks.Count + "," + rsp.LeaveViewChunks.Count);
        if (rsp.RetCode == 0)
        {
            foreach (CSVector2Int csv in rsp.LeaveViewChunks)
            {
                UnloadChunk(csv.x, csv.y);
            }
            foreach (CSChunk cschunk in rsp.EnterViewChunks)
            {
                LoadChunk(cschunk);
            }
            if (!PlayerController.isInitialized)
            {
                PlayerController.Init();
                ChunkRefresher.ForceRefreshAll();
            }
            ChunkChecker.FinishRefresh();
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
    #endregion
}
