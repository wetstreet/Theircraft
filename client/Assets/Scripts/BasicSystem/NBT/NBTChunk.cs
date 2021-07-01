using UnityEngine;
using protocol.cs_theircraft;
using System.Collections.Generic;
using Substrate.Nbt;
using Unity.Jobs;
using Unity.Collections;
using System.Threading.Tasks;

public class NBTChunk
{
    public int x;
    public int z;
    public int globalX;
    public int globalZ;
    public Transform transform;
    public GameObject gameObject;
    public bool isDirty = false;
    public TagNodeList Sections;

    public bool hasBuiltMesh = false;
    public int lightGenerationCount = 0;
    public int meshBuildCount = 0;

    public List<Vector3Int> torchList = new List<Vector3Int>();

    public NBTGameObject collidable;
    public NBTGameObject notCollidable;
    public NBTGameObject water;

    public bool isBorderChunk = false;

    public NBTChunk()
    {
        gameObject = new GameObject("chunk (" + x + "," + z + ")");
        transform = gameObject.transform;

        collidable = NBTGameObject.Create("Collidable", this, LayerMask.NameToLayer("Chunk"));
        notCollidable = NBTGameObject.Create("NotCollidable", this, LayerMask.NameToLayer("Plant"));
        //water = NBTGameObject.Create("Water", transform, LayerMask.NameToLayer("Water"), false);
    }

    public void SetData(int _x, int _z, TagNodeList sections)
    {
        x = _x;
        z = _z;
        globalX = x * 16;
        globalZ = z * 16;
        Sections = sections;
        gameObject.name = "chunk (" + x + "," + z + ")";
        transform.localPosition = new Vector3(x * 16, 0, z * 16);
        ClearData();
    }

    //input is local position
    public byte GetBlockByte(int xInChunk, int worldY, int zInChunk)
    {
        if (xInChunk < 0 || xInChunk > 15 || zInChunk < 0 || zInChunk > 15)
        {
            //if (xInChunk < 0)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x - 1, z);
            //    return chunk.GetBlockByte(xInChunk + 16, worldY, zInChunk);
            //}
            //else if (xInChunk > 15)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x + 1, z);
            //    return chunk.GetBlockByte(xInChunk - 16, worldY, zInChunk);
            //}
            //else if (zInChunk < 0)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x, z - 1);
            //    return chunk.GetBlockByte(xInChunk, worldY, zInChunk + 16);
            //}
            //else if (zInChunk > 15)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x, z + 1);
            //    return chunk.GetBlockByte(xInChunk, worldY, zInChunk - 16);
            //}

            return NBTHelper.GetBlockByte(xInChunk + 16 * x, worldY, zInChunk + 16 * z);
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
    public void SetBlockByte(int xInChunk, int worldY, int zInChunk, byte type)
    {
        int sectionIndex = Mathf.FloorToInt(worldY / 16f);
        if (sectionIndex >= 0 && sectionIndex < Sections.Count)
        {
            TagNodeCompound section = Sections[sectionIndex] as TagNodeCompound;
            TagNodeByteArray blocks = section["Blocks"] as TagNodeByteArray;

            int yInSection = worldY - sectionIndex * 16;
            int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

            if (blockPos >= 0 && blockPos < 4096)
            {
                blocks.Data[blockPos] = type;
            }
        }
    }

    //input is local position
    public void SetBlockData(int xInChunk, int worldY, int zInChunk, byte type, byte data)
    {
        int sectionIndex = Mathf.FloorToInt(worldY / 16f);
        if (sectionIndex >= 0 && sectionIndex < Sections.Count)
        {
            TagNodeCompound section = Sections[sectionIndex] as TagNodeCompound;
            TagNodeByteArray Blocks = section["Blocks"] as TagNodeByteArray;
            TagNodeByteArray Data = section["Data"] as TagNodeByteArray;

            int yInSection = worldY - sectionIndex * 16;
            int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

            if (blockPos >= 0 && blockPos < 4096)
            {
                Blocks.Data[blockPos] = type;
                NBTHelper.SetNibble(Data.Data, blockPos, data);
            }
        }
    }

    //input is local position
    public void GetBlockData(int xInChunk, int worldY, int zInChunk, ref byte blockType, ref byte blockData)
    {
        if (xInChunk < 0 || xInChunk > 15 || worldY < 0 || worldY > 255 || zInChunk < 0 || zInChunk > 15)
        {
            //if (xInChunk < 0)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x - 1, z);
            //    chunk.GetBlockData(xInChunk + 16, worldY, zInChunk, ref blockType, ref blockData);
            //}
            //else if (xInChunk > 15)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x + 1, z);
            //    chunk.GetBlockData(xInChunk - 16, worldY, zInChunk, ref blockType, ref blockData);
            //}
            //else if (zInChunk < 0)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x, z - 1);
            //    chunk.GetBlockData(xInChunk, worldY, zInChunk + 16, ref blockType, ref blockData);
            //}
            //else if (zInChunk > 15)
            //{
            //    NBTChunk chunk = NBTHelper.GetChunk(x, z + 1);
            //    chunk.GetBlockData(xInChunk, worldY, zInChunk - 16, ref blockType, ref blockData);
            //}

            //NBTHelper.GetBlockData(xInChunk + 16 * x, worldY, zInChunk + 16 * z, ref blockType, ref blockData);
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

    //input is local position
    public bool HasOpaqueBlock(int xInChunk, int worldY, int zInChunk)
    {
        byte type = GetBlockByte(xInChunk, worldY, zInChunk);
        return !NBTGeneratorManager.IsTransparent(type);
    }

    Vector3Int pos = new Vector3Int();
    public void RefreshMeshData()
    {
        //Debug.Log("RefreshMeshData,chunk=" + x + ",z=" + z);
        UnityEngine.Profiling.Profiler.BeginSample("RefreshMeshData");

        collidable.Clear();
        notCollidable.Clear();
        //water.Clear();

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
                        NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(rawType);

                        if (generator != null)
                        {
                            int worldY = yInSection + sectionIndex * 16;
                            pos.Set(xInSection, worldY, zInSection);
                            byte blockData = NBTHelper.GetNibble(Data.Data, blockPos);

                            try
                            {
                                if (generator is NBTStationaryWater)
                                {
                                    //generator.GenerateMeshInChunk(this, blockData, pos, water);
                                }
                                else if (generator is NBTPlant)
                                {
                                    generator.AddCube(this, blockData, pos, notCollidable);
                                }
                                else
                                {
                                    generator.AddCube(this, blockData, pos, collidable);
                                }
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError(generator.GetType() + "\n" + e.ToString());
                            }
                        }
                        else if (rawType != 0 && rawType != 11)
                        {
                            Debug.LogWarning("generator not exist, type=" + rawType);
                        }
                    }
                }
            }
        }

        hasBuiltMesh = true;
        isDirty = false;

        UnityEngine.Profiling.Profiler.EndSample();
    }

