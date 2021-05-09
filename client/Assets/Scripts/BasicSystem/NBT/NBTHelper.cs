using Substrate.Core;
using Substrate.Nbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

public class NBTHelper
{
    public static string save = "New World-";

    private static Dictionary<Vector2Int, NBTChunk> chunkDict = new Dictionary<Vector2Int, NBTChunk>();

    static string savePath { get
        {
            //string path = Environment.ExpandEnvironmentVariables("%APPDATA%");
            //path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //path = Path.Combine(path, ".minecraft");
            //path = Path.Combine(path, "saves");

            string path = Path.Combine(Application.streamingAssetsPath, "saves");
            path = Path.Combine(path, save);
            return path;
        }
    }

    public static void Save()
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

    static PlayerFile playerFile;
    static NbtTree playerData;
    public static TagNodeCompound GetPlayerData()
    {
        if (playerData == null)
        {
            string path = Path.Combine(savePath, "playerdata");
            string[] files = Directory.GetFiles(path);

            if (files.Length == 0)
            {
                throw new Exception("no player data");
            }
            playerFile = new PlayerFile(files[0]);
            playerData = new NbtTree();
            playerData.ReadFrom(playerFile.GetDataInputStream());
        }
        return playerData.Root;
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

    private static Vector2Int key = new Vector2Int();
    public static NBTChunk GetChunk(int chunkX, int chunkZ)
    {
        key.Set(chunkX, chunkZ);
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

        key.Set(chunkX, chunkZ);
        if (chunkDict.ContainsKey(key))
        {
            return chunkDict[key];
        }
        return null;
    }
    public static NBTChunk LoadChunk(int chunkX, int chunkZ)
    {
        UnityEngine.Profiling.Profiler.BeginSample("ChunkChecker.Update");

        key.Set(chunkX, chunkZ);
        if (!chunkDict.ContainsKey(key))
        {
            TagNodeCompound Chunk = GetChunkNode(chunkX, chunkZ);
            if (Chunk != null)
            {
                TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;

                TagNodeList Sections = Level["Sections"] as TagNodeList;
                NBTChunk chunk = ChunkPool.GetChunk();
                chunk.SetData(chunkX, chunkZ, Sections);
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
        UnityEngine.Profiling.Profiler.BeginSample("ChunkChecker.Update");

        Vector2Int key = new Vector2Int(chunkX, chunkZ);
        if (!chunkDict.ContainsKey(key))
        {
            TagNodeCompound Chunk = await GetChunkNodeAsync(chunkX, chunkZ);
            Debug.Log("chunk load callback,x=" + chunkX + ",z=" + chunkZ);
            if (Chunk != null)
            {
                TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;

                TagNodeList Sections = Level["Sections"] as TagNodeList;
                NBTChunk chunk = ChunkPool.GetChunk();
                chunk.SetData(chunkX, chunkZ, Sections);
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

    public static void RemoveChunk(int chunkX, int chunkZ)
    {
        key.Set(chunkX, chunkZ);
        if (chunkDict.ContainsKey(key))
        {
            NBTChunk chunk = chunkDict[key];
            ChunkRefresher.Remove(chunk);
            chunk.ClearData();
            ChunkPool.Recover(chunk);
            chunkDict.Remove(key);
        }
    }

    // get the chunks to be load (chunks should be loaded - chunks already loaded)
    public static List<Vector2Int> GetLoadChunks(Vector2Int centerChunkPos)
    {
        List<Vector2Int> shouldLoadChunks = Utilities.GetSurroudingChunks(centerChunkPos);
        return shouldLoadChunks.Except(chunkDict.Keys).ToList();
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
        key.Set(x, z);

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

    static Dictionary<Vector2Int, NbtTree> chunkDictNBT = new Dictionary<Vector2Int, NbtTree>();
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
                    UnityEngine.Profiling.Profiler.BeginSample("GetChunkDataInputStream");
                    NbtTree _tree = new NbtTree();

                    UnityEngine.Profiling.Profiler.BeginSample("NBTTree ReadFrom");

                    Stream stream = region.GetChunkDataInputStream(_x, _z);

                    await Task.Run(() =>
                    {
                        _tree.ReadFrom(stream);
                    });

                    chunkDictNBT[key] = _tree;

                    UnityEngine.Profiling.Profiler.EndSample();

                    UnityEngine.Profiling.Profiler.EndSample();
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

    public static Dictionary<Vector2Int, RegionFile> regionDict = new Dictionary<Vector2Int, RegionFile>();

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

    public static void SetBlockByte(Vector3Int pos, byte type) { SetBlockByte(pos.x, pos.y, pos.z, type); }

    public static void SetBlockByte(int x, int y, int z, byte type)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        chunk.SetBlockByte(xInChunk, y, zInChunk, type);
        chunk.RebuildMesh();

        if (type == 0)
        {
            if (xInChunk == 0)
            {
                NBTChunk leftChunk = GetChunk(chunkX - 1, chunkZ);
                leftChunk.RebuildMesh();
            }
            if (xInChunk == 15)
            {
                NBTChunk rightChunk = GetChunk(chunkX + 1, chunkZ);
                rightChunk.RebuildMesh();
            }
            if (zInChunk == 0)
            {
                NBTChunk frontChunk = GetChunk(chunkX, chunkZ - 1);
                frontChunk.RebuildMesh();
            }
            if (zInChunk == 15)
            {
                NBTChunk backChunk = GetChunk(chunkX, chunkZ + 1);
                backChunk.RebuildMesh();
            }
        }
    }

    public static void SetBlockData(Vector3Int pos, byte type, byte data) { SetBlockData(pos.x, pos.y, pos.z, type, data); }

    public static void SetBlockData(int x, int y, int z, byte type, byte data)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int zInChunk = z - chunkZ * 16;

        NBTChunk chunk = GetChunk(chunkX, chunkZ);
        chunk.SetBlockData(xInChunk, y, zInChunk, type, data);
        chunk.RebuildMesh();

        if (type == 0)
        {
            if (xInChunk == 0)
            {
                NBTChunk leftChunk = GetChunk(chunkX - 1, chunkZ);
                leftChunk.RebuildMesh();
            }
            if (xInChunk == 15)
            {
                NBTChunk rightChunk = GetChunk(chunkX + 1, chunkZ);
                rightChunk.RebuildMesh();
            }
            if (zInChunk == 0)
            {
                NBTChunk frontChunk = GetChunk(chunkX, chunkZ - 1);
                frontChunk.RebuildMesh();
            }
            if (zInChunk == 15)
            {
                NBTChunk backChunk = GetChunk(chunkX, chunkZ + 1);
                backChunk.RebuildMesh();
            }
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

    public static void GetBlockData(int x, int y, int z, ref byte blockType, ref byte blockData)
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
            chunk.GetBlockData(xInChunk, y, zInChunk, ref blockType, ref blockData);
        }
        else
        {
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

    public static void Uninit()
    {
        foreach (KeyValuePair<Vector2Int, RegionFile> kvPair in regionDict)
        {
            kvPair.Value.Close();
            kvPair.Value.Dispose();
        }
    }
}
