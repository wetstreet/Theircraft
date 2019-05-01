using System.Collections.Generic;
using UnityEngine;
using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Linq;

public class MergeTestStart : MonoBehaviour
{
    int sight = 3;

    // Start is called before the first frame update
    void Start()
    {
        GameKernel.Create();
        ChatPanel.ShowChatPanel();
        ItemSelectPanel.Show();
        NetworkManager.Register(ENUM_CMD.CS_CHUNKS_ENTER_LEAVE_VIEW_RES, ChunksEnterLeaveViewRes);
        NetworkManager.Register(ENUM_CMD.CS_PLAYER_MOVE_NOTIFY, OnPlayerMoveNotify);

        Vector2Int curChunk = Utilities.GetChunk(DataCenter.spawnPosition);
        Debug.Log(curChunk);
        List<Vector2Int> preloadChunks = Utilities.GetNearbyChunks(curChunk, sight);
        ChunksEnterLeaveViewReq(preloadChunks);
    }

    //没回包之前不要请求chunk数据
    bool lastChunkInitialized;
    Vector2Int lastChunk;
    bool readyToRefreshChunks = true;
    Vector2Int tempChunk;
    void Update()
    {
        if (mergetestPlayerController.isInitialized && readyToRefreshChunks)
        {
            Vector2Int curChunk = mergetestPlayerController.GetCurrentChunk();

            if (!lastChunkInitialized)
            {
                lastChunk = curChunk;
                lastChunkInitialized = true;
                return;
            }

            if (lastChunk != curChunk)
            {
                List<Vector2Int> beforeChunks = Utilities.GetNearbyChunks(lastChunk, sight);
                List<Vector2Int> afterChunks = Utilities.GetNearbyChunks(curChunk, sight);
                List<Vector2Int> toLoadChunks = afterChunks.Except(beforeChunks).ToList();
                List<Vector2Int> toUnloadChunks = beforeChunks.Except(afterChunks).ToList();
                Debug.Log(curChunk + "," + toLoadChunks.Count + "," + toUnloadChunks.Count);

                ChunksEnterLeaveViewReq(toLoadChunks, toUnloadChunks);
                readyToRefreshChunks = false;
                tempChunk = curChunk;
            }
        }
    }

    void ChunksEnterLeaveViewReq(List<Vector2Int> enterViewChunks, List<Vector2Int> leaveViewChunks = null)
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

        //Debug.Log("CS_CHUNKS_ENTER_LEVAE_VIEW_REQ," + req.EnterViewChunks.Count + "," + req.LeaveViewChunks.Count);
        NetworkManager.Enqueue(ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ, req);
    }

    void ChunksEnterLeaveViewRes(byte[] data)
    {
        CSChunksEnterLeaveViewRes rsp = NetworkManager.Deserialize<CSChunksEnterLeaveViewRes>(data);

        //Debug.Log("CSChunksEnterLeaveViewRes," + rsp.EnterViewChunks.Count + "," + rsp.LeaveViewChunks.Count);
        if (rsp.RetCode == 0)
        {
            test.GenerateChunk(rsp.EnterViewChunks);

            if (!mergetestPlayerController.isInitialized)
                mergetestPlayerController.Init();

            if (rsp.LeaveViewChunks != null)
            {
                foreach(CSVector2Int csv in rsp.LeaveViewChunks)
                {
                    Vector2Int chunk = Utilities.CSVector2Int_To_Vector2Int(csv);
                    test.RemoveChunk(chunk);
                }
            }
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }

        readyToRefreshChunks = true;
        lastChunk = tempChunk;
    }

    void OnPlayerMoveNotify(byte[] data)
    {
        CSPlayerMoveNotify notify = NetworkManager.Deserialize<CSPlayerMoveNotify>(data);
        //Debug.Log("CSPlayerMoveNotify,id=" + notify.PlayerID + ",(" + notify.Position.x + "," + notify.Position.y + "," + notify.Position.z + ")");
        OtherPlayerManager.MovePlayer(notify.PlayerID, notify.Position, notify.Rotation);
    }
}
