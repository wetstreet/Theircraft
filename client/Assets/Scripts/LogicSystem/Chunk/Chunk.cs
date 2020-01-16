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

    public GameObject collidableGO;
    public GameObject nonCollidableGO;

    //public List<Vector3> vertices = new List<Vector3>(capacity);
    //public List<Vector2> uv = new List<Vector2>(capacity);
    //public List<int> triangles = new List<int>(capacity);

    //public List<Vector3> plantVertices = new List<Vector3>(1024);
    //public List<Vector2> plantUV = new List<Vector2>(1024);
    //public List<int> plantTriangles = new List<int>(1024);

    static int capacity1 = 8192;
    public List<Vector3> vertices1 = new List<Vector3>(capacity1);
    public List<Vector2> uv1 = new List<Vector2>(capacity1);
    public List<int> triangles1 = new List<int>(capacity1);

    static int capacity2 = 1024;
    public List<Vector3> vertices2 = new List<Vector3>(capacity2);
    public List<Vector2> uv2 = new List<Vector2>(capacity2);
    public List<int> triangles2 = new List<int>(capacity2);

    public Mesh collidableMesh;
    public Mesh nonCollidableMesh;

    public Chunk()
    {
        gameObject = new GameObject("chunk (" + x + "," + z + ")");
        transform = gameObject.transform;

        collidableMesh = new Mesh();
        collidableMesh.name = "CollidableMesh";

        collidableGO = new GameObject("Collidable");
        collidableGO.transform.parent = transform;
        collidableGO.AddComponent<MeshFilter>().sharedMesh = collidableMesh;
        collidableGO.AddComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Materials/block");
        collidableGO.AddComponent<MeshCollider>().sharedMesh = collidableMesh;
        collidableGO.AddComponent<NavMeshSourceTag>();
        collidableGO.layer = LayerMask.NameToLayer("Chunk");

        nonCollidableMesh = new Mesh();
        nonCollidableMesh.name = "NonCollidableMesh";

        nonCollidableGO = new GameObject("NonCollidable");
        nonCollidableGO.transform.parent = transform;
        nonCollidableGO.AddComponent<MeshFilter>().sharedMesh = nonCollidableMesh;
        nonCollidableGO.AddComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Materials/block");
        nonCollidableGO.AddComponent<MeshCollider>().sharedMesh = nonCollidableMesh;
        nonCollidableGO.layer = LayerMask.NameToLayer("Plant");
    }

    public void SetData(int _x, int _z, byte[] _blocksInByte)
    {
        x = _x;
        z = _z;
        pos.x = x;
        pos.y = z;
        blocksInByte = _blocksInByte;
        gameObject.name = "chunk (" + x + "," + z + ")";
        transform.localPosition = new Vector3(x * 16, 0, z * 16);
        ClearData();
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
    public byte GetBlockByte(int xInChunk, int _y, int zInChunk)
    {
        return blocksInByte[256 * _y + 16 * xInChunk + zInChunk];
    }

    //input is local position
    public CSBlockType GetBlockType(int xInChunk, int _y, int zInChunk)
    {
        return (CSBlockType)GetBlockByte(xInChunk,_y,zInChunk);
    }

    //input is local position
    public void SetBlockType(int xInChunk, int _y, int zInChunk, CSBlockType type)
    {
        blocksInByte[256 * _y + 16 * xInChunk + zInChunk] = (byte)type;
    }

    public bool HasCollidableBlock(int x, int y, int z)
    {
        if (x < 0 || x > 15 || z < 0 || z > 15 || y < 0 || y > 255)
        {
            return false;
        }
        byte type = GetBlockByte(x, y, z);
        return type > 0 && ChunkMeshGenerator.type2texcoords[type].isCollidable;
    }

    //input is local position
    public bool HasOpaqueBlock(int x, int y, int z)
    {
        if (x < 0 || x > 15 || z < 0 || z > 15 || y < 0 || y > 255)
        {
            return false;
        }
        byte type = GetBlockByte(x, y, z);
        return type > 0 && !ChunkMeshGenerator.type2texcoords[type].isTransparent;
    }

    public void RefreshMeshData()
    {
        ChunkMeshGenerator.GenerateMesh(this);
    }

    public void RebuildMesh()
    {
        collidableMesh.Clear();
        collidableMesh.vertices = vertices1.ToArray();
        collidableMesh.uv = uv1.ToArray();
        collidableMesh.triangles = triangles1.ToArray();
        collidableGO.GetComponent<MeshFilter>().sharedMesh = collidableMesh;
        collidableGO.GetComponent<MeshCollider>().sharedMesh = collidableMesh;
        
        nonCollidableMesh.Clear();
        nonCollidableMesh.SetVertices(vertices2);
        nonCollidableMesh.SetUVs(0, uv2);
        nonCollidableMesh.SetTriangles(triangles2, 0);
        nonCollidableGO.GetComponent<MeshFilter>().sharedMesh = nonCollidableMesh;
        nonCollidableGO.GetComponent<MeshCollider>().sharedMesh = nonCollidableMesh;
    }

    public void ClearData()
    {
        collidableGO.GetComponent<MeshFilter>().sharedMesh = null;
        nonCollidableGO.GetComponent<MeshFilter>().sharedMesh = null;
    }
}
