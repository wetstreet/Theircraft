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
        ChunkPool.Init();
        GameKernel.Create();
        //OtherPlayerManager.Init();
        ItemSelectPanel.Show();
        ChatPanel.ShowChatPanel();
        NetworkManager.Register(ENUM_CMD.CS_PLAYER_MOVE_NOTIFY, OnPlayerMoveNotify);

        Vector2Int curChunk = new Vector2Int();
        Utilities.GetChunk(ref curChunk, DataCenter.spawnPosition);
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
    int lastRenderDistance;
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
                lastRenderDistance = SettingsPanel.RenderDistance;
                lastChunk = curChunk;
                lastChunkInitialized = true;
                return;
            }

            if (lastChunk != curChunk || lastRenderDistance != SettingsPanel.RenderDistance)
            {
                // only load chunks in render distance (if render distance is greater than 6, then load chunks in 6)
                // and unload chunks out of render distance
                List<Vector2Int> haveChunks = test.GetChunkList();
                List<Vector2Int> shouldLoadChunks = Utilities.GetSurroudingChunks(curChunk);
                List<Vector2Int> toLoadChunks = shouldLoadChunks.Except(haveChunks).ToList();
                List<Vector2Int> toUnloadChunks = haveChunks.Where(c => Mathf.Abs(c.x - curChunk.x) > SettingsPanel.RenderDistance || Mathf.Abs(c.y - curChunk.y) > SettingsPanel.RenderDistance).ToList();
                Debug.Log(curChunk + "," + lastChunk + "," + toLoadChunks.Count + "," + toUnloadChunks.Count);

                if (toLoadChunks.Count > 0 || toUnloadChunks.Count > 0)
                {
                    ChunksEnterLeaveViewReq(toLoadChunks, toUnloadChunks);
                    readyToRefreshChunks = false;
                    tempChunk = curChunk;
                }
                else
                {
                    lastChunk = curChunk;
                }
                lastRenderDistance = SettingsPanel.RenderDistance;
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
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ, req, ChunksEnterLeaveViewRes);
    }

    void ChunksEnterLeaveViewRes(byte[] data)
    {
        CSChunksEnterLeaveViewRes rsp = NetworkManager.Deserialize<CSChunksEnterLeaveViewRes>(data);

        //Debug.Log("CSChunksEnterLeaveViewRes," + rsp.EnterViewChunks.Count + "," + rsp.LeaveViewChunks.Count);
        if (rsp.RetCode == 0)
        {
            if (!mergetestPlayerController.isInitialized)
            {
                //test.GenerateChunks(rsp.EnterViewChunks);
                foreach (CSChunk cschunk in rsp.EnterViewChunks)
                {
                    Chunk chunk = ChunkPool.GetChunk();
                    chunk.x = cschunk.Position.x;
                    chunk.z = cschunk.Position.y;
                    chunk.blocksInByte = cschunk.BlocksInBytes;
                    chunk.RebuildMesh();
                }
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
