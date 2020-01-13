using System.Collections.Generic;
using UnityEngine;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<Chunk> refreshChunkList = new List<Chunk>();
    public static void Update()
    {
        if (refreshChunkList.Count > 0)
        {
            refreshChunkList.Sort(ChunkComparer.instance);
            Debug.Log(refreshChunkList[0].transform.name);
        }
    }

    public static void AddRefreshList(Chunk chunk)
    {
        refreshChunkList.Add(chunk);
    }

    public static void ForceRefreshAll()
    {
        foreach (Chunk chunk in refreshChunkList)
        {
            chunk.RebuildMesh();
        }
        refreshChunkList.Clear();
    }
}
