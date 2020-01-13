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
        
        List<Vector2Int> preloadChunks = Utilities.GetSurroudingChunks(PlayerController.GetCurrentChunk());
        ChunkManager.ChunksEnterLeaveViewReq(preloadChunks);
    }

    void Update()
    {
        if (PlayerController.isInitialized)
        {
            ChunkChecker.Update();
        }
    }
}
