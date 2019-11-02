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
    GameObject plantGameObject;

    static int capacity = 8192;

    public List<Vector3> vertices = new List<Vector3>(capacity);
    public List<Vector2> uv = new List<Vector2>(capacity);
    public List<int> triangles = new List<int>(capacity);

    public List<Vector3> plantVertices = new List<Vector3>(1024);
    public List<Vector2> plantUV = new List<Vector2>(1024);
    public List<int> plantTriangles = new List<int>(1024);

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    MeshFilter plantMeshFilter;
    MeshCollider plantMeshCollider;

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
        byte type = blocksInByte[256 * y + 16 * x + z];
        return type > 0 && type != (byte)CSBlockType.Grass;
    }

    public void RebuildMesh()
    {
        ChunkMeshGenerator.GenerateMeshData(this);
        Mesh mesh = new Mesh();
        mesh.name = "ChunkMesh";
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

        Mesh plantMesh = new Mesh();
        plantMesh.name = "PlantMesh";
        plantMesh.vertices = plantVertices.ToArray();
        plantMesh.uv = plantUV.ToArray();
        plantMesh.triangles = plantTriangles.ToArray();
        plantMeshFilter.sharedMesh = plantMesh;
        plantMeshCollider.sharedMesh = plantMesh;
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
        mr.material = Resources.Load<Material>("Materials/block");
        meshCollider = gameObject.AddComponent<MeshCollider>();
        gameObject.layer = LayerMask.NameToLayer("Chunk");

        plantGameObject = new GameObject("plant");
        plantGameObject.transform.parent = transform;
        plantMeshFilter = plantGameObject.AddComponent<MeshFilter>();
        plantMeshCollider = plantGameObject.AddComponent<MeshCollider>();
        MeshRenderer plantRenderer = plantGameObject.AddComponent<MeshRenderer>();
        plantRenderer.sharedMaterial = Resources.Load<Material>("Materials/Plant");
        plantGameObject.layer = LayerMask.NameToLayer("Plant");
    }
}
