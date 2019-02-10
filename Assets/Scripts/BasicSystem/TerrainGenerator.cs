using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;
using System.Collections;

public class TerrainGenerator : MonoBehaviour{

    static Dictionary<Vector2Int, Transform> blockmap = new Dictionary<Vector2Int, Transform>();

    static Dictionary<Vector2Int, List<Block>> chunk2BlocksDict = new Dictionary<Vector2Int, List<Block>>();
    static Dictionary<Vector3Int, Block> pos2BlockDict = new Dictionary<Vector3Int, Block>();

    static readonly int scale = 35;
    static readonly int maxHeight = 15;

    static TerrainGenerator instance;

    public static void Init()
    {
        Block.Init();
        instance = new GameObject("TerrainGenerator").AddComponent<TerrainGenerator>();
    }

    static List<Block> needDestroyBlockList = new List<Block>();
    static List<Vector2Int> needRefreshChunkList = new List<Vector2Int>();
    IEnumerator ChunkRefresher()
    {
        while (true)
        {
            if (needDestroyBlockList.Count > 0)
            {
                foreach (Block block in needDestroyBlockList)
                {
                    block.Destroy();
                }
                needDestroyBlockList.Clear();
                yield return null;
            }
            if (needRefreshChunkList.Count > 0)
            {
                Vector2Int chunk = needRefreshChunkList[0];
                needRefreshChunkList.RemoveAt(0);
                if (chunk2BlocksDict.ContainsKey(chunk))
                {
                    int count = 0;
                    //这里先留个坑，这个列表应该复制一份，否则遍历到一半列表被更改了就会报错。
                    foreach (Block block in chunk2BlocksDict[chunk])
                    {
                        if (IsBlockExposed(block.pos))
                        {
                            block.Generate();
                            count++;
                        }
                        if (count == 100)
                        {
                            count = 0;
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return null;
        }
    }

    private void Start()
    {
        StartCoroutine(ChunkRefresher());
    }

    public static void SetChunksData(List<CSChunk> chunkList)
    {
        foreach (CSChunk chunk in chunkList)
        {
            Vector2Int chunkPos = Ultiities.CSVector2Int_To_Vector2Int(chunk.Position);
            List<Block> list = new List<Block>();
            foreach (CSBlock csblock in chunk.Blocks)
            {
                Vector3Int blockPos = Ultiities.CSVector3Int_To_Vector3Int(csblock.position);
                Block block = new Block(chunkPos, blockPos, csblock.type);
                list.Add(block);
                pos2BlockDict[blockPos] = block;
            }
            chunk2BlocksDict[chunkPos] = list;
        }
    }

    public static void DestroyChunks(List<CSVector2Int> chunkPosList)
    {
        foreach (CSVector2Int csChunkPos in chunkPosList)
        {
            Vector2Int chunkPos = Ultiities.CSVector2Int_To_Vector2Int(csChunkPos);
            foreach (Block block in chunk2BlocksDict[chunkPos])
            {
                pos2BlockDict.Remove(block.pos);
                //太卡了，改成异步
                //block.Destroy();
                needDestroyBlockList.Add(block);
            }
            chunk2BlocksDict.Remove(chunkPos);
        }
    }

    public static void RefreshChunksAsync(List<Vector2Int> list)
    {
        needRefreshChunkList.AddRange(list);
    }

    public static void SetBlockData(CSBlock csblock)
    {
        Vector3Int blockPos = Ultiities.CSVector3Int_To_Vector3Int(csblock.position);
        Vector2Int chunkPos = Ultiities.GetChunk(blockPos);
        Block block = new Block(chunkPos, blockPos, csblock.type);
        chunk2BlocksDict[chunkPos].Add(block);
        pos2BlockDict[blockPos] = block;
    }

    public static void DestroyBlock(Vector3Int pos)
    {
        if (!pos2BlockDict.ContainsKey(pos))
            return;
        
        Block block = pos2BlockDict[pos];
        chunk2BlocksDict[block.chunkPos].Remove(block);
        pos2BlockDict.Remove(block.pos);
        block.Destroy();
    }

    public static void RefreshNearbyBlocks(Vector3Int blockPos)
    {
        List<Block> nearByBlocks = new List<Block>();
        if (pos2BlockDict.ContainsKey(blockPos))
            nearByBlocks.Add(pos2BlockDict[blockPos]);
        if (pos2BlockDict.ContainsKey(blockPos + Vector3Int.up))
            nearByBlocks.Add(pos2BlockDict[blockPos + Vector3Int.up]);
        if (pos2BlockDict.ContainsKey(blockPos + Vector3Int.down))
            nearByBlocks.Add(pos2BlockDict[blockPos + Vector3Int.down]);
        if (pos2BlockDict.ContainsKey(blockPos + Vector3Int.left))
            nearByBlocks.Add(pos2BlockDict[blockPos + Vector3Int.left]);
        if (pos2BlockDict.ContainsKey(blockPos + Vector3Int.right))
            nearByBlocks.Add(pos2BlockDict[blockPos + Vector3Int.right]);
        if (pos2BlockDict.ContainsKey(blockPos + new Vector3Int(0, 0, 1)))
            nearByBlocks.Add(pos2BlockDict[blockPos + new Vector3Int(0, 0, 1)]);
        if (pos2BlockDict.ContainsKey(blockPos + new Vector3Int(0, 0, -1)))
            nearByBlocks.Add(pos2BlockDict[blockPos + new Vector3Int(0, 0, -1)]);
        RefreshBlocks(nearByBlocks);
    }

    public static void RefreshBlocks(List<Block> blocks)
    {
        foreach (Block block in blocks)
        {
            if (IsBlockExposed(block.pos))
                block.Generate();
            else
                block.Destroy();
        }
    }

    public static void RefreshAllChunks()
    {
        float time1 = Time.realtimeSinceStartup;
        float timeGenerate = 0;
        float timeDestroy = 0;
        int generateCount = 0;
        int destroyCount = 0;
        foreach (Block block in pos2BlockDict.Values)
        {
            if (IsBlockExposed(block.pos))
            {
                float time2 = Time.realtimeSinceStartup;
                block.Generate();
                generateCount++;
                timeGenerate += Time.realtimeSinceStartup - time2;
            }
            else
            {
                float time2 = Time.realtimeSinceStartup;
                block.Destroy();
                destroyCount++;
                timeDestroy += Time.realtimeSinceStartup - time2;
            }
        }
        Debug.Log("refreshTime=" + (Time.realtimeSinceStartup - time1) + ",generateTime="+timeGenerate+",generateCount="+generateCount+",destroyTime="+timeDestroy+",destroyCount="+destroyCount);
    }

    public static Transform GetChunkParent(Vector2Int chunk, bool createIfNotExist = true)
    {
        if (!blockmap.ContainsKey(chunk) && createIfNotExist)
        {
            Transform chunkParent = new GameObject(string.Format("chunk({0},{1})", chunk.x, chunk.y)).transform;
            chunkParent.parent = instance.transform;
            chunkParent.localPosition = Vector3.zero;
            blockmap[chunk] = chunkParent;
        }
        return blockmap[chunk];
    }

    static Vector3Int posInstance = new Vector3Int();
    static bool IsBlockExposed(Vector3Int pos)
    {
        posInstance.Set(pos.x - 1, pos.y, pos.z);
        if (!pos2BlockDict.ContainsKey(posInstance)) return true;
        posInstance.Set(pos.x + 1, pos.y, pos.z);
        if (!pos2BlockDict.ContainsKey(posInstance)) return true;
        posInstance.Set(pos.x, pos.y - 1, pos.z);
        if (!pos2BlockDict.ContainsKey(posInstance)) return true;
        posInstance.Set(pos.x, pos.y + 1, pos.z);
        if (!pos2BlockDict.ContainsKey(posInstance)) return true;
        posInstance.Set(pos.x, pos.y, pos.z - 1);
        if (!pos2BlockDict.ContainsKey(posInstance)) return true;
        posInstance.Set(pos.x, pos.y, pos.z + 1);
        if (!pos2BlockDict.ContainsKey(posInstance)) return true;
        return false;
    }
}
