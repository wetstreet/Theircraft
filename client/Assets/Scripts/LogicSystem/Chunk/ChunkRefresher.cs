using System.Collections.Generic;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<Chunk> refreshChunkList;

    public static void Init()
    {
        refreshChunkList = new List<Chunk>();
    }

    public static void Update()
    {
        if (PlayerController.isInitialized && refreshChunkList.Count > 0)
        {
            refreshChunkList.Sort(ChunkComparer.instance);
            Chunk chunk = refreshChunkList[0];
            if (PlayerController.GetChunkToFrontDot(chunk) > 0 || PlayerController.IsNearByChunk(chunk))
            {
                chunk.RebuildMesh();
                refreshChunkList.RemoveAt(0);
            }
        }
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
                chunk.RebuildMesh();
            }
            refreshChunkList.Clear();
        }
    }
}
