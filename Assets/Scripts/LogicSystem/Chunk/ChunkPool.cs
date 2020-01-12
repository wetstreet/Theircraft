using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPool
{
    static Queue<Chunk> chunks = new Queue<Chunk>(1000);
    static GameObject instance;
    static GameObject chunkParent;

    public static void Init()
    {
        instance = new GameObject("ChunkPool");
        chunkParent = new GameObject("Chunks");
        instance.transform.localPosition = new Vector3(0, -100, 0);
        for (int i = 0; i < 1000; i++)
        {
            Chunk chunk = new Chunk();
            chunk.transform.parent = instance.transform;
            chunks.Enqueue(chunk);
        }
    }

    public static Chunk GetChunk()
    {
        Chunk chunk = chunks.Dequeue();
        chunk.transform.parent = chunkParent.transform;
        chunk.transform.localPosition = Vector3.zero;
        return chunk;
    }

    public static void Recover(Chunk chunk)
    {
        chunk.transform.parent = instance.transform;
        chunk.transform.localPosition = Vector3.zero;
        chunks.Enqueue(chunk);
    }
}