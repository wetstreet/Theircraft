using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using protocol.cs_enum;
using protocol.cs_theircraft;

public class MultiplayerEntry : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        GameKernel.Create();
        ChatPanel.ShowChatPanel();
        ItemSelectPanel.Show();
        TerrainGenerator.Init();

        NetworkManager.Register(ENUM_CMD.CS_CHUNKS_ENTER_LEAVE_VIEW_RES, ChunksEnterLeaveViewRes);
        List<Vector2Int> preloadChunks = Ultiities.GetSurroudingChunks(Vector2Int.zero);
        ChunksEnterLeaveViewReq(preloadChunks.ToArray());
    }

    void OnDestroy()
    {
        GameKernel.Dispose();
    }

    Vector2Int lastChunk;
    void Update()
    {
        if (PlayerController.isInitialized)
        {
            Vector3 pos = PlayerController.Instance.transform.localPosition;
            Vector2Int chunk = Ultiities.GetChunk(pos);

            if (lastChunk != chunk)
            {
                List<Vector2Int> lastSurroudingChunks = Ultiities.GetSurroudingChunks(lastChunk);
                List<Vector2Int> surroudingChunks = Ultiities.GetSurroudingChunks(chunk);
                List<Vector2Int> loadChunks = surroudingChunks.Except(lastSurroudingChunks).ToList();
                List<Vector2Int> unloadChunks = lastSurroudingChunks.Except(surroudingChunks).ToList();
                ChunksEnterLeaveViewReq(loadChunks.ToArray(), unloadChunks.ToArray());
            }

            lastChunk = chunk;
        }
    }

    void ChunksEnterLeaveViewReq(Vector2Int[] enterViewChunks, Vector2Int[] leaveViewChunks = null)
    {
        CSChunksEnterLeaveViewReq req = new CSChunksEnterLeaveViewReq();

        List<CSVector2Int> enter = new List<CSVector2Int>();
        foreach (Vector2Int chunk in enterViewChunks)
        {
            CSVector2Int c = new CSVector2Int
            {
                x = chunk.x,
                y = chunk.y
            };
            enter.Add(c);
        }
        req.EnterViewChunks.AddRange(enter);

        if (leaveViewChunks != null)
        {
            List<CSVector2Int> leave = new List<CSVector2Int>();
            foreach (Vector2Int chunk in leaveViewChunks)
            {
                CSVector2Int c = new CSVector2Int
                {
                    x = chunk.x,
                    y = chunk.y
                };
                leave.Add(c);
            }
            req.LeaveViewChunks.AddRange(leave);
        }

        NetworkManager.Enqueue(ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ, req);
    }
    
    void ChunksEnterLeaveViewRes(byte[] data)
    {
        CSChunksEnterLeaveViewRes rsp = NetworkManager.Deserialize<CSChunksEnterLeaveViewRes>(data);
        Debug.Log("CSChunksEnterLeaveViewRes");
        if (rsp.RetCode == 0)
        {
            if (!PlayerController.isInitialized)
            {
                foreach (CSChunk chunk in rsp.EnterViewChunks)
                {
                    TerrainGenerator.GenerateChunkFromList(new Vector2Int(chunk.Position.x, chunk.Position.y), chunk.Blocks.ToArray());
                }
                PlayerController.Init();
            }
            else
            {
                foreach (CSChunk chunk in rsp.EnterViewChunks)
                {
                    TerrainGenerator.AddToGenerateList(new Vector2Int(chunk.Position.x, chunk.Position.y), chunk.Blocks.ToArray());
                }
                foreach (CSVector2Int chunk in rsp.LeaveViewChunks)
                {
                    TerrainGenerator.DestroyChunk(new Vector2Int(chunk.x, chunk.y));
                }
            }
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
