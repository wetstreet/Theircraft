using UnityEngine;
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
    public TagNodeByteArray Biomes;
    public Dictionary<Vector3Int, TagNodeCompound> tileEntityDict = new Dictionary<Vector3Int, TagNodeCompound>();
    public Dictionary<Vector3Int, FurnaceData> furnaceDict = new Dictionary<Vector3Int, FurnaceData>();

    TagNodeList TileEntities;
    HashSet<Vector3Int> tileEntityList = new HashSet<Vector3Int>();
    Dictionary<Vector3Int, GameObject> tileEntityObjs = new Dictionary<Vector3Int, GameObject>();

    public NBTGameObject collidable;
    public NBTGameObject notCollidable;
    public NBTGameObject water;
    public GameObject special;

    public CubeAttributes ca = new CubeAttributes();

    public bool isBorderChunk = false;

    bool isBuilding = false;

    public NBTChunk()
    {
        gameObject = new GameObject("chunk (" + x + "," + z + ")");
        transform = gameObject.transform;

        collidable = NBTCollidable.Create(this);
        notCollidable = NBTNotCollidable.Create(this);
        water = NBTWater.Create(this);
        special = new GameObject("special");
        special.transform.parent = transform;
        special.transform.localPosition = Vector3.zero;
    }

    public void SetData(int _x, int _z, TagNodeCompound Level)
    {
        x = _x;
        z = _z;
        globalX = x * 16;
        globalZ = z * 16;
        Sections = Level["Sections"] as TagNodeList;
        TileEntities = Level["TileEntities"] as TagNodeList;
        Biomes = Level["Biomes"] as TagNodeByteArray;

        foreach (TagNodeCompound node in TileEntities)
        {
            Vector3Int pos = new Vector3Int((TagNodeInt)node["x"], (TagNodeInt)node["y"], (TagNodeInt)node["z"]);
            tileEntityDict.Add(pos, node);
            if (node["id"].ToTagString() == "minecraft:furnace")
            {
                FurnaceData fd = new FurnaceData(node);
                furnaceDict.Add(pos, fd);
            }
        }

        gameObject.name = "chunk (" + x + "," + z + ")";
        transform.localPosition = new Vector3(x * 16, 0, z * 16);
    }


    public void Tick()
    {
        foreach (var furnace in furnaceDict.Values)
        {
            furnace.Tick();
        }
    }

    public void AddTileEntity(Vector3Int pos, TagNodeCompound node)
    {
        if (TileEntities.ValueType != TagType.TAG_COMPOUND)
        {
            TileEntities.ChangeValueType(TagType.TAG_COMPOUND);
        }
        TileEntities.Add(node);
        tileEntityDict.Add(pos, node);

        if (node["id"].ToTagString() == "minecraft:furnace")
        {
            FurnaceData fd = new FurnaceData(node);
            furnaceDict.Add(pos, fd);
        }
    }

    public void RemoveTileEntity(Vector3Int pos)
    {
        TagNodeCompound node = tileEntityDict[pos];
        TileEntities.Remove(node);
        tileEntityDict.Remove(pos);

        if (furnaceDict.ContainsKey(pos))
        {
            furnaceDict.Remove(pos);
        }
    }

    public byte GetBlockByte(Vector3Int pos)
    {
        return GetBlockByte(pos.x, pos.y, pos.z);
    }

    //input is local position
    public byte GetBlockByte(int xInChunk, int worldY, int zInChunk)
    {
        if (xInChunk < 0 || xInChunk > 15 || zInChunk < 0 || zInChunk > 15)
        {
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
    public void GetBlockData(int xInChunk, int worldY, int zInChunk, out byte blockType, out byte blockData)
    {
        blockType = 0;
        blockData = 0;
        if (worldY < 0 || worldY > 255)
        {
            return;
        }

        if (xInChunk < 0 || xInChunk > 15 || zInChunk < 0 || zInChunk > 15)
        {
            NBTHelper.GetBlockData(xInChunk + 16 * x, worldY, zInChunk + 16 * z, out blockType, out blockData);
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

    //input is local position
    public bool HasOpaqueBlock(int xInChunk, int worldY, int zInChunk)
    {
        byte type = GetBlockByte(xInChunk, worldY, zInChunk);
        return !NBTGeneratorManager.IsTransparent(type);
    }

    public void AddTileEntityObj(Vector3Int pos, NBTBlock generator, byte data)
    {
        if (!tileEntityObjs.ContainsKey(pos))
        {
            GameObject obj = generator.GetTileEntityGameObject(this, data, pos);
            tileEntityObjs[pos] = obj;
        }
    }

    public void RemoveTileEntityObj(Vector3Int pos)
    {
        pos.x -= x * 16;
        pos.z -= z * 16;
        if (tileEntityObjs.ContainsKey(pos))
        {
            GameObject obj = tileEntityObjs[pos];
            tileEntityObjs.Remove(pos);
            tileEntityList.Remove(pos);
            Object.Destroy(obj);
        }
    }

    public GameObject GetTileEntityObj(Vector3Int pos)
    {
        if (tileEntityObjs.ContainsKey(pos))
        {
            return tileEntityObjs[pos];
        }
        return null;
    }

    void AddCube(NBTBlock generator, byte blockData, UpdateFlags updateFlag)
    {
        if (water.nbtMesh == null || collidable == null || notCollidable == null)
        {
            Debug.LogError("no mesh");
            return;
        }
        UnityEngine.Profiling.Profiler.BeginSample("AddCube");
        if (generator is NBTStationaryWater)
        {
            if (updateFlag.HasFlag(UpdateFlags.Water))
            {
                generator.AddCube(this, blockData, pos, water);
            }
        }
        else if (generator is NBTPlant || generator is NBTSnowLayer)
        {
            if (updateFlag.HasFlag(UpdateFlags.NotCollidable))
            {
                generator.AddCube(this, blockData, pos, notCollidable);
            }
        }
        else
        {
            if (updateFlag.HasFlag(UpdateFlags.Collidable))
            {
                generator.AddCube(this, blockData, pos, collidable);
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    Vector3Int pos = new Vector3Int();
    public void RefreshMeshData(UpdateFlags updateFlag = UpdateFlags.All)
    {
        //Debug.Log("RefreshMeshData,chunk=" + x + ",z=" + z);
        UnityEngine.Profiling.Profiler.BeginSample("RefreshMeshData");

        if (updateFlag.HasFlag(UpdateFlags.Collidable))
        {
            collidable.Clear();
        }
        if (updateFlag.HasFlag(UpdateFlags.NotCollidable))
        {
            notCollidable.Clear();
        }
        if (updateFlag.HasFlag(UpdateFlags.Water))
        {
            water.Clear();
        }

        collidable.nbtMesh.subMeshCount = Sections.Count;
        notCollidable.nbtMesh.subMeshCount = Sections.Count;
        water.nbtMesh.subMeshCount = Sections.Count;

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
                        int worldY = yInSection + sectionIndex * 16;

                        if (generator != null)
                        {
                            pos.Set(xInSection, worldY, zInSection);
                            byte blockData = NBTHelper.GetNibble(Data.Data, blockPos);

                            try
                            {
                                if (generator.isTileEntity)
                                {
                                    tileEntityList.Add(pos);
                                }
                                else
                                {
                                    AddCube(generator, blockData, updateFlag);
                                }
                            }
                            catch (System.Exception e)
                            {
                                int worldX = x * 16 + xInSection;
                                int worldZ = z * 16 + zInSection;
                                Vector3Int worldPos = new Vector3Int(worldX, worldY, worldZ);
                                Debug.LogError(generator.GetType() + ",pos=" + worldPos + "\n" + e.ToString());
                            }
                        }
                        else if (rawType != 0 && rawType != 11)
                        {
                            int worldX = x * 16 + xInSection;
                            int worldZ = z * 16 + zInSection;
                            Vector3Int worldPos = new Vector3Int(worldX, worldY, worldZ);
                            Debug.LogWarning("generator not exist" + ",pos=" + worldPos +", type=" + rawType);
                        }
                    }
                }
            }
            collidable.nbtMesh.triangleIndexes[sectionIndex] = collidable.nbtMesh.triangleCount;
            notCollidable.nbtMesh.triangleIndexes[sectionIndex] = notCollidable.nbtMesh.triangleCount;
            water.nbtMesh.triangleIndexes[sectionIndex] = water.nbtMesh.triangleCount;
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    void CheckBorder()
    {
        NBTChunk leftChunk = NBTHelper.GetChunk(x - 1, z);
        NBTChunk rightChunk = NBTHelper.GetChunk(x + 1, z);
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
                RebuildMeshAsync(false);
            }
            isBorderChunk = false;
        }
    }

    void CheckNearbyChunks()
    {
        NBTChunk leftChunk = NBTHelper.GetChunk(x - 1, z);
        NBTChunk rightChunk = NBTHelper.GetChunk(x + 1, z);
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

    bool hasInitTileEntity = false;
    void InitTileEntity()
    {
        if (hasInitTileEntity)
            return;

        foreach (Vector3Int pos in tileEntityList)
        {
            if (!tileEntityObjs.ContainsKey(pos))
            {
                int sectionIndex = pos.y / 16;
                TagNodeCompound Section = Sections[sectionIndex] as TagNodeCompound;
                TagNodeByteArray Blocks = Section["Blocks"] as TagNodeByteArray;
                TagNodeByteArray Data = Section["Data"] as TagNodeByteArray;

                int yInSection = pos.y % 16;
                int blockPos = yInSection * 16 * 16 + pos.z * 16 + pos.x;
                byte rawType = Blocks.Data[blockPos];
                NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(rawType);
                byte blockData = NBTHelper.GetNibble(Data.Data, blockPos);
                GameObject obj = generator.GetTileEntityGameObject(this, blockData, pos);

                tileEntityObjs[pos] = obj;
            }
        }

        hasInitTileEntity = true;
    }

    public void RebuildMesh(UpdateFlags updateFlags = UpdateFlags.All, bool checkBorder = true)
    {
        if (isBuilding)
            return;
        isBuilding = true;

        UnityEngine.Profiling.Profiler.BeginSample("RebuildMesh");

        if (checkBorder)
        {
            CheckBorder();
            CheckNearbyChunks();
        }

        RefreshMeshData(updateFlags);
        InitTileEntity();
        if (updateFlags.HasFlag(UpdateFlags.Collidable))
        {
            collidable.Refresh();
        }
        if (updateFlags.HasFlag(UpdateFlags.NotCollidable))
        {
            notCollidable.Refresh();
        }
        if (updateFlags.HasFlag(UpdateFlags.Water))
        {
            water.Refresh();
        }

        UnityEngine.Profiling.Profiler.EndSample();

        isBuilding = false;
    }

    public async void RebuildMeshAsync(bool checkBorder = true)
    {
        if (isBuilding)
            return;
        isBuilding = true;

        if (checkBorder)
        {
            CheckBorder();
            CheckNearbyChunks();
        }

        await Task.Run(()=> {
            RefreshMeshData(UpdateFlags.All);
        });
        InitTileEntity();
        collidable.Refresh();
        notCollidable.Refresh();
        water.Refresh();

        isBuilding = false;
    }

    public void ClearMesh()
    {
        collidable.GetComponent<MeshFilter>().sharedMesh = null;
        notCollidable.GetComponent<MeshFilter>().sharedMesh = null;
        water.GetComponent<MeshFilter>().sharedMesh = null;
    }

    public void Serialize()
    {
        foreach (var pair in furnaceDict)
        {
            TagNodeCompound node = tileEntityDict[pair.Key];
            pair.Value.Serialize(node);
        }
    }

    public void ClearData()
    {
        ClearMesh();

        hasInitTileEntity = false;
        foreach (var obj in tileEntityObjs)
        {
            Object.Destroy(obj.Value);
        }
        tileEntityObjs.Clear();
        tileEntityDict.Clear();
        tileEntityList.Clear();
        furnaceDict.Clear();
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

    public byte GetLightByte(int xInChunk, int yInChunk, int zInChunk, bool extends = false)
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
                    return chunk.GetLightByte(xInChunk - xOffset * 16, yInChunk, zInChunk - zOffset * 16);
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
        TagNodeByteArray BlockLight = Section["BlockLight"] as TagNodeByteArray;
        byte blockLight = NBTHelper.GetNibble(BlockLight.Data, blockPos);

        //Debug.Log("y=" + x + "," + z + ",section=" + sectionIndex);

        return skyLight > blockLight ? skyLight : blockLight;
    }

    public void GetLights(int xInChunk, int yInChunk, int zInChunk, out float skyLight, out float blockLight)
    {
        GetLightsByte(xInChunk, yInChunk, zInChunk, out byte skyLightByte, out byte blockLightByte, true);
        skyLight = skyLightByte / 15f;
        blockLight = blockLightByte / 15f;
    }

    public void GetLightsByte(int xInChunk, int yInChunk, int zInChunk, out byte skyLight, out byte blockLight, bool extends = false)
    {
        skyLight = 15;
        blockLight = 0;
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
                    chunk.GetLightsByte(xInChunk - xOffset * 16, yInChunk, zInChunk - zOffset * 16, out skyLight, out blockLight);
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
        skyLight = NBTHelper.GetNibble(SkyLight.Data, blockPos);
        TagNodeByteArray BlockLight = Section["BlockLight"] as TagNodeByteArray;
        blockLight = NBTHelper.GetNibble(BlockLight.Data, blockPos);
    }
}
