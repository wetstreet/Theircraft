using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Threading.Tasks;

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
        ChunksEnterLeaveViewReq(preloadChunks);
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
                ChunksEnterLeaveViewReq(loadChunks, unloadChunks);
            }

            lastChunk = chunk;
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
    
    async void ChunksEnterLeaveViewRes(byte[] data)
    {
        //反序列化太卡了，放在别的线程处理
        CSChunksEnterLeaveViewRes rsp = null;
        await Task.Run(() => {
            rsp = NetworkManager.Deserialize<CSChunksEnterLeaveViewRes>(data);
        });

        //Debug.Log("CSChunksEnterLeaveViewRes," + rsp.EnterViewChunks.Count + "," + rsp.LeaveViewChunks.Count);
        if (rsp.RetCode == 0)
        {
            if (!PlayerController.isInitialized)
            {
                TerrainGenerator.SetChunksData(rsp.EnterViewChunks);
                TerrainGenerator.RefreshAllChunks();
                PlayerController.Init();
            }
            else
            {
                TerrainGenerator.SetChunksData(rsp.EnterViewChunks);
                TerrainGenerator.DestroyChunks(rsp.LeaveViewChunks);

                List<Vector2Int> needRefreshList = new List<Vector2Int>();
                foreach(CSChunk cschunk in rsp.EnterViewChunks)
                {

                    needRefreshList.Add(Ultiities.CSVector2Int_To_Vector2Int(cschunk.Position));
                }
                TerrainGenerator.RefreshChunksAsync(needRefreshList);
            }
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
