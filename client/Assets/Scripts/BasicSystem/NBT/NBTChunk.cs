using UnityEngine;
using protocol.cs_theircraft;
using System.Collections.Generic;
using Substrate.Nbt;

public class NBTChunk
{
    public int x;
    public int z;
    public int globalX;
    public int globalZ;
    public Vector2Int pos;      //used for index in dictionary
    public Transform transform;
    public GameObject gameObject;
    public bool isDirty = false;
    TagNodeList Sections;

    static int capacity = 8192;

    public GameObject collidableGO;
    public GameObject nonCollidableGO;

    static int capacity1 = 8192;
    public List<Vector3> vertices1 = new List<Vector3>(capacity1);
    public List<Color> colors1 = new List<Color>(capacity1);
    public List<Vector2> uv1 = new List<Vector2>(capacity1);
    List<List<int>> trianglesList = new List<List<int>>();

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

    List<Material> materialList = new List<Material>();

    public NBTChunk()
    {
        gameObject = new GameObject("chunk (" + x + "," + z + ")");
        transform = gameObject.transform;

        collidableMesh = new Mesh();
        collidableMesh.name = "CollidableMesh";
        
        collidableGO = new GameObject("Collidable");
        collidableGO.transform.parent = transform;
        collidableGO.AddComponent<MeshFilter>().sharedMesh = collidableMesh;
        collidableGO.AddComponent<MeshCollider>().sharedMesh = collidableMesh;
        collidableGO.AddComponent<MeshRenderer>();
        collidableGO.AddComponent<NavMeshSourceTag>();
        collidableGO.layer = LayerMask.NameToLayer("Chunk");

        //nonCollidableMesh = new Mesh();
        //nonCollidableMesh.name = "NonCollidableMesh";

        //nonCollidableGO = new GameObject("NonCollidable");
        //nonCollidableGO.transform.parent = transform;
        //nonCollidableGO.AddComponent<MeshFilter>().sharedMesh = nonCollidableMesh;
        //nonCollidableGO.AddComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Materials/block");
        //nonCollidableGO.AddComponent<MeshCollider>().sharedMesh = nonCollidableMesh;
        //nonCollidableGO.layer = LayerMask.NameToLayer("Plant");
    }

    public void SetData(int _x, int _z, TagNodeList sections)
    {
        x = _x;
        z = _z;
        pos.x = x;
        pos.y = z;
        globalX = x * 16;
        globalZ = z * 16;
        Sections = sections;
        gameObject.name = "chunk (" + x + "," + z + ")";
        transform.localPosition = new Vector3(x * 16, 0, z * 16);
        ClearData();
    }

    //public static int GetChunkPosByGlobalPos(int _x)
    //{
    //    return Mathf.FloorToInt(_x / 16f);
    //}

    //public int GetXInChunkByGlobalX(int _x)
    //{
    //    return _x - x * 16;
    //}

    //public int GetZInChunkByGlobalZ(int _z)
    //{
    //    return _z - z * 16;
    //}

    ////input is local position
    public byte GetBlockByte(int xInChunk, int worldY, int zInChunk)
    {
        int sectionIndex = Mathf.FloorToInt(worldY / 16f);
        if (sectionIndex <= Sections.Count)
        {
            TagNodeCompound section = Sections[sectionIndex] as TagNodeCompound;
            TagNodeByteArray blocks = section["Blocks"] as TagNodeByteArray;

            int yInSection = worldY - sectionIndex * 16;
            int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

            return blocks.Data[blockPos];
        }
        return 0;
    }

    TagNodeCompound GetSection(int yInChunk)
    {
        int sectionIndex = Mathf.FloorToInt(yInChunk / 16f);
        if (sectionIndex <= Sections.Count)
        {
            return Sections[sectionIndex] as TagNodeCompound;
        }
        return null;
    }

