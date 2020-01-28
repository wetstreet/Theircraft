using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPool
{
    static Queue<Chunk> chunks;
    static GameObject instance;
    static Transform chunkParent;

    public static void Init()
    {
        chunks = new Queue<Chunk>(1000);
        instance = new GameObject("ChunkPool");
        chunkParent = new GameObject("Chunks").transform;
        instance.transform.localPosition = new Vector3(0, -100, 0);
        for (int i = 0; i < 1000; i++)
        {
            Chunk chunk = new Chunk();
            Recover(chunk);
        }
    }

    public static Chunk GetChunk()
    {
        Chunk chunk = chunks.Dequeue();
        chunk.transform.parent = chunkParent;
        chunk.transform.localPosition = Vector3.zero;
        return chunk;
    }

    public static void Recover(Chunk chunk)
    {
        chunk.transform.parent = instance.transform;
        chunk.transform.localPosition = Vector3.zero;
        chunks.Enqueue(chunk);
    }

    public static void Uninit()
    {
        chunks = null;
    }
}