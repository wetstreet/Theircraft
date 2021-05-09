using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPool
{
    static Queue<NBTChunk> chunks;
    static GameObject instance;
    static Transform chunkParent;

    static bool isInited = false;

    public static void Init()
    {
        chunks = new Queue<NBTChunk>(1000);
        instance = new GameObject("ChunkPool");
        chunkParent = new GameObject("Chunks").transform;
        instance.transform.localPosition = new Vector3(0, -100, 0);
        for (int i = 0; i < 1000; i++)
        {
            NBTChunk chunk = new NBTChunk();
            Recover(chunk);
        }
        isInited = true;
    }

    public static NBTChunk GetChunk()
    {
        if (!isInited)
        {
            Init();
        }
        NBTChunk chunk = chunks.Dequeue();
        chunk.transform.parent = chunkParent;
        chunk.transform.localPosition = Vector3.zero;
        chunk.gameObject.SetActive(true);
        return chunk;
    }

    public static void Recover(NBTChunk chunk)
    {
        chunk.transform.parent = instance.transform;
        chunk.transform.localPosition = Vector3.zero;
        chunk.gameObject.SetActive(false);
        chunks.Enqueue(chunk);
    }

    public static void Uninit()
    {
        chunks = null;
        isInited = false;
    }
}