using UnityEngine;
using protocol.cs_theircraft;
using System.Collections.Generic;

public class Chunk
{
    public int x;
    public int z;
    public int globalX;
    public int globalZ;
    public Vector2Int pos;      //used for index in dictionary
    public Transform transform;
    public GameObject gameObject;
    public byte[] blocksInByte;
    public bool isDirty = false;

    static int capacity = 8192;

    public GameObject collidableGO;
    public GameObject nonCollidableGO;

    static int capacity1 = 8192;
    public List<Vector3> vertices1 = new List<Vector3>(capacity1);
    public List<Color> colors1 = new List<Color>(capacity1);
    public List<Vector2> uv1 = new List<Vector2>(capacity1);
    public List<int> triangles1 = new List<int>(capacity1);

    static int capacity2 = 1024;
    public List<Vector3> vertices2 = new List<Vector3>(capacity2);
    public List<Vector2> uv2 = new List<Vector2>(capacity2);
    public List<int> triangles2 = new List<int>(capacity2);

    public Mesh collidableMesh;
    public Mesh nonCollidableMesh;

    public bool hasBuiltMesh = false;
    public int lightGenerationCount = 0;
    public int meshBuildCount = 0;

    public List<Vector3Int> torchList = new List<Vector3Int>();

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
        globalX = x * 16;
        globalZ = z * 16;
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

    //input is global position
    public void SetBlockTypeByGlobalPosition(int _x, int _y, int _z, CSBlockType type)
    {
        int xInChunk = GetXInChunkByGlobalX(_x);
        int zInChunk = GetZInChunkByGlobalZ(_z);
        //blocksInByte[256 * _y + 16 * xInChunk + zInChunk] = (byte)type;
        SetBlockType(xInChunk, _y, zInChunk, type);
    }

    void SetNearbyChunkDirty(int xInChunk, int zInChunk)
    {
        if (xInChunk == 0)
        {
            ChunkManager.SetChunkDirty(x - 1, z);
        }
        else if (xInChunk == 15)
        {
            ChunkManager.SetChunkDirty(x + 1, z);
        }
        if (zInChunk == 0)
        {
            ChunkManager.SetChunkDirty(x, z - 1);
        }
        else if (zInChunk == 15)
        {
            ChunkManager.SetChunkDirty(x, z + 1);
        }
    }

    //input is local position
    //public void RemoveBlock(Vector3Int pos)
    //{
    //    SetBlockTypeByGlobalPosition(pos.x, pos.y, pos.z, CSBlockType.None);
    //}

    //input is local position
    public void SetBlockType(int xInChunk, int _y, int zInChunk, CSBlockType type)
    {
        blocksInByte[256 * _y + 16 * xInChunk + zInChunk] = (byte)type;
        isDirty = true;
        SetNearbyChunkDirty(xInChunk, zInChunk);
    }