    int GetBlockPos(int xInSection, int yInSection, int zInSection)
    {
        return yInSection * 16 * 16 + zInSection * 16 + xInSection;
    }

    ////input is local position
    //public CSBlockType GetBlockType(int xInChunk, int _y, int zInChunk)
    //{
    //    return (CSBlockType)GetBlockByte(xInChunk, _y, zInChunk);
    //}

    ////input is global position
    //public void SetBlockTypeByGlobalPosition(int _x, int _y, int _z, CSBlockType type)
    //{
    //    int xInChunk = GetXInChunkByGlobalX(_x);
    //    int zInChunk = GetZInChunkByGlobalZ(_z);
    //    //blocksInByte[256 * _y + 16 * xInChunk + zInChunk] = (byte)type;
    //    SetBlockType(xInChunk, _y, zInChunk, type);
    //}

    //void SetNearbyChunkDirty(int xInChunk, int zInChunk)
    //{
    //    if (xInChunk == 0)
    //    {
    //        ChunkManager.SetChunkDirty(x - 1, z);
    //    }
    //    else if (xInChunk == 15)
    //    {
    //        ChunkManager.SetChunkDirty(x + 1, z);
    //    }
    //    if (zInChunk == 0)
    //    {
    //        ChunkManager.SetChunkDirty(x, z - 1);
    //    }
    //    else if (zInChunk == 15)
    //    {
    //        ChunkManager.SetChunkDirty(x, z + 1);
    //    }
    //}

    ////input is local position
    //public void SetBlockType(int xInChunk, int _y, int zInChunk, CSBlockType type)
    //{
    //    blocksInByte[256 * _y + 16 * xInChunk + zInChunk] = (byte)type;
    //    isDirty = true;
    //    SetNearbyChunkDirty(xInChunk, zInChunk);
    //}

    //public bool HasNotCollidableBlock(int x, int y, int z)
    //{
    //    if (x < 0 || x > 15 || z < 0 || z > 15 || y < 0 || y > 255)
    //    {
    //        return false;
    //    }
    //    byte type = GetBlockByte(x, y, z);
    //    return type > 0 && !ChunkMeshGenerator.type2texcoords[type].isCollidable;
    //}

    ////input is local position
    public bool HasOpaqueBlock(int xInChunk, int worldY, int zInChunk)
    {
        if (xInChunk < 0 || xInChunk > 15 || worldY < 0 || worldY > 255 || zInChunk < 0 || zInChunk > 15) return false;

        byte type = 0;
        int sectionIndex = Mathf.FloorToInt(worldY / 16f);
        if (sectionIndex >= 0 && sectionIndex < Sections.Count)
        {
            TagNodeCompound section = Sections[sectionIndex] as TagNodeCompound;
            TagNodeByteArray blocks = section["Blocks"] as TagNodeByteArray;

            int yInSection = worldY - sectionIndex * 16;
            int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

            if (blockPos >= 0 && blockPos < 4096)
            {
                type = blocks.Data[blockPos];
            }
        }
        return !NBTGeneratorManager.IsTransparent(type);
    }

    //public bool HasOpaqueBlock(Vector3Int pos)
    //{
    //    return HasOpaqueBlock(pos.x, pos.y, pos.z);
    //}

    Dictionary<byte, CSBlockType> typeDict = new Dictionary<byte, CSBlockType>
    {
        { 0, CSBlockType.None },
        { 1, CSBlockType.Stone },
        { 2, CSBlockType.GrassBlock },
        { 3, CSBlockType.Dirt },
        { 4, CSBlockType.Cobblestone },
        { 5, CSBlockType.OakPlanks },
        { 6, CSBlockType.OakSapling },
        { 7, CSBlockType.BedRock },
        { 12, CSBlockType.Sand },
        { 13, CSBlockType.Gravel },
        { 14, CSBlockType.GoldOre },
        { 15, CSBlockType.IronOre },
        { 16, CSBlockType.CoalOre },
        { 17, CSBlockType.OakLog },
        { 18, CSBlockType.OakLeaves },
        { 20, CSBlockType.Glass },
        { 32, CSBlockType.DeadBush },
        { 45, CSBlockType.Brick },
        { 81, CSBlockType.Cactus },
    };


