using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public class Chunk
{
    public int x;
    public int z;
    public Transform transform;
    public GameObject gameObject;
    public byte[] blocksInByte = new byte[65536];

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    public Chunk()
    {
        GenerateGameObject();
    }

    public void SetData(int _x, int _z, byte[] _blocksInByte)
    {
        x = _x;
        z = _z;
        blocksInByte = _blocksInByte;
    }

    public CSBlockType GetBlockType(int x, int y, int z)
    {
        return (CSBlockType)blocksInByte[256 * y + 16 * x + z];
    }

    public void SetBlockType(int x, int y, int z, CSBlockType type)
    {
        blocksInByte[256 * y + 16 * x + z] = (byte)type;
    }

    public bool HasBlock(int x, int y, int z)
    {
        if (x < 0 || x > 15 || z < 0 || z > 15 || y < 0 || y > 255)
        {
            return false;
        }
        return GetBlockType(x, y, z) != CSBlockType.None;
    }

    public void RebuildMesh()
    {
        Mesh mesh = ChunkMeshGenerator.GenerateMesh(this);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void GenerateGameObject()
    {
        if (gameObject != null)
        {
            return;
        }

        gameObject = new GameObject("chunk (" + x + "," + z + ")");
        transform = gameObject.transform;
        meshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("merge-test/block");

        meshCollider = gameObject.AddComponent<MeshCollider>();
        gameObject.tag = "Chunk";
        gameObject.layer = LayerMask.NameToLayer("Chunk");
    }
}

public class ChunkPool
{
    static Queue<Chunk> chunks = new Queue<Chunk>(100);
    static GameObject instance;
    static GameObject chunkParent;

    public static void Init()
    {
        instance = new GameObject("ChunkPool");
        chunkParent = new GameObject("Chunks");
        instance.transform.localPosition = new Vector3(0, -100, 0);
        for (int i = 0; i < 100; i++)
        {
            Chunk chunk = new Chunk();
            chunk.transform.parent = instance.transform;
            chunks.Enqueue(chunk);
        }
    }

    public static Chunk GetChunk()
    {
        Chunk chunk = chunks.Dequeue();
        chunk.transform.parent = chunkParent.transform;
        chunk.transform.localPosition = Vector3.zero;
        return chunk;
    }

    public static void Recover(Chunk chunk)
    {
        chunk.transform.parent = instance.transform;
        chunk.transform.localPosition = Vector3.zero;
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
