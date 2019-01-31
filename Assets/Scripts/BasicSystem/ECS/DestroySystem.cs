using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections.Generic;
using Theircraft;

public class DestroySystem : ComponentSystem
{
    struct BlockGroup
    {
        [ReadOnly]
        public readonly int Length;
        [ReadOnly]
        public EntityArray entity;
        [ReadOnly]
        public ComponentDataArray<Position> positions;
        [ReadOnly]
        public ComponentDataArray<Chunk> chunks;
    }
        
    [Inject]
    BlockGroup blockGroup;


    static bool needDestroy;

    static HashSet<Chunk> waitForDestroyChunkSet = new HashSet<Chunk>();
    public static void AsyncDestroyChunk(Vector2Int chunk)
    {
        Chunk c = new Chunk
        {
            x = chunk.x,
            z = chunk.y
        };
        waitForDestroyChunkSet.Add(c);
        needDestroy = true;
    }
    static HashSet<Position> waitForDestroyBlockSet = new HashSet<Position>();
    public static void AsyncDestroyBlock(CSVector3Int block)
    {
        Position pos = new Position
        {
            Value = new float3(block.x, block.y, block.z)
        };
        waitForDestroyBlockSet.Add(pos);
        needDestroy = true;
    }

    protected override void OnUpdate()
    {
        if (!needDestroy)
            return;
        for (int i = 0; i < blockGroup.Length; i++)
        {
            if (waitForDestroyChunkSet.Contains(blockGroup.chunks[i]) ||
                waitForDestroyBlockSet.Contains(blockGroup.positions[i]))
            {
                PostUpdateCommands.DestroyEntity(blockGroup.entity[i]);
            }
        }
        waitForDestroyChunkSet.Clear();
        waitForDestroyBlockSet.Clear();
        needDestroy = false;
    }
}
