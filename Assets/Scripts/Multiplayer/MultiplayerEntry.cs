using UnityEngine;
using Theircraft;
using System.Collections.Generic;
using System.Linq;

public class MultiplayerEntry : MonoBehaviour {

    bool playerInited = false;

    // Use this for initialization
    void Start()
    {
        GameKernel.Create();
        ChatPanel.ShowChatPanel();
        ItemSelectPanel.Show();

        NetworkManager.Register(CSMessageType.CHUNKS_ENTER_LEAVE_VIEW_RES, ChunksEnterLeaveViewRes);
        List<Vector2Int> preloadChunks = Ultiities.GetSurroudingChunks(Vector2Int.zero);
        ChunksEnterLeaveViewReq(preloadChunks.ToArray());
    }

    Vector2Int lastChunk;
    void Update()
    {
        Vector3 pos = PlayerController.Instance.transform.localPosition;
        Vector2Int chunk = Ultiities.GetChunk(pos);

        if (lastChunk != chunk)
        {
            List<Vector2Int> lastSurroudingChunks = Ultiities.GetSurroudingChunks(lastChunk);
            List<Vector2Int> surroudingChunks = Ultiities.GetSurroudingChunks(chunk);
            List<Vector2Int> loadChunks = surroudingChunks.Except(lastSurroudingChunks).ToList();
            List<Vector2Int> unloadChunks = lastSurroudingChunks.Except(surroudingChunks).ToList();
            ChunksEnterLeaveViewReq(loadChunks.ToArray(),unloadChunks.ToArray());
        }

        lastChunk = chunk;
    }

    void ChunksEnterLeaveViewReq(Vector2Int[] enterViewChunks, Vector2Int[] leaveViewChunks = null)
    {
        CSChunksEnterLeaveViewReq req = new CSChunksEnterLeaveViewReq();

        List<CSVector2Int> enter = new List<CSVector2Int>();
        foreach (Vector2Int chunk in enterViewChunks)
        {
            CSVector2Int c = new CSVector2Int();
            c.x = chunk.x;
            c.y = chunk.y;
            enter.Add(c);
        }
        req.EnterViewChunks = enter;

        if (leaveViewChunks != null)
        {
            List<CSVector2Int> leave = new List<CSVector2Int>();
            foreach (Vector2Int chunk in leaveViewChunks)
            {
                CSVector2Int c = new CSVector2Int();
                c.x = chunk.x;
                c.y = chunk.y;
                leave.Add(c);
            }
            req.LeaveViewChunks = leave.ToArray();
            print(req.LeaveViewChunks.Length);
        }


        NetworkManager.Enqueue(CSMessageType.CHUNKS_ENTER_LEAVE_VIEW_REQ, req);
    }

    void ChunksEnterLeaveViewRes(byte[] data)
    {
        CSChunksEnterLeaveViewRes rsp = NetworkManager.Deserialzie<CSChunksEnterLeaveViewRes>(data);
        Debug.Log("CSChunksEnterLeaveViewRes");
        if (rsp.RetCode == 0)
        {
            foreach (CSChunk chunk in rsp.EnterViewChunks)
            {
                TerrainGenerator.GenerateChunkFromList(new Vector2Int(chunk.Position.x, chunk.Position.y), chunk.Blocks);
            }
            print(rsp.LeaveViewChunks.Length);
            foreach (CSVector2Int chunk in rsp.LeaveViewChunks)
            {
                TerrainGenerator.DestroyChunk(new Vector2Int(chunk.x, chunk.y));
            }
            if (!playerInited)
            {
                PlayerController.Init();
                playerInited = true;
            }
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
