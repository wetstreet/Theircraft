using UnityEngine;
using Theircraft;

public class MultiplayerEntry : MonoBehaviour {

    bool playerInited = false;

    // Use this for initialization
    void Start ()
    {
        GameKernel.Create();
        ChatPanel.ShowChatPanel();
        ItemSelectPanel.Show();

        NetworkManager.Register(CSMessageType.CHUNK_ENTER_VIEW_RES, ChunkEnterViewRes);
        ChunkEnterViewReq(new Vector2Int(0, 0));
        ChunkEnterViewReq(new Vector2Int(-1, 0));
        ChunkEnterViewReq(new Vector2Int(0, -1));
        ChunkEnterViewReq(new Vector2Int(-1, -1));
    }
	
	// Update is called once per frame
	void Update () {

    }

    void ChunkEnterViewReq(Vector2Int chunk)
    {
        CSChunkEnterViewReq req = new CSChunkEnterViewReq
        {
            ChunkX = chunk.x,
            ChunkZ = chunk.y,
        };
        NetworkManager.Enqueue(CSMessageType.CHUNK_ENTER_VIEW_REQ, req);
    }

    void ChunkEnterViewRes(byte[] data)
    {
        CSChunkEnterViewRes rsp = NetworkManager.Deserialzie<CSChunkEnterViewRes>(data);
        Debug.Log("ChunkInfoRes=" + rsp.RetCode + "," + rsp.ChunkX + "," + rsp.ChunkZ);
        if (rsp.RetCode == 0)
        {
            //for (int i = 0; i < rsp.BlockList.Length; i++)
            //{
            //    print(rsp.BlockList[i].position);
            //}
            TerrainGenerator.GenerateChunkFromList(new Vector2Int(rsp.ChunkX, rsp.ChunkZ), rsp.BlockList);
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
