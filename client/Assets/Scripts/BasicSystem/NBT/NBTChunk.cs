using UnityEngine;
using protocol.cs_theircraft;
using System.Collections.Generic;
using Substrate.Nbt;

public class NBTChunk
{
    public int x;
    public int z;
    public Transform transform;
    public GameObject gameObject;
    public bool isDirty = false;
    TagNodeList Sections;

    public bool hasBuiltMesh = false;
    public int lightGenerationCount = 0;
    public int meshBuildCount = 0;

    public List<Vector3Int> torchList = new List<Vector3Int>();

    public NBTGameObject nbtGO;

    public NBTChunk()
    {
        gameObject = new GameObject("chunk (" + x + "," + z + ")");
        transform = gameObject.transform;

        nbtGO = NBTGameObject.Create("Collidable", transform);
    }

    public void SetData(int _x, int _z, TagNodeList sections)
    {
        x = _x;
        z = _z;
        Sections = sections;
        gameObject.name = "chunk (" + x + "," + z + ")";
        transform.localPosition = new Vector3(x * 16, 0, z * 16);
        ClearData();
    }

    //input is local position
    public byte GetBlockByte(int xInChunk, int worldY, int zInChunk)
    {
        if (xInChunk < 0 || xInChunk > 15 || worldY < 0 || worldY > 255 || zInChunk < 0 || zInChunk > 15)
        {
            if (xInChunk < 0)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x - 1, z);
                return chunk.GetBlockByte(xInChunk + 16, worldY, zInChunk);
            }
            else if (xInChunk > 15)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x + 1, z);
                return chunk.GetBlockByte(xInChunk - 16, worldY, zInChunk);
            }
            else if (zInChunk < 0)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x, z - 1);
                return chunk.GetBlockByte(xInChunk, worldY, zInChunk + 16);
            }
            else if (zInChunk > 15)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x, z + 1);
                return chunk.GetBlockByte(xInChunk, worldY, zInChunk - 16);
            }
        }

        int sectionIndex = Mathf.FloorToInt(worldY / 16f);
        if (sectionIndex >= 0 && sectionIndex < Sections.Count)
        {
            TagNodeCompound section = Sections[sectionIndex] as TagNodeCompound;
            TagNodeByteArray blocks = section["Blocks"] as TagNodeByteArray;

            int yInSection = worldY - sectionIndex * 16;
            int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

            if (blockPos >= 0 && blockPos < 4096)
            {
                return blocks.Data[blockPos];
            }
        }
        return 0;
    }

    //input is local position
    public void GetBlockData(int xInChunk, int worldY, int zInChunk, ref byte blockType, ref byte blockData)
    {
        if (xInChunk < 0 || xInChunk > 15 || worldY < 0 || worldY > 255 || zInChunk < 0 || zInChunk > 15)
        {
            if (xInChunk < 0)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x - 1, z);
                chunk.GetBlockData(xInChunk + 16, worldY, zInChunk, ref blockType, ref blockData);
            }
            else if (xInChunk > 15)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x + 1, z);
                chunk.GetBlockData(xInChunk - 16, worldY, zInChunk, ref blockType, ref blockData);
            }
            else if (zInChunk < 0)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x, z - 1);
                chunk.GetBlockData(xInChunk, worldY, zInChunk + 16, ref blockType, ref blockData);
            }
            else if (zInChunk > 15)
            {
                NBTChunk chunk = NBTHelper.GetChunk(x, z + 1);
                chunk.GetBlockData(xInChunk, worldY, zInChunk - 16, ref blockType, ref blockData);
            }
            return;
        }

        int sectionIndex = Mathf.FloorToInt(worldY / 16f);
        if (sectionIndex >= 0 && sectionIndex < Sections.Count)
        {
            TagNodeCompound section = Sections[sectionIndex] as TagNodeCompound;
            TagNodeByteArray blocks = section["Blocks"] as TagNodeByteArray;
            TagNodeByteArray data = section["Data"] as TagNodeByteArray;

            int yInSection = worldY - sectionIndex * 16;
            int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

            if (blockPos >= 0 && blockPos < 4096)
            {
                blockType = blocks.Data[blockPos];
                blockData = NBTHelper.GetNibble(data.Data, blockPos);
                return;
            }
        }
        return;
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
    public bool HasOpaqueBlock(int xInChunk, int worldY, int zInChunk)
    {
        byte type = GetBlockByte(xInChunk, worldY, zInChunk);
        return !NBTGeneratorManager.IsTransparent(type);
    }

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
        nbtGO.Clear();

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
                            generator.GenerateMeshInChunk(this, blockData, pos, nbtGO);
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
            generator.AfterGenerateMesh(nbtGO);
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
        nbtGO.Refresh();
    }

    public void ClearData()
    {
        hasBuiltMesh = false;
        lightGenerationCount = 0;
        meshBuildCount = 0;
        torchList.Clear();
        nbtGO.GetComponent<MeshFilter>().sharedMesh = null;
    }
}
