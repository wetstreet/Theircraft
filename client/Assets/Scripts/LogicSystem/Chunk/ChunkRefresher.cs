using System.Collections.Generic;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<Chunk> refreshChunkList = new List<Chunk>();

    public static void Update()
    {
        if (PlayerController.isInitialized && refreshChunkList.Count > 0)
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