    public bool HasNotCollidableBlock(int x, int y, int z)
    {
        if (x < 0 || x > 15 || z < 0 || z > 15 || y < 0 || y > 255)
        {
            return false;
        }
        byte type = GetBlockByte(x, y, z);
        return type > 0 && !ChunkMeshGenerator.type2texcoords[type].isCollidable;
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

    public bool HasOpaqueBlock(Vector3Int pos)
    {
        return HasOpaqueBlock(pos.x, pos.y, pos.z);
    }

    public void RebuildMesh(bool forceRefreshMeshData = true)
    {
        if (forceRefreshMeshData || isDirty)
        {
            //UpdateLighting();
            this.RefreshMeshData();
        }

        collidableMesh.Clear();
        collidableMesh.SetVertices(vertices1);
        //collidableMesh.SetColors(colors1);
        collidableMesh.SetUVs(0, uv1);
        //collidableMesh.SetNormals(normals1);
        collidableMesh.SetTriangles(triangles1, 0);
        collidableMesh.RecalculateNormals();
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
        hasBuiltMesh = false;
        lightGenerationCount = 0;
        meshBuildCount = 0;
        foreach (Vector3Int torchPos in torchList)
        {
            TorchMeshGenerator.Instance.RemoveTorchAt(torchPos);
        }
        torchList.Clear();
        collidableGO.GetComponent<MeshFilter>().sharedMesh = null;
        nonCollidableGO.GetComponent<MeshFilter>().sharedMesh = null;
    }

    public void AddTorch(Vector3Int globalPos)
    {
        TorchMeshGenerator.Instance.AddTorchAt(globalPos);
        torchList.Add(globalPos);
    }

    byte[] lights = new byte[65536];

    void ClearLights()
    {
        for (int i = 0; i < 65536; i++)
        {
            lights[i] = 0;
        }
    }

    public int GetLightAtPos(int x, int y, int z)
    {
        //if (lightGenerationCount == 0)
        //{
        //    return 15;
        //}
        int index = 256 * y + 16 * x + z;
        return lights[index];
    }

    public int GetLightAtPos(Vector3Int pos)
    {
        return GetLightAtPos(pos.x, pos.y, pos.z);
    }

    int count;
    string log = "";
    void SetLightAtPos(Vector3Int pos, int light)
    {
        count++;
        //Debug.Log("SetLightAtPos,pos=" + pos + ",light=" + light);
        //log += "\nSetLightAtPos,pos=" + pos + ",light=" + light;
        int index = 256 * pos.y + 16 * pos.x + pos.z;
        lights[index] = (byte)light;
    }

    public void UpdateLighting()
    {
        ClearLights();
        log = "";
        count = 0;

        List<Vector3Int> skyLightList = new List<Vector3Int>();

        int maxOpaqueHeight = 0;

        //设置天光
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                for (int y = 255; y >= 0; y--)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (HasOpaqueBlock(pos))
                    {
                        if (pos.y > maxOpaqueHeight)
                        {
                            maxOpaqueHeight = pos.y;
                        }
                        break;
                    }
                    SetLightAtPos(pos, 15);
                    skyLightList.Add(pos);
                }
            }
        }

        //扩散
        foreach (Vector3Int pos in skyLightList)
        {
            if (pos.y > maxOpaqueHeight)
            {
                //性能优化，顶部的天光无法扩散，跳过计算
                continue;
            }
            if (pos.x > 0 && GetLightAtPos(pos + Utilities.vector3Int.left) < 14)
            {
                FloodFill(pos + Utilities.vector3Int.left, 14);
            }
            if (pos.x < 15 && GetLightAtPos(pos + Utilities.vector3Int.right) < 14)
            {
                FloodFill(pos + Utilities.vector3Int.right, 14);
            }
            if (pos.z < 15 && GetLightAtPos(pos + Utilities.vector3Int.forward) < 14)
            {
                FloodFill(pos + Utilities.vector3Int.forward, 14);
            }
            if (pos.z > 0 && GetLightAtPos(pos + Utilities.vector3Int.back) < 14)
            {
                FloodFill(pos + Utilities.vector3Int.back, 14);
            }
        }
        lightGenerationCount++;

        //FloodFill(new Vector3Int(0, 2, 0), 15);
        Debug.Log("count=" + count + ",maxOpaqueHeight="+ maxOpaqueHeight + ",log=\n" + log);
        //for (int y = 0; y < 256; y++)
        //{
        //    for (int x = 0; x < 16; x++)
        //        for (int z = 0; z < 16; z++)
        //            Debug.Log("x=" + x + ",y=" + y + ",z=" + z + ",light=" + GetLightAtPos(new Vector3Int(x, y, z)));
        //}
    }

    public void FloodFill(Vector3Int node, int targetLight)
    {
        if (node == new Vector3Int(13, 3, 12))
        {
            Debug.Log("light=" + targetLight);
        }
        int nodeLight = GetLightAtPos(node);

        //如果当前亮度比目标亮度大，则不更新
        if (nodeLight >= targetLight) { return; }

        if (HasOpaqueBlock(node)) return;

        //否则更新当前节点亮度
        SetLightAtPos(node, targetLight);

        if (node.x > 0)
            FloodFill(node + Utilities.vector3Int.left, targetLight - 1);
        if (node.x < 15)
            FloodFill(node + Utilities.vector3Int.right, targetLight - 1);
        if (node.z > 0)
            FloodFill(node + Utilities.vector3Int.back, targetLight - 1);
        if (node.z < 15)
            FloodFill(node + Utilities.vector3Int.forward, targetLight - 1);
        if (node.y > 0)
            FloodFill(node + Utilities.vector3Int.down, targetLight - 1);
        if (node.y < 255)
            FloodFill(node + Utilities.vector3Int.up, targetLight - 1);
    }
}