    void CheckBorder()
    {
        NBTChunk leftChunk = NBTHelper.GetChunk(x - 1, z);
        NBTChunk rightChunk = NBTHelper.GetChunk(x - 1, z);
        NBTChunk frontChunk = NBTHelper.GetChunk(x, z + 1);
        NBTChunk backChunk = NBTHelper.GetChunk(x, z - 1);

        if (leftChunk == null || rightChunk == null || frontChunk == null || backChunk == null)
        {
            isBorderChunk = true;
        }
        else
        {
            if (isBorderChunk)
            {
                Debug.Log("border chunk=" + x + "," + z);
                RebuildMeshAsync(true, false);
            }
            isBorderChunk = false;
        }
    }

    public void RebuildMesh(bool forceRefreshMeshData = true, bool checkBorder = true)
    {
        UnityEngine.Profiling.Profiler.BeginSample("RebuildMesh");

        if (checkBorder)
        {
            CheckBorder();
            NBTChunk leftChunk = NBTHelper.GetChunk(x - 1, z);
            NBTChunk rightChunk = NBTHelper.GetChunk(x - 1, z);
            NBTChunk frontChunk = NBTHelper.GetChunk(x, z + 1);
            NBTChunk backChunk = NBTHelper.GetChunk(x, z - 1);

            if (leftChunk != null)
            {
                leftChunk.CheckBorder();
            }
            if (rightChunk != null)
            {
                rightChunk.CheckBorder();
            }
            if (frontChunk != null)
            {
                frontChunk.CheckBorder();
            }
            if (backChunk != null)
            {
                backChunk.CheckBorder();
            }
        }

        if (forceRefreshMeshData || isDirty)
        {
            RefreshMeshData();
        }

        collidable.Refresh();
        notCollidable.Refresh();
        //water.Refresh();

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public async void RebuildMeshAsync(bool forceRefreshMeshData = true, bool checkBorder = true)
    {
        if (checkBorder)
        {
            CheckBorder();
            NBTChunk leftChunk = NBTHelper.GetChunk(x - 1, z);
            NBTChunk rightChunk = NBTHelper.GetChunk(x - 1, z);
            NBTChunk frontChunk = NBTHelper.GetChunk(x, z + 1);
            NBTChunk backChunk = NBTHelper.GetChunk(x, z - 1);

            if (leftChunk != null)
            {
                leftChunk.CheckBorder();
            }
            if (rightChunk != null)
            {
                rightChunk.CheckBorder();
            }
            if (frontChunk != null)
            {
                frontChunk.CheckBorder();
            }
            if (backChunk != null)
            {
                backChunk.CheckBorder();
            }
        }

        if (forceRefreshMeshData || isDirty)
        {
            await Task.Run(RefreshMeshData);
        }
        collidable.Refresh();
        notCollidable.Refresh();
    }

    public void ClearData()
    {
        hasBuiltMesh = false;
        lightGenerationCount = 0;
        meshBuildCount = 0;
        torchList.Clear();

        collidable.GetComponent<MeshFilter>().sharedMesh = null;
        notCollidable.GetComponent<MeshFilter>().sharedMesh = null;
        //water.GetComponent<MeshFilter>().sharedMesh = null;
    }

    public byte GetSkyLightByte(int xInChunk, int yInChunk, int zInChunk, bool extends = false)
    {
        if (xInChunk < 0 || xInChunk > 15 || zInChunk < 0 || zInChunk > 15)
        {
            if (extends)
            {
                int xOffset = 0;
                int zOffset = 0;

                if (xInChunk < 0)
                    xOffset = -1;
                else if (xInChunk > 15)
                    xOffset = 1;

                if (zInChunk < 0)
                    zOffset = -1;
                else if (zInChunk > 15)
                    zOffset = 1;

                NBTChunk chunk = NBTHelper.GetChunk(x + xOffset, z + zOffset);
                if (chunk != null)
                    return chunk.GetSkyLightByte(xInChunk - xOffset * 16, yInChunk, zInChunk - zOffset * 16);
                else
                    return 15;
            }
            else
            {
                return 15;
            }
        }


        int sectionIndex = yInChunk / 16;
        if (sectionIndex >= Sections.Count || yInChunk < 0 || yInChunk > 255)
        {
            return 15;
        }

        int yInSection = yInChunk % 16;
        int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

        TagNodeCompound Section = Sections[sectionIndex] as TagNodeCompound;
        TagNodeByteArray SkyLight = Section["SkyLight"] as TagNodeByteArray;
        byte skyLight = NBTHelper.GetNibble(SkyLight.Data, blockPos);

        //Debug.Log("y=" + x + "," + z + ",section=" + sectionIndex);

        return skyLight;
    }

    public void SetSkyLightByte(int xInChunk, int yInChunk, int zInChunk, byte skyLight, bool extends = false)
    {
        if (xInChunk < 0 || xInChunk > 15 || zInChunk < 0 || zInChunk > 15)
        {
            if (extends)
            {
                int xOffset = 0;
                int zOffset = 0;

                if (xInChunk < 0)
                    xOffset = -1;
                else if (xInChunk > 15)
                    xOffset = 1;

                if (zInChunk < 0)
                    zOffset = -1;
                else if (zInChunk > 15)
                    zOffset = 1;

                NBTChunk chunk = NBTHelper.GetChunk(x + xOffset, z + zOffset);
                if (chunk != null)
                {
                    chunk.SetSkyLightByte(xInChunk - xOffset * 16, yInChunk, zInChunk - zOffset * 16, skyLight);
                }
            }
            return;
        }

        int sectionIndex = yInChunk / 16;
        if (sectionIndex >= Sections.Count || yInChunk < 0 || yInChunk > 255)
        {
            return;
        }

        int yInSection = yInChunk % 16;
        int blockPos = yInSection * 16 * 16 + zInChunk * 16 + xInChunk;

        TagNodeCompound Section = Sections[sectionIndex] as TagNodeCompound;
        TagNodeByteArray SkyLight = Section["SkyLight"] as TagNodeByteArray;
        NBTHelper.SetNibble(SkyLight.Data, blockPos, skyLight);
    }

    public float GetSkyLight(int xInChunk, int yInChunk, int zInChunk)
    {
        return GetSkyLightByte(xInChunk, yInChunk, zInChunk, true) / 15f;
    }
}
