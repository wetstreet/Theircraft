using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public class Chunk
{
    public int x;
    public int z;
    public Vector2Int pos;      //used for index in dictionary
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
        return GetBlockType(x, y, z) != CSBlockType.None;
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
    static readonly Dictionary<Vector2Int, Chunk> chunkDict = new Dictionary<Vector2Int, Chunk>();

    static void AddToChunkDict(Chunk chunk)
    {
        chunkDict.Add(chunk.pos, chunk);
    }

    static void RemoveFromChunkDict(Chunk chunk)
    {
        chunkDict.Remove(chunk.pos);
    }

    //intput is global position
    public static bool HasBlock(int x, int y, int z)
    {
        return GetBlockType(x, y, z) != CSBlockType.None;
    }

    //intput is global position
    public static CSBlockType GetBlockType(int x, int y, int z)
    {
        //get chunk position first
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            //calculate block position in chunk
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            //Debug.Log("GetBlockType,globalblockpos=(" + x + "," + y + "," + z + "),chunkpos=(" + chunk.x + "," + chunk.z + "),blockposinchunk=(" + xInChunk + "," + y + "," + zInChunk + ")");
            return chunk.GetBlockType(xInChunk, y, zInChunk);
        }
        return CSBlockType.None;
    }

    //intput is global position
    public static Chunk GetChunk(int x, int y, int z)
    {
        int chunkX = Chunk.GetChunkPosByGlobalPos(x);
        int chunkZ = Chunk.GetChunkPosByGlobalPos(z);
        return GetChunk(chunkX, chunkZ);
    }

    static Vector2Int keyVec = new Vector2Int();
    public static Chunk GetChunk(int x, int z)
    {
        keyVec.x = x;
        keyVec.y = z;
        if (chunkDict.ContainsKey(keyVec))
        {
            return chunkDict[keyVec];
        }
        return null;
    }

    public static void AddBlock(int x, int y, int z, CSBlockType type)
    {
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            chunk.SetBlockType(xInChunk, y, zInChunk, type);
            chunk.RebuildMesh();
        }
    }

    public static void RemoveBlock(int x, int y, int z)
    {
        Chunk chunk = GetChunk(x, y, z);
        if (chunk != null)
        {
            int xInChunk = chunk.GetXInChunkByGlobalX(x);
            int zInChunk = chunk.GetZInChunkByGlobalZ(z);
            chunk.SetBlockType(xInChunk, y, zInChunk, CSBlockType.None);
            chunk.RebuildMesh();
        }
    }

    public static Dictionary<Vector2Int,Chunk>.KeyCollection GetChunkDictKeys()
    {
        return chunkDict.Keys;
    }

    public static void LoadChunk(CSChunk csChunk)
    {
        //Debug.Log("loadChunk,x=" + csChunk.Position.x + ",z=" + csChunk.Position.y);
        Chunk chunk = ChunkPool.GetChunk();
        chunk.SetData(csChunk.Position.x, csChunk.Position.y, csChunk.BlocksInBytes);
        chunk.RebuildMesh();
        AddToChunkDict(chunk);
    }

    public static void UnloadChunk(CSVector2Int chunkPos)
    {
        UnloadChunk(chunkPos.x, chunkPos.y);
    }

    public static void UnloadChunk(int x, int z)
    {
        //Debug.Log("UnloadChunk,x=" + x + ",z=" + z);
        Chunk chunk = GetChunk(x, z);
        if (chunk != null)
        {
            chunk.ClearData();
            RemoveFromChunkDict(chunk);
            ChunkPool.Recover(chunk);
        }
    }
}
