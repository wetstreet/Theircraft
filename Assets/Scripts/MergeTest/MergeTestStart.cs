using System.Collections.Generic;
using UnityEngine;
using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Linq;
using System.Collections;

public class MergeTestStart : MonoBehaviour
{

    Coroutine refreshChunkCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        GameKernel.Create();
        //OtherPlayerManager.Init();
        ItemSelectPanel.Show();
        ChatPanel.ShowChatPanel();
        NetworkManager.Register(ENUM_CMD.CS_CHUNKS_ENTER_LEAVE_VIEW_RES, ChunksEnterLeaveViewRes);
        NetworkManager.Register(ENUM_CMD.CS_PLAYER_MOVE_NOTIFY, OnPlayerMoveNotify);

        Vector2Int curChunk = Utilities.GetChunk(DataCenter.spawnPosition);
        Debug.Log(curChunk);
        List<Vector2Int> preloadChunks = Utilities.GetSurroudingChunks(curChunk);
        ChunksEnterLeaveViewReq(preloadChunks);

        refreshChunkCoroutine = StartCoroutine(RefreshChunkCoroutine());
    }

    private void OnDestroy()
    {
        StopCoroutine(refreshChunkCoroutine);
    }

    Queue<CSChunk> loadQueue = new Queue<CSChunk>();
    Queue<Vector2Int> unloadQueue = new Queue<Vector2Int>();
    IEnumerator RefreshChunkCoroutine()
    {
        yield return null;
        while (true)
        {
            bool changed = false;
            while (unloadQueue.Count > 0)
            {
                test.RemoveChunk(unloadQueue.Dequeue());
                changed = true;
                yield return new WaitForSecondsRealtime(0.1f);
            }
            while (loadQueue.Count > 0)
            {
                test.GenerateChunk(loadQueue.Dequeue());
                changed = true;
                yield return new WaitForSecondsRealtime(0.1f);
            }
            if (changed)
            {
                readyToRefreshChunks = true;
                lastChunk = tempChunk;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(1);
        }
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
                List<Vector2Int> beforeChunks = Utilities.GetSurroudingChunks(lastChunk);
                List<Vector2Int> afterChunks = Utilities.GetSurroudingChunks(curChunk);
                List<Vector2Int> toLoadChunks = afterChunks.Except(beforeChunks).ToList();
                List<Vector2Int> toUnloadChunks = beforeChunks.Except(afterChunks).ToList();
                Debug.Log(curChunk + "," + lastChunk + "," + toLoadChunks.Count + "," + toUnloadChunks.Count);

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
            if (!mergetestPlayerController.isInitialized)
            {
                test.GenerateChunks(rsp.EnterViewChunks);
                mergetestPlayerController.Init();

                readyToRefreshChunks = true;
                lastChunk = tempChunk;
            }
            else
            {
                foreach (CSVector2Int csv in rsp.LeaveViewChunks)
                {
                    Vector2Int chunk = Utilities.CSVector2Int_To_Vector2Int(csv);
                    unloadQueue.Enqueue(chunk);
                }
                foreach (CSChunk chunk in rsp.EnterViewChunks)
                {
                    loadQueue.Enqueue(chunk);
                }
            }
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }

    void OnPlayerMoveNotify(byte[] data)
    {
        CSPlayerMoveNotify notify = NetworkManager.Deserialize<CSPlayerMoveNotify>(data);
        //Debug.Log("CSPlayerMoveNotify,id=" + notify.PlayerID + ",(" + notify.Position.x + "," + notify.Position.y + "," + notify.Position.z + ")");
        OtherPlayerManager.MovePlayer(notify.PlayerID, notify.Position, notify.Rotation);
    }
}
