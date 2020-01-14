using System.Collections.Generic;
using UnityEngine;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<Chunk> refreshChunkList = new List<Chunk>();
    // refresh a chunk per 3 frames
    static int refreshInterval = 3;
    static int refreshCounter;
    public static void Update()
    {
        refreshCounter = ++refreshCounter % refreshInterval;
        if (refreshCounter != 0)
        {
            return;
        }

        if (refreshChunkList.Count > 0)
        {
            refreshChunkList.Sort(ChunkComparer.instance);
            Chunk chunk = refreshChunkList[0];
            //Debug.Log(chunk.transform.name);

            // do not refresh if chunk is not in front
            if (PlayerController.GetChunkToFrontDot(chunk) > 0)
            {
                chunk.RebuildMesh();
                refreshChunkList.RemoveAt(0);
            }
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
