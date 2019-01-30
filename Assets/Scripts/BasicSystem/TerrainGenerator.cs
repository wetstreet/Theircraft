using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Theircraft;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class TerrainGenerator : MonoBehaviour{

    static Dictionary<Vector2Int, GameObject> blockmap = new Dictionary<Vector2Int, GameObject>();

    static LinkedList<Vector2Int> linkedList = new LinkedList<Vector2Int>();

    static int scale = 35;
    static int maxHeight = 15;

    static TerrainGenerator _instance;
    static TerrainGenerator instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TerrainGenerator");
                _instance = obj.AddComponent<TerrainGenerator>();
            }
            return _instance;
        }
    }

    private void Start()
    {
        StartCoroutine(GenerateLoop());
    }

    static EntityManager manager;
    public static void GenerateBlock(Vector3 pos, CSBlockType blockType = CSBlockType.Grass)
    {
        Vector2Int chunk = Ultiities.GetChunk(pos);
        //GameObject obj = BlockGenerator.CreateCube(blockType);
        GameObject prefab = BlockGenerator.GetBlockPrefab(blockType);
        if (manager == null)
            manager = World.Active.GetOrCreateManager<EntityManager>();
        var entity = manager.Instantiate(prefab);
        manager.SetComponentData(entity, new Position { Value = new float3(pos.x, pos.y, pos.z) });

        GameObject obj = new GameObject();
        obj.AddComponent<BoxCollider>();
        if (!blockmap.ContainsKey(chunk))
        {
            GenerateChunkParent(chunk);
        }
        obj.transform.parent = blockmap[chunk].transform;
        obj.transform.name = string.Format("block({0},{1},{2})", pos.x, pos.y, pos.z);
        obj.transform.localPosition = pos;
    }

    public static void ShowChunk(Vector2Int chunk, bool isSync)
    {
        if (blockmap.ContainsKey(chunk))
        {
            blockmap[chunk].SetActive(true);
            return;
        }

        if (isSync)
            GenerateChunk(chunk);
        else
        {
            lock (linkedList)
            {
                linkedList.AddLast(chunk);
            }
        }
    }

    public static bool HideChunk(Vector2Int chunk)
    {
        if (!blockmap.ContainsKey(chunk))
        {
            lock (linkedList)
            {
                if (linkedList.Contains(chunk))
                {
                    linkedList.Remove(chunk);
                    return true;
                }
            }
            return false;
        }

        GameObject obj = blockmap[chunk];
        obj.SetActive(false);
        return true;
    }

    public static Transform GenerateChunkParent(Vector2Int chunk)
    {
        Transform chunkParent = new GameObject(string.Format("chunk({0},{1})", chunk.x, chunk.y)).transform;
        chunkParent.parent = instance.transform;
        chunkParent.localPosition = Vector3.zero;
        blockmap[chunk] = chunkParent.gameObject;
        return chunkParent;
    }

    public static GameObject GenerateChunk(Vector2Int chunk)
    {
        Transform chunkParent = GenerateChunkParent(chunk);
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                float x = (float)0.5 + i + chunk.x * 16;
                float z = (float)0.5 + j + chunk.y * 16;
                float noise = Mathf.PerlinNoise(x / scale, z / scale);
                int height = Mathf.RoundToInt(maxHeight * noise);
                GenerateBlock(new Vector3(x, height, z));
            }
        }
        return chunkParent.gameObject;
    }

    IEnumerator GenerateLoop()
    {
        while (true)
        {
            lock (linkedList)
            {
                if (linkedList.Count > 0)
                {
                    Vector2Int chunk = linkedList.First.Value;
                    linkedList.RemoveFirst();
                    blockmap[chunk] = GenerateChunk(chunk);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    

    public static void HideChunks(List<Vector2Int> list)
    {
        foreach (Vector2Int chunk in list)
        {
            HideChunk(chunk);
        }
    }

    public static void DestroyChunk(Vector2Int chunk)
    {
        GameObject obj = GameObject.Find(string.Format("chunk({0},{1})", chunk.x, chunk.y));
        if (obj != null)
        {
            Destroy(obj);
            blockmap.Remove(chunk);
        }
    }

    public static void ShowChunks(List<Vector2Int> list, bool isSync = false)
    {
        foreach (Vector2Int chunk in list)
        {
            ShowChunk(chunk, isSync);
        }
    }

    //根据服务器数据或者本地数据库的数据来生成方块
    public static GameObject GenerateChunkFromList(Vector2Int chunk, CSBlock[] blockArray)
    {
        Transform chunkParent = GenerateChunkParent(chunk);
        float time1 = Time.realtimeSinceStartup;
        foreach (CSBlock block in blockArray)
        {
            GenerateBlock(new Vector3(block.position.x, block.position.y, block.position.z), block.type);
        }
        Debug.Log("generate time =" + (Time.realtimeSinceStartup - time1));
        return chunkParent.gameObject;
    }

    //根据服务器数据或者本地数据库的数据来生成方块
    public static void GenerateChunksFromList(CSChunk[] chunks)
    {
        foreach (CSChunk chunk in chunks)
        {
            GenerateChunkFromList(new Vector2Int(chunk.Position.x, chunk.Position.y), chunk.Blocks);
        }
    }
}
