using UnityEngine;
using Theircraft;
using System.Collections.Generic;
using System.Linq;

public class MultiplayerEntry : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        GameKernel.Create();
        ChatPanel.ShowChatPanel();
        ItemSelectPanel.Show();
        TerrainGenerator.Init();

        NetworkManager.Register(8, ChunksEnterLeaveViewRes);
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
        protocol.cs_theircraft.CSChunksEnterLeaveViewReq req = new protocol.cs_theircraft.CSChunksEnterLeaveViewReq();

        List<protocol.cs_theircraft.CSVector2Int> enter = new List<protocol.cs_theircraft.CSVector2Int>();
        foreach (Vector2Int chunk in enterViewChunks)
        {
            protocol.cs_theircraft.CSVector2Int c = new protocol.cs_theircraft.CSVector2Int
            {
                x = chunk.x,
                y = chunk.y
            };
            enter.Add(c);
        }
        req.EnterViewChunks.AddRange(enter);

        if (leaveViewChunks != null)
        {
            List<protocol.cs_theircraft.CSVector2Int> leave = new List<protocol.cs_theircraft.CSVector2Int>();
            foreach (Vector2Int chunk in leaveViewChunks)
            {
                protocol.cs_theircraft.CSVector2Int c = new protocol.cs_theircraft.CSVector2Int
                {
                    x = chunk.x,
                    y = chunk.y
                };
                leave.Add(c);
            }
            req.LeaveViewChunks.AddRange(leave);
        }

        NetworkManager.EnqueueExt(protocol.cs_enum.ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ, req);
    }
    
    void ChunksEnterLeaveViewRes(byte[] data)
    {
        float time1 = Time.realtimeSinceStartup;
        protocol.cs_theircraft.CSChunksEnterLeaveViewRes rsp = NetworkManager.Deserialize<protocol.cs_theircraft.CSChunksEnterLeaveViewRes>(data);
        Debug.Log("deserialize time =" + (Time.realtimeSinceStartup - time1));
        Debug.Log("CSChunksEnterLeaveViewRes");
        if (rsp.RetCode == 0)
        {
            if (!PlayerController.isInitialized)
            {
                foreach (protocol.cs_theircraft.CSChunk chunk in rsp.EnterViewChunks)
                {
                    TerrainGenerator.GenerateChunkFromList(new Vector2Int(chunk.Position.x, chunk.Position.y), chunk.Blocks.ToArray());
                }
                PlayerController.Init();
            }
            else
            {
                //foreach (CSChunk chunk in rsp.EnterViewChunks)
                //{
                //    TerrainGenerator.AddToGenerateList(new Vector2Int(chunk.Position.x, chunk.Position.y), chunk.Blocks);
                //}
                //foreach (CSVector2Int chunk in rsp.LeaveViewChunks)
                //{
                //    TerrainGenerator.DestroyChunk(new Vector2Int(chunk.x, chunk.y));
                //}
            }
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
