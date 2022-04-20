using Substrate.Core;
using Substrate.Nbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Flags] public enum UpdateFlags
{
    None = 0,
    Collidable = 1,
    NotCollidable = 2,
    Water = 4,
    Lighting = 8,
    All = 15,
}

public class NBTHelper
{
    public static string save = "New World1";

    public static Dictionary<Vector2Int, RegionFile> regionDict = new Dictionary<Vector2Int, RegionFile>();
    private static Dictionary<Vector2Int, NBTChunk> chunkDict = new Dictionary<Vector2Int, NBTChunk>();
    private static Dictionary<Vector2Int, NbtTree> chunkDictNBT = new Dictionary<Vector2Int, NbtTree>();

    public static bool IsAlive = false;

    static string savePath
    {
        get
        {
#if UNITY_EDITOR
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, ".minecraft");
            path = Path.Combine(path, "saves");
            path = Path.Combine(path, "New World1");
#else
            string path = Path.Combine(Application.persistentDataPath, "saves", save);
#endif
            return path;
        }
    }

    static PlayerFile playerFile;
    static NbtTree playerData;
    static NBTFile levelFile;
    static NbtTree levelTree;
    static TagNodeCompound levelDat;
    public static void Init()
    {
        string[] files = Directory.GetFiles(Path.Combine(savePath, "playerdata"));

        if (files.Length == 0)
        {
            throw new Exception("no player data");
        }
        playerFile = new PlayerFile(files[0]);
        playerData = new NbtTree();
        using (Stream stream = playerFile.GetDataInputStream())
        {
            playerData.ReadFrom(stream);
        }

        levelFile = new NBTFile(Path.Combine(savePath, "level.dat"));
        levelTree = new NbtTree();
        using (Stream stream = levelFile.GetDataInputStream())
        {
            levelTree.ReadFrom(stream);
        }
        levelDat = levelTree.Root["Data"] as TagNodeCompound;

        if (!playerData.Root.ContainsKey("SpawnX"))
        {
            TagNodeList Pos = playerData.Root["Pos"] as TagNodeList;
            playerData.Root.Add("SpawnX", new TagNodeInt((int)((TagNodeDouble)Pos[0]).Data));
            playerData.Root.Add("SpawnY", new TagNodeInt((int)((TagNodeDouble)Pos[1]).Data));
            playerData.Root.Add("SpawnZ", new TagNodeInt((int)((TagNodeDouble)Pos[2]).Data));
            Debug.Log("no spawn pos, using saved pos as spawn pos");
        }

        IsAlive = true;
    }

    public static TagNodeCompound GetPlayerData()
    {
        return playerData.Root;
    }

    public static TagNodeCompound GetLevelDat()
    {
        return levelDat;
    }

    public static void Tick()
    {
        var chunks = chunkDict.Values;
        foreach (var chunk in chunkDict.Values)
        {
            chunk.Tick();
        }
    }

    public static void SavePlayerData()
    {
        TagNodeList Pos = playerData.Root["Pos"] as TagNodeList;
        Pos[0] = (TagNodeDouble)PlayerController.instance.transform.position.x;
        Pos[1] = (TagNodeDouble)PlayerController.instance.transform.position.y;
        Pos[2] = (TagNodeDouble)PlayerController.instance.transform.position.z;
        TagNodeList Rotation = playerData.Root["Rotation"] as TagNodeList;
        Rotation[0] = (TagNodeFloat)(-PlayerController.instance.transform.localEulerAngles.y);
        Rotation[1] = (TagNodeFloat)PlayerController.instance.camera.localEulerAngles.x;

        InventorySystem.SaveData(playerData.Root["Inventory"] as TagNodeList);

        using (Stream stream = playerFile.GetDataOutputStream())
        {
            playerData.WriteTo(stream);
        }
    }

    public static void SaveToDisk()
    {
        SavePlayerData();

        TagNodeCompound player = levelDat["Player"] as TagNodeCompound;
        TagNodeCompound abilities = player["abilities"] as TagNodeCompound;
        abilities["flying"] = (TagNodeByte)(PlayerController.instance.isFlying ? 1 : 0);
        levelDat["DayTime"] = (TagNodeLong)TimeOfDay.instance.tick;
        int mode = (int)GameModeManager.mode;
        levelDat["GameType"] = (TagNodeInt)mode;
        using (Stream stream = levelFile.GetDataOutputStream())
        {
            levelTree.WriteTo(stream);
        }

        foreach (var chunk in chunkDict.Values)
        {
            chunk.Serialize();
        }

        foreach (KeyValuePair<Vector2Int, NbtTree> kvPair in chunkDictNBT)
        {
            int chunkX = kvPair.Key.x;
            int chunkZ = kvPair.Key.y;
            int regionX = GetRegionCoordinate(chunkX);
            int regionZ = GetRegionCoordinate(chunkZ);
            RegionFile region = GetRegion(regionX, regionZ);
            int _x = chunkX - regionX * 32;
            int _z = chunkZ - regionZ * 32;
            using (Stream stream = region.GetChunkDataOutputStream(_x, _z))
            {
                kvPair.Value.WriteTo(stream);
            }
        }
    }

    public static Vector3 GetPlayerPos()
    {
        TagNodeCompound player = GetPlayerData();
        TagNodeList Pos = player["Pos"] as TagNodeList;
        TagNodeDouble x = Pos[0] as TagNodeDouble;
        TagNodeDouble y = Pos[1] as TagNodeDouble;
        TagNodeDouble z = Pos[2] as TagNodeDouble;
        Vector3 pos = new Vector3((float)x, (float)y, (float)z);
        return pos;
    }

    public static Vector3 GetPlayerRot()
    {
        TagNodeCompound player = GetPlayerData();
        TagNodeList Pos = player["Rotation"] as TagNodeList;
        TagNodeFloat y = Pos[0] as TagNodeFloat;
        TagNodeFloat x = Pos[1] as TagNodeFloat;
        Vector3 rot = new Vector3(0, y, x);
        return rot;
    }

    public static NBTChunk GetChunk(int chunkX, int chunkZ)
    {
        Vector2Int key = new Vector2Int(chunkX, chunkZ);
        if (chunkDict.ContainsKey(key))
        {
            return chunkDict[key];
        }
        return null;
    }

    public static NBTChunk GetChunk(Vector2Int pos)
    {
        if (chunkDict.ContainsKey(pos))
        {
            return chunkDict[pos];
        }
        return null;
    }

    public static NBTChunk GetChunk(Vector3Int pos)
    {
        int chunkX = Mathf.FloorToInt(pos.x / 16f);
        int chunkZ = Mathf.FloorToInt(pos.z / 16f);

        Vector2Int key = new Vector2Int(chunkX, chunkZ);
        if (chunkDict.ContainsKey(key))
        {
            return chunkDict[key];
        }
        return null;
    }

    public static NBTChunk LoadChunk(int chunkX, int chunkZ)
    {
        UnityEngine.Profiling.Profiler.BeginSample("ChunkChecker.Update");

        Vector2Int key = new Vector2Int(chunkX, chunkZ);
        if (!chunkDict.ContainsKey(key))
        {
            TagNodeCompound Chunk = GetChunkNode(chunkX, chunkZ);
            if (Chunk != null)
            {
                TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;
                NBTChunk chunk = ChunkPool.GetChunk();
                chunk.SetData(chunkX, chunkZ, Level);
                chunkDict.Add(key, chunk);
            }
        }

        UnityEngine.Profiling.Profiler.EndSample();

        if (chunkDict.ContainsKey(key))
        {
            return chunkDict[key];
        }
        return null;
    }

    public static async Task<NBTChunk> LoadChunkAsync(int chunkX, int chunkZ)
    {
        Vector2Int key = new Vector2Int(chunkX, chunkZ);
        if (!chunkDict.ContainsKey(key))
        {
            TagNodeCompound Chunk = await GetChunkNodeAsync(chunkX, chunkZ);
            if (Chunk != null)
            {
                TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;
                NBTChunk chunk = ChunkPool.GetChunk();
                chunk.SetData(chunkX, chunkZ, Level);
                chunkDict.Add(key, chunk);
            }
        }

        if (chunkDict.ContainsKey(key))
        {
            return chunkDict[key];
        }
        return null;
    }

    public static void RemoveChunk(int chunkX, int chunkZ)
    {
        Vector2Int key = new Vector2Int(chunkX, chunkZ);
        if (chunkDict.ContainsKey(key))
        {
            NBTChunk chunk = chunkDict[key];
            ChunkRefresher.Remove(chunk);
            chunk.Serialize();
            chunk.ClearData();
            ChunkPool.Recover(chunk);
            chunkDict.Remove(key);
        }
        if (chunkDictNBT.ContainsKey(key))
        {
            int regionX = GetRegionCoordinate(chunkX);
            int regionZ = GetRegionCoordinate(chunkZ);
            RegionFile region = GetRegion(regionX, regionZ);
            int _x = chunkX - regionX * 32;
            int _z = chunkZ - regionZ * 32;
            using (Stream stream = region.GetChunkDataOutputStream(_x, _z))
            {
                chunkDictNBT[key].WriteTo(stream);
            }
            chunkDictNBT.Remove(key);
        }
    }

    // get the chunks to be load (chunks should be loaded - chunks already loaded)
    public static List<Vector2Int> GetLoadChunks(Vector2Int centerChunkPos)
    {
        List<Vector2Int> shouldLoadChunks = Utilities.GetSurroudingChunks(centerChunkPos);
        return shouldLoadChunks.Except(chunkDict.Keys).ToList();
    }

    public static void RespawnRefreshChunks()
    {
        var preloadChunks = Utilities.GetSurroudingChunks(PlayerController.GetCurrentChunkPos(), 1);
        var unloadChunks = chunkDict.Keys.Except(preloadChunks).ToList();
        foreach (Vector2Int chunk in unloadChunks)
        {
            NBTHelper.RemoveChunk(chunk.x, chunk.y);
        }
        foreach (Vector2Int chunkPos in preloadChunks)
        {
            NBTChunk chunk = NBTHelper.LoadChunk(chunkPos.x, chunkPos.y);
            ChunkRefresher.Add(chunk);
        }
        ChunkRefresher.ForceRefreshAll();
    }

    // get the chunks to be unload (any chunks in the loaded chunks dict whose distance to the centerChunkPos is bigger than chunkRadius should be unloaded)
    public static List<Vector2Int> GetUnloadChunks(Vector2Int centerChunkPos, int chunkRadius)
    {
        return chunkDict.Keys.Where(chunkPos =>
            Mathf.Abs(chunkPos.x - centerChunkPos.x) > chunkRadius || Mathf.Abs(chunkPos.y - centerChunkPos.y) > chunkRadius
            ).ToList();
    }

    public static TagNodeCompound GetChunkNode(int x, int z)
    {
        Vector2Int key = new Vector2Int(x, z);

        if (!chunkDictNBT.ContainsKey(key))
        {
            int regionX = GetRegionCoordinate(x);
            int regionZ = GetRegionCoordinate(z);
            RegionFile region = GetRegion(regionX, regionZ);

            if (region != null)
            {
                int _x = x - regionX * 32;
                int _z = z - regionZ * 32;
                if (region.HasChunk(_x, _z))
                {
                    UnityEngine.Profiling.Profiler.BeginSample("GetChunkDataInputStream");
                    NbtTree _tree = new NbtTree();
                    using (Stream stream = region.GetChunkDataInputStream(_x, _z))
                    {
                        UnityEngine.Profiling.Profiler.BeginSample("NBTTree ReadFrom");
                        _tree.ReadFrom(stream);
                        UnityEngine.Profiling.Profiler.EndSample();
                    }
                    UnityEngine.Profiling.Profiler.EndSample();

                    chunkDictNBT.Add(key, _tree);
                }
            }
        }
        if (chunkDictNBT.ContainsKey(key))
        {
            return chunkDictNBT[key].Root;
        }
        return null;
    }

    public async static Task<TagNodeCompound> GetChunkNodeAsync(int x, int z)
    {
        Vector2Int key = new Vector2Int(x, z);

        if (!chunkDictNBT.ContainsKey(key))
        {
            int regionX = GetRegionCoordinate(x);
            int regionZ = GetRegionCoordinate(z);
            RegionFile region = GetRegion(regionX, regionZ);

            if (region != null)
            {
                int _x = x - regionX * 32;
                int _z = z - regionZ * 32;
                if (region.HasChunk(_x, _z))
                {
                    NbtTree _tree = new NbtTree();

                    using (Stream stream = region.GetChunkDataInputStream(_x, _z))
                    {
                        await Task.Run(() =>
                        {
                            _tree.ReadFrom(stream);
                        });
                    }

                    if (!IsAlive)
                        throw new Exception("exit game scene");

                    chunkDictNBT[key] = _tree;
                }
            }
            else
            {
                Debug.LogError("Region does not exist! need generation.");
            }
        }
        if (chunkDictNBT.ContainsKey(key))
        {
            return chunkDictNBT[key].Root;
        }
        return null;
    }

    public static byte GetNibble(byte[] arr, int index)
    {
        return (byte)(index % 2 == 0 ? arr[index / 2] & 0x0F : (arr[index / 2] >> 4) & 0x0F);
    }

    public static void SetNibble(byte[] arr, int index, byte data)
    {
        if (index % 2 == 0)
        {
            int newData = (arr[index / 2] & 0xF0) | (data & 0x0F);
            arr[index / 2] = (byte)newData;
        }
        else
        {
            int newData = (arr[index / 2] & 0x0F) | (data << 4 & 0xF0);
            arr[index / 2] = (byte)newData;
        }
    }

    public static RegionFile GetRegion(int regionX, int regionZ)
    {
        UnityEngine.Profiling.Profiler.BeginSample("NBTHelper.GetRegion");
        Vector2Int pos = new Vector2Int(regionX, regionZ);

        if (!regionDict.ContainsKey(pos))
        {
            string path = Path.Combine(savePath, "region");
            path = Path.Combine(path, "r." + regionX + "." + regionZ + ".mca");

            if (File.Exists(path))
            {
                regionDict.Add(pos, new RegionFile(path));
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();

        if (regionDict.ContainsKey(pos))
        {
            return regionDict[pos];
        }
        return null;
    }

    public static int GetRegionCoordinate(int val)
    {
        return Mathf.FloorToInt(val / 32.0f);
    }

    public static void SetBlockByteNoUpdate(Vector3Int pos, byte type) { SetBlockByteNoUpdate(pos.x, pos.y, pos.z, type); }
    public static void SetBlockByteNoUpdate(int x, int y, int z, byte type)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        chunk.SetBlockByte(xInChunk, y, zInChunk, type);
    }

    static UpdateFlags GetUpdateFlags(NBTBlock generator)
    {
        if (generator is NBTPlant || generator is NBTSnowLayer)
        {
            return UpdateFlags.NotCollidable;
        }
        else if (generator is NBTStationaryWater)
        {
            return UpdateFlags.Water;
        }
        else
        {
            return UpdateFlags.Collidable | UpdateFlags.Lighting;
        }
    }

    public static void SetBlockByte(Vector3Int pos, byte type)
    {
        SetBlockByte(pos.x, pos.y, pos.z, type);
    }

    public static void SetBlockByte(int x, int y, int z, byte type)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        NBTBlock oldGenerator = NBTGeneratorManager.GetMeshGenerator(chunk.GetBlockByte(xInChunk, y, zInChunk));
        UpdateFlags updateFlag = GetUpdateFlags(oldGenerator);

        if (type == 0)
        {
            chunk.GetBlockData(xInChunk, y + 1, zInChunk, out byte topType, out byte topData);
            NBTBlock topGenerator = NBTGeneratorManager.GetMeshGenerator(topType);
            if (topGenerator != null && topGenerator is NBTPlant || topGenerator is NBTSnowLayer)
            {
                BreakBlockEffect.Create(topType, topData, new Vector3(x, y + 1, z));
                chunk.SetBlockByte(xInChunk, y + 1, zInChunk, 0);
                updateFlag |= UpdateFlags.NotCollidable;
            }
        }

        chunk.SetBlockByte(xInChunk, y, zInChunk, type);
        if (updateFlag.HasFlag(UpdateFlags.Lighting))
        {
            UpdateLighting(x, y, z);
        }
        chunk.RebuildMesh(updateFlag);

        NBTChunk leftChunk = GetChunk(chunkX - 1, chunkZ);
        if (xInChunk == 0)
            leftChunk.RebuildMesh();

        NBTChunk rightChunk = GetChunk(chunkX + 1, chunkZ);
        if (xInChunk == 15)
            rightChunk.RebuildMesh();

        NBTChunk backChunk = GetChunk(chunkX, chunkZ - 1);
        if (zInChunk == 0)
            backChunk.RebuildMesh();

        NBTChunk frontChunk = GetChunk(chunkX, chunkZ + 1);
        if (zInChunk == 15)
            frontChunk.RebuildMesh();
    }

    public static void SetBlockData(Vector3Int pos, byte type, byte data)
    {
        SetBlockData(pos.x, pos.y, pos.z, type, data);
    }

    public static void SetBlockData(int x, int y, int z, byte type, byte data)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        NBTBlock oldGenerator = NBTGeneratorManager.GetMeshGenerator(chunk.GetBlockByte(xInChunk, y, zInChunk));
        UpdateFlags updateFlag = GetUpdateFlags(oldGenerator);

        if (type == 0)
        {
            chunk.GetBlockData(xInChunk, y + 1, zInChunk, out byte topType, out byte topData);
            NBTBlock topGenerator = NBTGeneratorManager.GetMeshGenerator(topType);
            if (topGenerator != null && topGenerator is NBTPlant || topGenerator is NBTSnowLayer)
            {
                BreakBlockEffect.Create(topType, topData, new Vector3(x, y + 1, z));
                chunk.SetBlockByte(xInChunk, y + 1, zInChunk, 0);
                updateFlag |= UpdateFlags.NotCollidable;
            }
        }
        else
        {
            NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(type);
            updateFlag |= GetUpdateFlags(generator);
        }

        chunk.SetBlockData(xInChunk, y, zInChunk, type, data);
        if (updateFlag.HasFlag(UpdateFlags.Lighting))
        {
            UpdateLighting(x, y, z);
        }
        chunk.RebuildMesh(updateFlag);

        if (type == 0 && !oldGenerator.isTransparent)
        {
            // update border chunks if neccesary
            NBTChunk leftChunk = GetChunk(chunkX - 1, chunkZ);
            if (xInChunk == 0)
                leftChunk.RebuildMesh();
            else
                leftChunk.RebuildMeshAsync();

            NBTChunk rightChunk = GetChunk(chunkX + 1, chunkZ);
            if (xInChunk == 15)
                rightChunk.RebuildMesh();
            else
                rightChunk.RebuildMeshAsync();

            NBTChunk backChunk = GetChunk(chunkX, chunkZ - 1);
            if (zInChunk == 0)
                backChunk.RebuildMesh();
            else
                backChunk.RebuildMeshAsync();

            NBTChunk frontChunk = GetChunk(chunkX, chunkZ + 1);
            if (zInChunk == 15)
                frontChunk.RebuildMesh();
            else
                frontChunk.RebuildMeshAsync();
        }
    }

    public static byte GetBlockByte(Vector3Int pos) { return GetBlockByte(pos.x, pos.y, pos.z); }

    public static byte GetBlockByte(int x, int y, int z)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkY = Mathf.FloorToInt(y / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int yInChunk = y - chunkY * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);

        if (chunk != null)
        {
            return chunk.GetBlockByte(xInChunk, y, zInChunk);
        }
        else
        {
            //TagNodeCompound Chunk = GetChunkNode(chunkX, chunkZ);
            //if (Chunk != null)
            //{
            //    TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;

            //    TagNodeList Sections = Level["Sections"] as TagNodeList;
            //    if (chunkY < Sections.Count)
            //    {
            //        TagNodeCompound section = Sections[chunkY] as TagNodeCompound;

            //        TagNodeByteArray Blocks = section["Blocks"] as TagNodeByteArray;
            //        byte[] blocks = new byte[4096];
            //        Buffer.BlockCopy(Blocks, 0, blocks, 0, 4096);

            //        int blockPos = yInChunk * 16 * 16 + zInChunk * 16 + xInChunk;
            //        return blocks[blockPos];
            //    }
            //}
        }
        return 0;
    }

    public static void GetBlockData(int x, int y, int z, out byte blockType, out byte blockData)
    {
        UnityEngine.Profiling.Profiler.BeginSample("GetBlockData");
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkY = Mathf.FloorToInt(y / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int yInChunk = y - chunkY * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);

        if (chunk != null)
        {
            chunk.GetBlockData(xInChunk, y, zInChunk, out blockType, out blockData);
        }
        else
        {
            blockType = 0;
            blockData = 0;
            //UnityEngine.Profiling.Profiler.BeginSample("GetChunkNode");
            //TagNodeCompound Chunk = GetChunkNode(chunkX, chunkZ);
            //UnityEngine.Profiling.Profiler.EndSample();

            //if (Chunk != null)
            //{
            //    UnityEngine.Profiling.Profiler.BeginSample("GetBlockData new block");
            //    TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;

            //    TagNodeList Sections = Level["Sections"] as TagNodeList;
            //    if (chunkY < Sections.Count)
            //    {
            //        TagNodeCompound section = Sections[chunkY] as TagNodeCompound;

            //        TagNodeByteArray Blocks = section["Blocks"] as TagNodeByteArray;
            //        byte[] blocks = new byte[4096];
            //        Buffer.BlockCopy(Blocks, 0, blocks, 0, 4096);

            //        int blockPos = yInChunk * 16 * 16 + zInChunk * 16 + xInChunk;
            //        blockType = blocks[blockPos];

            //        TagNodeByteArray Data = section["Data"] as TagNodeByteArray;
            //        byte[] data = Data.Data;
            //        blockData = GetNibble(data, blockPos);
            //    }
            //    UnityEngine.Profiling.Profiler.EndSample();
            //}
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public static byte GetSkyLightByte(int x, int y, int z)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        if (chunk == null)
        {
            Debug.LogError("no chunk! x=" + x + ",y=" + y + ",z=" + z);
            return 0;
        }
        return chunk.GetSkyLightByte(xInChunk, y, zInChunk);
    }

    public static void GetLightsByte(int x, int y, int z, out byte skyLight, out byte blockLight)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        chunk.GetLightsByte(xInChunk, y, zInChunk, out skyLight, out blockLight);
    }

    public static void SetSkyLightByte(int x, int y, int z, byte skyLight)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        chunk.SetSkyLightByte(xInChunk, y, zInChunk, skyLight);
    }

    public static void UpdateLighting(int x, int y, int z)
    {
        //Debug.Log("update lighting");
        //float start = Time.realtimeSinceStartup;

        Queue<Vector3Int> skyLightQueue = new Queue<Vector3Int>();
        Vector3Int p = new Vector3Int();

        // init
        for (int i = -15; i <= 15; i++)
        {
            for (int j = -15; j <= 15; j++)
            {
                p.Set(x + i, y, z + j);

                int chunkX = Mathf.FloorToInt(p.x / 16f);
                int chunkZ = Mathf.FloorToInt(p.z / 16f);

                int xInChunk = p.x - chunkX * 16;
                int zInChunk = p.z - chunkZ * 16;

                NBTChunk chunk = GetChunk(chunkX, chunkZ);
                for (int temp_y = chunk.Sections.Count * 16 - 1; temp_y >= 0; temp_y--)
                {
                    chunk.SetSkyLightByte(xInChunk, temp_y, zInChunk, 0);
                }
            }
        }

        HashSet<Vector3Int> skyLightSet = new HashSet<Vector3Int>();

        // light from sun (no vertical falloff)
        for (int i = -15; i <= 15; i++)
        {
            for (int j = -15; j <= 15; j++)
            {
                p.Set(x + i, y, z + j);

                int chunkX = Mathf.FloorToInt(p.x / 16f);
                int chunkZ = Mathf.FloorToInt(p.z / 16f);

                int xInChunk = p.x - chunkX * 16;
                int zInChunk = p.z - chunkZ * 16;

                NBTChunk chunk = GetChunk(chunkX, chunkZ);
                int temp_y = chunk.Sections.Count * 16 - 1;
                while (NBTGeneratorManager.LightCanTravel(chunk.GetBlockByte(xInChunk, temp_y, zInChunk)))
                {
                    chunk.SetSkyLightByte(xInChunk, temp_y, zInChunk, 15);
                    skyLightSet.Add(new Vector3Int(p.x, temp_y, p.z));
                    if (temp_y == 0)
                        break;
                    else
                        temp_y--;
                }
            }
        }

        foreach (Vector3Int skyLightPos in skyLightSet)
        {
            if (skyLightSet.Contains(skyLightPos + Vector3Int.left) &&
                skyLightSet.Contains(skyLightPos + Vector3Int.right) &&
                skyLightSet.Contains(skyLightPos + Vector3Int.forward) &&
                skyLightSet.Contains(skyLightPos + Vector3Int.back))
            {
                continue;
            }
            skyLightQueue.Enqueue(new Vector3Int(skyLightPos.x, skyLightPos.y, skyLightPos.z));
        }

        int count = 0;
        int setcount = 0;

        // light propagation (use flood fill)
        Vector3Int[] arr = new Vector3Int[6];
        while (skyLightQueue.Count > 0)
        {
            count++;
            p = skyLightQueue.Dequeue();
            byte skyLight = GetSkyLightByte(p.x, p.y, p.z);

            arr[0] = p + Vector3Int.left;
            arr[1] = p + Vector3Int.right;
            arr[2] = p + Vector3Int.up;
            arr[3] = p + Vector3Int.down;
            arr[4] = p + Vector3Int.forward;
            arr[5] = p + Vector3Int.back;

            for (int i = 0; i < 6; i++)
            {
                Vector3Int nextPos = arr[i];
                if (NBTGeneratorManager.IsTransparent(GetBlockByte(nextPos.x, nextPos.y, nextPos.z)))
                {
                    byte nextSkyLight = GetSkyLightByte(nextPos.x, nextPos.y, nextPos.z);
                    if (nextSkyLight < skyLight - 1)
                    {
                        setcount++;
                        //Debug.Log("SetSkyLightByte,nextPos=" + nextPos.x + "," + nextPos.y + "," + nextPos.z);
                        SetSkyLightByte(nextPos.x, nextPos.y, nextPos.z, (byte)(skyLight - 1));
                        if (skyLight > 2)
                            skyLightQueue.Enqueue(nextPos);
                    }
                }
            }
        }

        //float end = Time.realtimeSinceStartup;
        //Debug.Log("time cost =" + (end - start) + ", actual propagation count=" + count + ",setcount=" + setcount);
    }

    public static void DebugInfo()
    {
        int collidableMaxVertexCount = 0;
        int collidableMaxTriangleCount = 0;
        int notCollidableMaxVertexCount = 0;
        int notCollidableMaxTriangleCount = 0;
        int waterMaxVertexCount = 0;
        int waterMaxTriangleCount = 0;
        foreach (var item in chunkDict)
        {
            NBTChunk chunk = item.Value;
            if (chunk.collidable.nbtMesh.vertexCount > collidableMaxVertexCount)
            {
                collidableMaxVertexCount = chunk.collidable.nbtMesh.vertexCount;
            }
            if (chunk.collidable.nbtMesh.triangleCount > collidableMaxTriangleCount)
            {
                collidableMaxTriangleCount = chunk.collidable.nbtMesh.triangleCount;
            }
            if (chunk.notCollidable.nbtMesh.vertexCount > notCollidableMaxVertexCount)
            {
                notCollidableMaxVertexCount = chunk.notCollidable.nbtMesh.vertexCount;
            }
            if (chunk.notCollidable.nbtMesh.triangleCount > notCollidableMaxTriangleCount)
            {
                notCollidableMaxTriangleCount = chunk.notCollidable.nbtMesh.triangleCount;
            }
            if (chunk.water.nbtMesh.vertexCount > waterMaxVertexCount)
            {
                waterMaxVertexCount = chunk.water.nbtMesh.vertexCount;
            }
            if (chunk.water.nbtMesh.triangleCount > waterMaxTriangleCount)
            {
                waterMaxTriangleCount = chunk.water.nbtMesh.triangleCount;
            }
        }
        Debug.Log(ChatPanel.ShowCode + "collidableMaxVertexCount=" + collidableMaxVertexCount);
        Debug.Log(ChatPanel.ShowCode + "collidableMaxTriangleCount=" + collidableMaxTriangleCount);
        Debug.Log(ChatPanel.ShowCode + "notCollidableMaxVertexCount=" + notCollidableMaxVertexCount);
        Debug.Log(ChatPanel.ShowCode + "notCollidableMaxTriangleCount=" + notCollidableMaxTriangleCount);
        Debug.Log(ChatPanel.ShowCode + "waterMaxVertexCount=" + waterMaxVertexCount);
        Debug.Log(ChatPanel.ShowCode + "waterMaxTriangleCount=" + waterMaxTriangleCount);
    }

    public static void Uninit()
    {
        IsAlive = false;
        foreach (KeyValuePair<Vector2Int, RegionFile> kvPair in regionDict)
        {
            kvPair.Value.Close();
            kvPair.Value.Dispose();
        }
        regionDict.Clear();
        chunkDictNBT.Clear();
        chunkDict.Clear();
    }
}