    public void RefreshMeshData()
    {
        //Debug.Log("RefreshMeshData,chunk=" + chunk.pos);
        vertices1.Clear();
        colors1.Clear();
        uv1.Clear();
        trianglesList.Clear();

        vertices2.Clear();
        uv2.Clear();
        triangles2.Clear();

        Vector3Int pos = new Vector3Int();

        List<NBTMeshGenerator> generators = new List<NBTMeshGenerator>();
        NBTGeneratorManager.ClearGeneratorData();

        for (int sectionIndex = 0; sectionIndex < Sections.Count; sectionIndex++)
        {
            TagNodeCompound Section = Sections[sectionIndex] as TagNodeCompound;
            TagNodeByteArray Blocks = Section["Blocks"] as TagNodeByteArray;
            TagNodeByteArray Data = Section["Data"] as TagNodeByteArray;

            for (int yInSection = 0; yInSection < 16; yInSection++)
            {
                for (int zInSection = 0; zInSection < 16; zInSection++)
                {
                    for (int xInSection = 0; xInSection < 16; xInSection++)
                    {
                        int blockPos = yInSection * 16 * 16 + zInSection * 16 + xInSection;
                        byte rawType = Blocks.Data[blockPos];
                        NBTMeshGenerator generator = NBTGeneratorManager.GetMeshGenerator(rawType);
                        if (generator != null)
                        {
                            int worldY = yInSection + sectionIndex * 16;
                            pos.Set(xInSection, worldY, zInSection);
                            byte blockData = NBTHelper.GetNibble(Data.Data, blockPos);
                            generator.GenerateMeshInChunk(this, blockData, pos, vertices1, uv1);
                            if (!generators.Contains(generator))
                            {
                                generators.Add(generator);
                            }
                        }
                    }
                }
            }
        }

        foreach (NBTMeshGenerator generator in generators)
        {
            generator.AfterGenerateMesh(trianglesList, materialList);
        }

        hasBuiltMesh = true;
        isDirty = false;
    }

    public void RebuildMesh(bool forceRefreshMeshData = true)
    {
        if (forceRefreshMeshData || isDirty)
        {
            RefreshMeshData();
        }

        collidableMesh.Clear();
        collidableMesh.SetVertices(vertices1);
        collidableMesh.SetUVs(0, uv1);
        collidableMesh.subMeshCount = trianglesList.Count;
        for (int i = 0; i < trianglesList.Count; i++)
        {
            collidableMesh.SetTriangles(trianglesList[i], i);
        }
        collidableMesh.RecalculateNormals();
        collidableGO.GetComponent<MeshRenderer>().sharedMaterials = materialList.ToArray();
        collidableGO.GetComponent<MeshFilter>().sharedMesh = collidableMesh;
        collidableGO.GetComponent<MeshCollider>().sharedMesh = collidableMesh;

        //nonCollidableMesh.Clear();
        //nonCollidableMesh.SetVertices(vertices2);
        //nonCollidableMesh.SetUVs(0, uv2);
        //nonCollidableMesh.SetTriangles(triangles2, 0);
        //nonCollidableGO.GetComponent<MeshFilter>().sharedMesh = nonCollidableMesh;
        //nonCollidableGO.GetComponent<MeshCollider>().sharedMesh = nonCollidableMesh;
    }

    public void ClearData()
    {
        hasBuiltMesh = false;
        lightGenerationCount = 0;
        meshBuildCount = 0;
        torchList.Clear();
        collidableGO.GetComponent<MeshFilter>().sharedMesh = null;
        //nonCollidableGO.GetComponent<MeshFilter>().sharedMesh = null;
    }
}
