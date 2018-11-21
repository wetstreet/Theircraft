using UnityEngine;
using Theircraft;

public class MultiplayerEntry : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        NetworkManager.Register(MessageType.CHUNK_INFO_RES, ChunkInfoRes);
    }
	
	// Update is called once per frame
	void Update () {

    }

    void ChunkInfoReq(Vector2Int chunk)
    {
        ChunkInfoReq req = new ChunkInfoReq
        {
            ChunkX = chunk.x,
            ChunkZ = chunk.y,
        };
        NetworkManager.Enqueue(MessageType.CHUNK_INFO_REQ, req);
    }

    void ChunkInfoRes(byte[] data)
    {
        ChunkInfoRes rsp = NetworkManager.Deserialzie<ChunkInfoRes>(data);
        Debug.Log("ChunkInfoRes,retCode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            for(int i = 0; i < rsp.BlockList.Length; i++)
            {
                print(rsp.BlockList[i]);
            }
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
