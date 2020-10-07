using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        SettingsPanel.Init();
        ChunkChecker.Init();
        ChunkRefresher.Init();
        ChunkManager.Init();
        ChunkPool.Init();
        OtherPlayerManager.Init();
        ItemSelectPanel.Show();
        ChatPanel.ShowChatPanel();

        //List<Vector2Int> preloadChunks = Utilities.GetSurroudingChunks(PlayerController.GetCurrentChunkPos());
        //ChunkManager.ChunksEnterLeaveViewReq(preloadChunks);
        Vector2Int chunkPos = PlayerController.GetCurrentChunkPos();
        int radius = SettingsPanel.RenderDistance;
        NBTHelper.save = "New World1";
        for (int _x = chunkPos.x - radius; _x <= chunkPos.x + radius; _x++)
        {
            for (int _z = chunkPos.y - radius; _z <= chunkPos.y + radius; _z++)
            {
                NBTChunk chunk = NBTHelper.GetChunk(_x, _z);
                chunk.RebuildMesh();
            }
        }
        PlayerController.Init();
    }

    public static Queue<Chunk> rebuildQueue = new Queue<Chunk>();
    void Update()
    {
        if (PlayerController.isInitialized)
        {
            ChunkChecker.Update();
            //ChunkRefresher.Update();
        }
    }

    private void OnDestroy()
    {
        ChunkManager.Uninit();
        ChunkPool.Uninit();
        InputManager.Destroy();
        UISystem.DestroyUIRoot();
        PlayerController.Destroy();
    }
}
