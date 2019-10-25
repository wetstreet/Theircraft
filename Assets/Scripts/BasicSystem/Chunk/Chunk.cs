using UnityEngine;
using protocol.cs_theircraft;
using System.Collections.Generic;

public class Chunk
{
    public int x;
    public int z;
    public Vector2Int pos;      //used for index in dictionary
    public Transform transform;
    public GameObject gameObject;
    public byte[] blocksInByte;

    static int capacity = 8192;

    public List<Vector3> vertices = new List<Vector3>(capacity);
    public List<Vector2> uv = new List<Vector2>(capacity);
    public List<int> triangles = new List<int>(capacity);

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
        pos.x = x;
        pos.y = z;
        blocksInByte = _blocksInByte;
        gameObject.name = "chunk (" + x + "," + z + ")";
    }

    public static int GetChunkPosByGlobalPos(int _x)
    {
        return Mathf.FloorToInt(_x / 16f);
    }

    public int GetXInChunkByGlobalX(int _x)
    {
        return _x - x * 16;
    }

    public int GetZInChunkByGlobalZ(int _z)
    {
        return _z - z * 16;
    }

    //input is local position
    public CSBlockType GetBlockType(int x, int y, int z)
    {
        return (CSBlockType)blocksInByte[256 * y + 16 * x + z];
    }

    //input is local position
    public void SetBlockType(int x, int y, int z, CSBlockType type)
    {
        blocksInByte[256 * y + 16 * x + z] = (byte)type;
    }

    //input is local position
    public bool HasBlock(int x, int y, int z)
    {
        if (x < 0 || x > 15 || z < 0 || z > 15 || y < 0 || y > 255)
        {
            return false;
        }
        return blocksInByte[256 * y + 16 * x + z] > 0;
    }

    public void RebuildMesh()
    {
        Mesh mesh = ChunkMeshGenerator.GenerateMesh(this);
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void ClearData()
    {
        meshFilter.sharedMesh = null;
        meshCollider.sharedMesh = null;
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
