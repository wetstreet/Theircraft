using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<Chunk> refreshChunkList = new List<Chunk>();

    static void RefreshChunks()
    {
        while (true)
        {
            lock (refreshChunkList)
            {
                if (PlayerController.isInitialized && refreshChunkList.Count > 0)
                {
                    refreshChunkList.Sort(ChunkComparer.instance);
                    Chunk chunk = refreshChunkList[0];

                    // do not refresh if chunk is not in front
                    if (PlayerController.GetChunkToFrontDot(chunk) > 0)
                    {
                        chunk.RefreshMeshData();
                        GameStart.rebuildQueue.Enqueue(chunk);
                        refreshChunkList.RemoveAt(0);
                    }
                }
            }
        }
    }

    static Thread chunkRefreshThread;
    static Coroutine rebuildCoroutine;
    public static void Init()
    {
        chunkRefreshThread = new Thread(RefreshChunks);
        chunkRefreshThread.Start();
        
    }

    public static void Uninit()
    {
        chunkRefreshThread.Abort();
    }

    public static void Add(Chunk chunk)
    {
        refreshChunkList.Add(chunk);
    }

    public static void Remove(Chunk chunk)
    {
        refreshChunkList.Remove(chunk);
    }

    public static void ForceRefreshAll()
    {

        lock (refreshChunkList)
        {
            foreach (Chunk chunk in refreshChunkList)
            {
                chunk.RefreshMeshData();
                chunk.RebuildMesh();
            }
            refreshChunkList.Clear();
        }
    }
}
