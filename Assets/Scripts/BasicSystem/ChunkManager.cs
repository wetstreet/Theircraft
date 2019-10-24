using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public class Chunk
{
    int x;
    int z;
    GameObject obj;
    public byte[] blocksInByte = new byte[65536];

    public CSBlockType GetBlockType(int x, int y, int z)
    {
        return (CSBlockType)blocksInByte[256 * y + 16 * x + z];
    }

    public void SetBlockType(int x, int y, int z, CSBlockType type)
    {
        blocksInByte[256 * y + 16 * x + z] = (byte)type;
    }

    public void GenerateGameObject()
    {
        if (obj != null)
        {
            return;
        }

        GameObject chunk = new GameObject("chunk (" + x + "," + z + ")");
        MeshFilter mf = chunk.AddComponent<MeshFilter>();

        mf.mesh = ChunkMeshGenerator.GenerateMesh(this);
        MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("merge-test/block");

        chunk.AddComponent<MeshCollider>();
        chunk.tag = "Chunk";
        chunk.layer = LayerMask.NameToLayer("Chunk");
    }
}

class ChunkPool
{
    static Queue<Chunk> chunks = new Queue<Chunk>(100);

    public static Chunk GetChunk()
    {
        return chunks.Dequeue();
    }

    public static void Recover(Chunk chunk)
    {
        chunks.Enqueue(chunk);
    }
}

public class ChunkManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
