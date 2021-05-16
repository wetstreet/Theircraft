using System.Collections.Generic;
using UnityEngine;

// refresh chunk meshes based on priority
public class ChunkRefresher
{
    static List<NBTChunk> refreshChunkList;

    public static void Init()
    {
        refreshChunkList = new List<NBTChunk>();
    }

    static float time;
    static float interval = 0.1f;

    public static void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("ChunkRefresher.Update");

        if (PlayerController.isInitialized && Time.time - time > interval && refreshChunkList.Count > 0)
        {
            // remove empty
            refreshChunkList.RemoveAll((NBTChunk chunk) => { return chunk == null; });

            refreshChunkList.Sort(ChunkComparer.instance);
            NBTChunk chunk = refreshChunkList[0];
            if (PlayerController.GetChunkToFrontDot(chunk) > 0 || PlayerController.IsNearByChunk(chunk))
            {
                chunk.RebuildMeshAsync();
                refreshChunkList.RemoveAt(0);
            }
            time = Time.time;
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
