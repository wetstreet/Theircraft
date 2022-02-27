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

    public static int GetChunkUpdatesCount()
    {
        return refreshChunkList.Count;
    }

    static NBTChunk GetFirstChunk(List<NBTChunk> list)
    {
        NBTChunk maxChunk = null;
        float maxDot = -1;
        float minDistance = 1000;

        for (int i = 0; i < list.Count; i++)
        {
            NBTChunk chunk = list[i];
            float dot = PlayerController.GetChunkToFrontDot(chunk);
            float distance = PlayerController.GetChunkDistance(chunk);
            if (dot > 0.3f && distance <= minDistance)
            {
                if (distance < minDistance || dot > maxDot)
                {
                    maxChunk = list[i];
                    maxDot = dot;
                    minDistance = distance;
                }
            }
        }
        return maxChunk;
    }

    public static void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("ChunkRefresher.Update");

        if (PlayerController.isInitialized && Time.time - time > interval && refreshChunkList.Count > 0)
        {
            // remove empty
            refreshChunkList.RemoveAll((NBTChunk chunk) => { return chunk == null; });

            NBTChunk chunk = GetFirstChunk(refreshChunkList);
            if (chunk != null)
            {
                chunk.RebuildMeshAsync();
                //chunk.RebuildMesh();
                refreshChunkList.Remove(chunk);
            }

            time = Time.time;
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public static void Add(List<NBTChunk> chunk)
    {
        refreshChunkList.AddRange(chunk);
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
        foreach (NBTChunk chunk in refreshChunkList)
        {
            chunk.RebuildMesh(UpdateFlags.All);
        }
        refreshChunkList.Clear();
    }
}
