using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public struct Block
{
    public Vector3Int pos;
    public Vector2Int chunk;
    public CSBlockType type;
}

public class test : MonoBehaviour
{
    static Dictionary<Vector2Int, GameObject> chunk2object = new Dictionary<Vector2Int, GameObject>();

    static List<Vector2Int> chunkList = new List<Vector2Int>();
    public static List<Vector2Int> GetChunkList()
    {
        //chunkList.AddRange(chunk2object.Keys);
        return new List<Vector2Int>(chunk2object.Keys);
        //return chunk2object.Keys.;
    }

    static Dictionary<Vector3Int, Block> posBlockDict = new Dictionary<Vector3Int, Block>();
    static Dictionary<Vector2Int, Dictionary<Vector3Int, Block>> chunkBlocksDict = new Dictionary<Vector2Int, Dictionary<Vector3Int, Block>>();

    public static bool ContainBlock(Vector3Int blockPos)
    {
        return posBlockDict.ContainsKey(blockPos);
    }

    public static void AddBlock(Vector3Int blockPos, CSBlockType type)
    {
        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt(blockPos.x / 16f), Mathf.FloorToInt(blockPos.z / 16f));
        Block block = new Block { pos = blockPos, chunk = chunkPos, type = type };
        posBlockDict[blockPos] = block;
        chunkBlocksDict[chunkPos].Add(block.pos, block);

        RefreshChunk(chunkPos);
    }

    public static Block GetBlockAtPos(Vector3Int pos)
    {
        return posBlockDict[pos];
    }

    public static void RemoveBlock(Vector3Int blockPos)
    {
        Block block = posBlockDict[blockPos];
        Vector2Int chunkPos = block.chunk;

        posBlockDict.Remove(blockPos);
        chunkBlocksDict[chunkPos].Remove(blockPos);

        RefreshChunk(chunkPos);
    }

    public static void GenerateChunk(CSChunk chunk)
    {
        Dictionary<Vector3Int, Block> chunk_posBlockDict = new Dictionary<Vector3Int, Block>();
        Vector2Int chunkPos = new Vector2Int(chunk.Position.x, chunk.Position.y);


        //压缩后的数据结构
        for (int k = 0; k < 256; k++)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    byte b = chunk.BlocksInBytes[256 * k + 16 * i + j];
                    CSBlockType type = (CSBlockType)b;
                    if (type != CSBlockType.None)
                    {
                        Vector3Int blockPos = new Vector3Int(chunkPos.x * 16 + i, k, chunkPos.y * 16 + j);
                        Block block = new Block { pos = blockPos, chunk = chunkPos, type = type };
                        posBlockDict[blockPos] = block;
                        chunk_posBlockDict[blockPos] = block;
                    }
                }
            }
        }

        //Debug.Log("chunk=("+chunk.Position.x+","+chunk.Position.y+"),chunk player num=" + chunk.Players.Count);
        foreach (CSPlayer p in chunk.Players)
        {
            OtherPlayerManager.AddPlayer(p);
        }
        
        chunkBlocksDict[chunkPos] = chunk_posBlockDict;
        GameObject chunkObj = GenerateChunkObj(chunkPos);
        chunk2object[chunkPos] = chunkObj;
    }

    public static void GenerateChunks(List<CSChunk> chunkList)
    {
        foreach (CSChunk chunk in chunkList)
        {
            GenerateChunk(chunk);
        }
    }

    static GameObject GenerateChunkObj(Vector2Int chunkPos)
    {
        GameObject chunk = new GameObject("chunk (" + chunkPos.x + "," + chunkPos.y + ")");
        MeshFilter mf = chunk.AddComponent<MeshFilter>();

        mf.mesh = ChunkMeshGenerator.GenerateMesh(chunkBlocksDict[chunkPos]);
        MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("merge-test/block");

        chunk.AddComponent<MeshCollider>();
        chunk.tag = "Block";
        chunk.layer = LayerMask.NameToLayer("Block");
        return chunk;
    }

    static void RefreshChunk(Vector2Int chunkPos)
    {
        GameObject chunkObj = chunk2object[chunkPos];
        MeshFilter mf = chunkObj.GetComponent<MeshFilter>();
        mf.mesh = ChunkMeshGenerator.GenerateMesh(chunkBlocksDict[chunkPos]);
        MeshCollider mc = chunkObj.GetComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;
    }

    public static void RemoveChunk(Vector2Int chunk)
    {
        if (chunk2object.ContainsKey(chunk))
        {
            GameObject obj = chunk2object[chunk];
            chunk2object.Remove(chunk);
            chunkBlocksDict.Remove(chunk);
            Destroy(obj);
        }
    }
}
