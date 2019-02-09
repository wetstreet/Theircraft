using UnityEngine;
using protocol.cs_theircraft;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class Block
{
    public readonly Vector2Int chunkPos;
    public readonly Vector3Int pos;
    public readonly CSBlockType type;

    public Transform colliderTransform;
    public Entity entity;

    public bool IsGenerated { get; private set; }

    static GameObject boxColliderPrefab;
    static EntityManager manager;
    static EntityArchetype blockArchetype;

    public static void Init()
    {
        boxColliderPrefab = Resources.Load<GameObject>("Prefabs/boxcollider");
        manager = World.Active.GetOrCreateManager<EntityManager>();
        blockArchetype = manager.CreateArchetype(typeof(Position));
    }

    public Block(Vector2Int _chunkPos,Vector3Int _blockPos, CSBlockType _type = CSBlockType.Grass)
    {
        chunkPos = _chunkPos;
        pos = _blockPos;
        type = _type;
    }

    public void Generate()
    {
        if (IsGenerated)
            return;
        
        GameObject prefab = BlockGenerator.GetBlockPrefab(type);
        entity = manager.Instantiate(prefab);
        manager.SetComponentData(entity, new Position { Value = new float3(pos.x, pos.y, pos.z) });
        manager.AddComponentData(entity, new Chunk { x = chunkPos.x, z = chunkPos.y });

        colliderTransform = Object.Instantiate(boxColliderPrefab).transform;
        colliderTransform.parent = TerrainGenerator.GetChunkParent(chunkPos);
        colliderTransform.name = string.Format("block({0},{1},{2})", pos.x, pos.y, pos.z);
        colliderTransform.localPosition = pos;

        IsGenerated = true;
    }

    public void Destroy()
    {
        if (!IsGenerated)
            return;

        Object.Destroy(colliderTransform.gameObject);
        manager.DestroyEntity(entity);

        IsGenerated = false;
    }
}
