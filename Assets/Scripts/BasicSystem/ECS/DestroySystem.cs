using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections.Generic;

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


    static HashSet<Chunk> waitForDestroyChunkSet = new HashSet<Chunk>();
    static bool needDestroy;
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


    protected override void OnUpdate()
    {
        if (!needDestroy)
            return;
        for (int i = 0; i < blockGroup.Length; i++)
        {
            if (waitForDestroyChunkSet.Contains(blockGroup.chunks[i]))
            {
                PostUpdateCommands.DestroyEntity(blockGroup.entity[i]);
            }
        }
        waitForDestroyChunkSet.Clear();
        needDestroy = false;
    }
}
