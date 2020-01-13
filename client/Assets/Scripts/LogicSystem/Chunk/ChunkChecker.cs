using System.Collections.Generic;
using UnityEngine;

// refresh chunks if neccessary.
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
    
    public static void Update()
    {
        if (isRefreshing)
        {
            return;
        }

        Vector2Int curChunk = PlayerController.GetCurrentChunk();

        // if player moved to another chunk or render distance is changed, then try to refresh chunks data.
        if (lastChunk != curChunk || lastRenderDistance != SettingsPanel.RenderDistance)
        {
            isRefreshing = true;
            tmpChunk = curChunk;
            lastRenderDistance = SettingsPanel.RenderDistance;

            // only load chunks in render distance (if render distance is greater than 6, then load chunks in 6)
            // and unload chunks out of render distance
            List<Vector2Int> loadChunks = ChunkManager.GetLoadChunks(curChunk);
            List<Vector2Int> unloadChunks = ChunkManager.GetUnloadChunks(curChunk, SettingsPanel.RenderDistance);
            Debug.Log(curChunk + "," + lastChunk + "," + loadChunks.Count + "," + unloadChunks.Count);

            if (loadChunks.Count > 0 || unloadChunks.Count > 0)
            {
                ChunkManager.ChunksEnterLeaveViewReq(loadChunks, unloadChunks);
            }
            else
            {
                FinishRefresh();
            }
        }
    }

    public static void FinishRefresh()
    {
        lastChunk = tmpChunk;
        isRefreshing = false;
    }
}
