using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<Chunk> refreshChunkList = new List<Chunk>();

    static void RefreshChunks()
    {
        if (refreshChunkList.Count > 0)
        {
            refreshChunkList.Sort(ChunkComparer.instance);
            Chunk chunk = refreshChunkList[0];

            // do not refresh if chunk is not in front
            if (PlayerController.GetChunkToFrontDot(chunk) > 0)
            {
                chunk.RebuildMesh();
                refreshChunkList.RemoveAt(0);
            }
        }
    }

    static Thread chunkRefreshThread;
    public static void Init()
    {
        chunkRefreshThread = new Thread(RefreshChunks);
        chunkRefreshThread.Start();
    }

    public static void Uninit()
    {
        chunkRefreshThread.Abort();
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
