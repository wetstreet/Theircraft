using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

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

    //intput is global position
    public static bool HasBlock(int x, int y, int z)
    {
        return GetBlockType(x, y, z) != CSBlockType.None;
    }

    //intput is global position
    public static CSBlockType GetBlockType(int x, int y, int z)
    {
        //get chunk position first
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            //calculate block position in chunk
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            //Debug.Log("GetBlockType,globalblockpos=(" + x + "," + y + "," + z + "),chunkpos=(" + chunk.x + "," + chunk.z + "),blockposinchunk=(" + xInChunk + "," + y + "," + zInChunk + ")");
            return chunk.GetBlockType(xInChunk, y, zInChunk);
        }
        return CSBlockType.None;
    }

    //intput is global position
    public static Chunk GetChunk(int x, int y, int z)
    {
        int chunkX = Chunk.GetChunkPosByGlobalPos(x);
        int chunkZ = Chunk.GetChunkPosByGlobalPos(z);
        return GetChunk(chunkX, chunkZ);
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

    public static Dictionary<Vector2Int,Chunk>.KeyCollection GetChunkDictKeys()
    {
        return chunkDict.Keys;
    }

    public static void LoadChunk(CSChunk csChunk)
    {
        //Debug.Log("loadChunk,x=" + csChunk.Position.x + ",z=" + csChunk.Position.y);
        Chunk chunk = ChunkPool.GetChunk();
        chunk.SetData(csChunk.Position.x, csChunk.Position.y, csChunk.BlocksInBytes);
        chunk.RebuildMesh();
        AddToChunkDict(chunk);
    }

    public static void UnloadChunk(CSVector2Int chunkPos)
    {
        UnloadChunk(chunkPos.x, chunkPos.y);
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
}
