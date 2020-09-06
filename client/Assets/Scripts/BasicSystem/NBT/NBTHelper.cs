using Substrate.Core;
using Substrate.Nbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NBTHelper : MonoBehaviour
{
    public static string save = "New World";

    private static Dictionary<Vector2Int, NBTChunk> chunkDict = new Dictionary<Vector2Int, NBTChunk>();

    private static Vector2Int key = new Vector2Int();
    public static NBTChunk GetChunk(int chunkX, int chunkZ)
    {
        key.Set(chunkX, chunkZ);
        if (!chunkDict.ContainsKey(key))
        {
            TagNodeCompound Chunk = GetChunkNode(chunkX, chunkZ);
            if (Chunk != null)
            {
                TagNodeCompound Level = Chunk["Level"] as TagNodeCompound;

                TagNodeList Sections = Level["Sections"] as TagNodeList;
                NBTChunk chunk = new NBTChunk();
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
}
