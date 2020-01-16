using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        ChunkPool.Init();
        GameKernel.Create();
        OtherPlayerManager.Init();
        ItemSelectPanel.Show();
        ChatPanel.ShowChatPanel();
        ChunkRefresher.Init();
        
        List<Vector2Int> preloadChunks = Utilities.GetSurroudingChunks(PlayerController.GetCurrentChunk());
        ChunkManager.ChunksEnterLeaveViewReq(preloadChunks);
    }

    public static Queue<Chunk> rebuildQueue = new Queue<Chunk>();
    void Update()
    {
        if (PlayerController.isInitialized)
        {
            ChunkChecker.Update();

            // rebuild one chunk per frame
            if (rebuildQueue.Count > 0)
            {
                Chunk chunk = rebuildQueue.Dequeue();
                chunk.RebuildMesh(false);
            }
        }
    }

    private void OnDestroy()
    {
        ChunkRefresher.Uninit();
    }
}
