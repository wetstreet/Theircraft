using System.Collections.Generic;
using UnityEngine;

// check if player has moved to another chunk, if so, then ask for new chunk data
public static class ChunkChecker
{
    static int lastRenderDistance;

    // the chunk player's at with the last server data
    static Vector2Int lastChunk;

    // the chunk player's at when started refreshing chunks data.
    // because player may move to another chunk in the process of refreshing chunks data,
    // so we need to know what chunk the player's at when the refresh started.
    static Vector2Int tmpChunk;
    
    static bool isRefreshing = false;

    public static void Init()
    {
        isRefreshing = false;

        lastRenderDistance = 0;
        lastChunk = PlayerController.GetCurrentChunkPos();

        tmpChunk = lastChunk;

        List<Vector2Int> preloadChunks = Utilities.GetSurroudingChunks(lastChunk, 1);
        ChunkManager.PreloadChunks(preloadChunks);
    }
    
    public static void Update()
    {
        UnityEngine.Profiling.Profiler.BeginSample("ChunkChecker.Update");

        if (isRefreshing)
        {
            return;
        }

        Vector2Int curChunk = PlayerController.GetCurrentChunkPos();

        // if player moved to another chunk or render distance is changed, then try to refresh chunks data.
        if (lastChunk != curChunk || lastRenderDistance != SettingsPanel.RenderDistance)
        {
            isRefreshing = true;
            tmpChunk = curChunk;
            lastRenderDistance = SettingsPanel.RenderDistance;

            // only load chunks in render distance (if render distance is greater than 6, then load chunks in 6)
            // and unload chunks out of render distance
            List<Vector2Int> loadChunks = NBTHelper.GetLoadChunks(curChunk);
            List<Vector2Int> unloadChunks = NBTHelper.GetUnloadChunks(curChunk, SettingsPanel.RenderDistance);

            if (loadChunks.Count > 0 || unloadChunks.Count > 0)
            {
                ChunkManager.ChunksEnterLeaveView(loadChunks, unloadChunks);
            }
            else
            {
                FinishRefresh();
            }
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public static void FinishRefresh()
    {
        lastChunk = tmpChunk;
        isRefreshing = false;
    }
}
