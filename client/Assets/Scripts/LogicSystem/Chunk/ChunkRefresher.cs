using System.Collections.Generic;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<NBTChunk> refreshChunkList;

    public static void Init()
    {
        refreshChunkList = new List<NBTChunk>();
    }

    public static void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("ChunkRefresher.Update");

        if (PlayerController.isInitialized && refreshChunkList.Count > 0)
        {
            refreshChunkList.Sort(ChunkComparer.instance);
            NBTChunk chunk = refreshChunkList[0];
            if (PlayerController.GetChunkToFrontDot(chunk) > 0 || PlayerController.IsNearByChunk(chunk))
            {
                chunk.RebuildMesh();
                refreshChunkList.RemoveAt(0);
            }
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public static void Add(NBTChunk chunk)
    {
        refreshChunkList.Add(chunk);
    }

    public static void Remove(NBTChunk chunk)
    {
        refreshChunkList.Remove(chunk);
    }

    public static void ForceRefreshAll()
    {

        lock (refreshChunkList)
        {
            foreach (NBTChunk chunk in refreshChunkList)
            {
                chunk.RebuildMesh();
            }
            refreshChunkList.Clear();
        }
    }
}
