using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;
using protocol.cs_enum;
using System.Linq;

public class ChunkManager
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
    public static CSBlockType GetBlockType(Vector3Int position)
    {
        return GetBlockType(position.x, position.y, position.z);
    }

    // intput is global position
    public static CSBlockType GetBlockType(int x, int y, int z)
    {
        return (CSBlockType)GetBlockByte(x, y, z);
    }

    // intput is global position
    public static bool HasBlock(Vector3Int pos)
    {
        return HasBlock(pos.x, pos.y, pos.z);
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

    //input is global position
    public static bool HasCollidableBlock(int x, int y, int z)
    {
        byte type = GetBlockByte(x, y, z);
        return type > 0 && ChunkMeshGenerator.type2texcoords[type].isCollidable;
    }

    public static void AddBlock(CSBlock block)
    {
        Chunk chunk = GetChunk(block.position.x, block.position.y, block.position.z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(block.position.x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(block.position.z);
            chunk.SetBlockType(xInChunk, block.position.y, zInChunk, block.type);
            AddBlockOrientation(block.position.ToVector3Int(), block.orient);
            chunk.RebuildMesh();

            // if this block is adjacent to other chunks, refresh nearby chunks
            foreach (Chunk nearbyChunk in GetNearbyChunks(xInChunk, zInChunk, chunk))
            {
                nearbyChunk.RebuildMesh();
            }
        }
    }

    static List<Chunk> GetNearbyChunks(int xInChunk, int zInChunk, Chunk chunk)
    {
        List<Chunk> nearbyChunks = new List<Chunk>();
        if (xInChunk == 0)
        {
            nearbyChunks.Add(GetChunk(chunk.x - 1, chunk.z));
        }
        else if (xInChunk == 15)
        {
            nearbyChunks.Add(GetChunk(chunk.x + 1, chunk.z));
        }
        if (zInChunk == 0)
        {
            nearbyChunks.Add(GetChunk(chunk.x, chunk.z - 1));
        }
        else if (zInChunk == 15)
        {
            nearbyChunks.Add(GetChunk(chunk.x, chunk.z + 1));
        }
        return nearbyChunks;
    }

    public static void RemoveBlock(int x, int y, int z)
    {
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            chunk.SetBlockType(xInChunk, y, zInChunk, CSBlockType.None);
            RemoveBlockOrientation(new Vector3Int(x, y, z));
            if (!chunk.HasCollidableBlock(xInChunk, y + 1, zInChunk))
            {
                chunk.SetBlockType(xInChunk, y + 1, zInChunk, CSBlockType.None);
            }
            chunk.RebuildMesh();

            // if this block is adjacent to other chunks, refresh nearby chunks
            foreach (Chunk nearbyChunk in GetNearbyChunks(xInChunk, zInChunk, chunk))
            {
                nearbyChunk.RebuildMesh();
            }
        }
    }

    static readonly Dictionary<Vector3Int, CSBlockOrientation> orientationDict = new Dictionary<Vector3Int, CSBlockOrientation>();
    public static void AddBlockOrientation(Vector3Int pos, CSBlockOrientation orientation)
    {
        orientationDict.Add(pos, orientation);
    }

    public static void RemoveBlockOrientation(Vector3Int pos)
    {
        orientationDict.Remove(pos);
    }

    public static CSBlockOrientation GetBlockOrientation(Vector3Int pos)
    {
        if (orientationDict.ContainsKey(pos))
        {
            return orientationDict[pos];
        }
        else
        {
            return CSBlockOrientation.Default;
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
        ChunkRefresher.Add(chunk);
    }

    public static void UnloadChunk(int x, int z)
    {
        //Debug.Log("UnloadChunk,x=" + x + ",z=" + z);
        Chunk chunk = GetChunk(x, z);
        if (chunk != null)
        {
            ChunkRefresher.Remove(chunk);
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

    static void ChunksEnterLeaveViewRes(object data)
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
                LocalNavMeshBuilder.Init();
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
