using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using protocol.cs_theircraft;

public class TerrainGenerator : MonoBehaviour{

    static Dictionary<Vector2Int, GameObject> blockmap = new Dictionary<Vector2Int, GameObject>();

    static int scale = 35;
    static int maxHeight = 15;

    static TerrainGenerator instance;

    struct ChunkBlocks
    {
        public Vector2Int chunk;
        public CSBlock[] blocks;
    }
    static List<ChunkBlocks> waitForGenerateList = new List<ChunkBlocks>();
    static GameObject boxColliderPrefab;
    static EntityManager manager;
    static EntityArchetype blockArchetype;

    public static void Init()
    {
        instance = new GameObject("TerrainGenerator").AddComponent<TerrainGenerator>();
        boxColliderPrefab = Resources.Load<GameObject>("Prefabs/boxcollider");
        manager = World.Active.GetOrCreateManager<EntityManager>();
        blockArchetype = manager.CreateArchetype(typeof(Position));
    }

    private void Start()
    {
        StartCoroutine(GenerateCoroutine());
    }

    public static void AddToGenerateList(Vector2Int chunk, CSBlock[] blockArray)
    {
        ChunkBlocks cb = new ChunkBlocks
        {
            chunk = chunk,
            blocks = blockArray
        };
        waitForGenerateList.Add(cb);
    }

    IEnumerator GenerateCoroutine()
    {
        yield return null;
        while (true)
        {
            if (waitForGenerateList.Count > 0)
            {
                ChunkBlocks cb = waitForGenerateList[0];
                waitForGenerateList.RemoveAt(0);
                Transform chunkParent = GenerateChunkParent(cb.chunk);
                int count = 0;
                foreach (CSBlock block in cb.blocks)
                {
                    GenerateBlock(new Vector3(block.position.x, block.position.y, block.position.z), block.type);
                    count++;
                    if (count > 1000)
                    {
                        yield return new WaitForEndOfFrame();
                        count = 0;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
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

    public static void GenerateBlock(Vector3 pos, CSBlockType blockType = CSBlockType.Grass)
    {
        Vector2Int chunk = Ultiities.GetChunk(pos);
        GameObject prefab = BlockGenerator.GetBlockPrefab(blockType);
        var entity = manager.Instantiate(prefab);
        manager.SetComponentData(entity, new Position { Value = new float3(pos.x, pos.y, pos.z) });
        manager.AddComponentData(entity, new Chunk { x = chunk.x, z = chunk.y });

        GameObject obj = Instantiate(boxColliderPrefab);
        if (!blockmap.ContainsKey(chunk))
        {
            GenerateChunkParent(chunk);
        }
        obj.transform.parent = blockmap[chunk].transform;
        obj.transform.name = string.Format("block({0},{1},{2})", pos.x, pos.y, pos.z);
        obj.transform.localPosition = pos;
    }


    public static Transform GenerateChunkParent(Vector2Int chunk)
    {
        Transform chunkParent = new GameObject(string.Format("chunk({0},{1})", chunk.x, chunk.y)).transform;
        chunkParent.parent = instance.transform;
        chunkParent.localPosition = Vector3.zero;
        blockmap[chunk] = chunkParent.gameObject;
        return chunkParent;
    }

    public static void DestroyChunk(Vector2Int chunk)
    {
        GameObject obj = GameObject.Find(string.Format("chunk({0},{1})", chunk.x, chunk.y));
        if (obj != null)
        {
            DestroySystem.AsyncDestroyChunk(chunk);

            Destroy(obj);
            blockmap.Remove(chunk);
        }
    }
}
