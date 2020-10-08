﻿using Substrate.Core;
using Substrate.Nbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NBTHelper : MonoBehaviour
{
    public static string save = "New World1";

    private static Dictionary<Vector2Int, NBTChunk> chunkDict = new Dictionary<Vector2Int, NBTChunk>();

    public static TagNodeCompound GetPlayerData()
    {
        string path = Environment.ExpandEnvironmentVariables("%APPDATA%");
        if (!Directory.Exists(path))
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        path = Path.Combine(path, ".minecraft");
        path = Path.Combine(path, "saves");
        path = Path.Combine(path, save);
        path = Path.Combine(path, "playerdata");
        string[] files = Directory.GetFiles(path);

        if (files.Length > 0)
        {
            PlayerFile player = new PlayerFile(files[0]);
            NbtTree _tree = new NbtTree();
            _tree.ReadFrom(player.GetDataInputStream());
            return _tree.Root;
        }
        return null;
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

    public static NBTChunk LoadChunk(int chunkX, int chunkZ)
    {
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
                _tree.ReadFrom(region.GetChunkDataInputStream(_x, _z));
                return _tree.Root;
            }
        }
        return null;
    }

    public static byte GetNibble(byte[] arr, int index)
    {
        return (byte)(index % 2 == 0 ? arr[index / 2] & 0x0F : (arr[index / 2] >> 4) & 0x0F);
    }

    public static Dictionary<Vector2Int, RegionFile> regionDict = new Dictionary<Vector2Int, RegionFile>();

    public static RegionFile GetRegion(int regionX, int regionZ)
    {
        Vector2Int pos = new Vector2Int(regionX, regionZ);

        if (!regionDict.ContainsKey(pos))
        {
            string path = Environment.ExpandEnvironmentVariables("%APPDATA%");
            if (!Directory.Exists(path))
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            path = Path.Combine(path, ".minecraft");
            path = Path.Combine(path, "saves");
            path = Path.Combine(path, save);
            path = Path.Combine(path, "region");
            path = Path.Combine(path, "r." + regionX + "." + regionZ + ".mca");

            if (File.Exists(path))
            {
                regionDict.Add(pos, new RegionFile(path));
            }
        }

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

    public static void PrintBlockData(int x, int y, int z)
    {
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkY = Mathf.FloorToInt(y / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);
        TagNodeCompound Chunk = NBTHelper.GetChunkNode(chunkX, chunkZ);
        if (Chunk != null)
        {
            TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;

            TagNodeList Sections = Level["Sections"] as TagNodeList;
            if (chunkY < Sections.Count)
            {
                TagNodeCompound section = Sections[chunkY] as TagNodeCompound;

                TagNodeByteArray Blocks = section["Blocks"] as TagNodeByteArray;
                byte[] blocks = new byte[4096];
                Buffer.BlockCopy(Blocks, 0, blocks, 0, 4096);

                TagNodeByteArray Data = section["Data"] as TagNodeByteArray;
                byte[] data = Data.Data;

                int xInChunk = x - chunkX * 16;
                int yInChunk = y - chunkY * 16;
                int zInChunk = z - chunkZ * 16;
                int blockPos = yInChunk * 16 * 16 + zInChunk * 16 + xInChunk;

                Debug.Log("type=" + blocks[blockPos] + ",data=" + NBTHelper.GetNibble(data, blockPos));

                //string blockstring = "";
                //for (int i = 0; i < 4096; i++)
                //{
                //    blockstring += blocks[i] + ",";
                //}
                //Debug.Log("chunk=(" + chunkX + "," + chunkY + "," + chunkZ + "),PosInChunk=("+ xInChunk + "," + yInChunk + "," + zInChunk + ")\n" + blockstring);
            }
            else
            {
                Debug.Log("section not exist!");
            }
        }
        else
        {
            Debug.Log("chunk not exist!");
        }
    }

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
        int chunkX = Mathf.FloorToInt(x / 16f);
        int chunkY = Mathf.FloorToInt(y / 16f);
        int chunkZ = Mathf.FloorToInt(z / 16f);

        int xInChunk = x - chunkX * 16;
        int yInChunk = y - chunkY * 16;
        int zInChunk = z - chunkZ * 16;

        //NBTChunk chunk = GetChunk(chunkX, chunkZ);

        //if (chunk != null)
        //{
        //    chunk.GetBlockData(xInChunk, y, zInChunk, ref blockType, ref blockData);
        //}
        //else
        //{
            TagNodeCompound Chunk = GetChunkNode(chunkX, chunkZ);
            if (Chunk != null)
            {
                TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;

                TagNodeList Sections = Level["Sections"] as TagNodeList;
                if (chunkY < Sections.Count)
                {
                    TagNodeCompound section = Sections[chunkY] as TagNodeCompound;

                    TagNodeByteArray Blocks = section["Blocks"] as TagNodeByteArray;
                    byte[] blocks = new byte[4096];
                    Buffer.BlockCopy(Blocks, 0, blocks, 0, 4096);

                    int blockPos = yInChunk * 16 * 16 + zInChunk * 16 + xInChunk;
                    blockType = blocks[blockPos];

                    TagNodeByteArray Data = section["Data"] as TagNodeByteArray;
                    byte[] data = Data.Data;
                    blockData = GetNibble(data, blockPos);
                }
            }
        //}
    }
}
