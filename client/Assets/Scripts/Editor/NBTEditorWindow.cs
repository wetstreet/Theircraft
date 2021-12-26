using Substrate.Core;
using Substrate.Nbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NBTEditorWindow : EditorWindow
{
    [MenuItem("Window/NBT Editor Window")]
    static void Init()
    {
        NBTEditorWindow window = (NBTEditorWindow)GetWindow(typeof(NBTEditorWindow));
        window.Show();
        window.save = NBTHelper.save;
    }

    struct Biome
    {
        public string name;
        public float temp;
        public float rainfall;

        public Biome(string name, float temp, float rainfall)
        {
            this.name = name;
            this.temp = temp;
            this.rainfall = rainfall;
        }
    }

    static Biome[] gBiomes =
    {
        new Biome("Ocean", 0.5f, 0.5f),
        new Biome("Plains", 0.8f, 0.4f),
        new Biome("Desert", 2.0f, 0.0f),
        new Biome("Mountains", 0.2f, 0.3f),
        new Biome("Forest", 0.7f, 0.8f),
        new Biome("Taiga", 0.25f, 0.8f),
    };

    Vector3Int pos;
    byte type;
    byte biomeType;
    byte blockdata;
    byte skyLight;
    byte blockLight;
    Color c;
    float AdjTemp;
    float AdjRainfall;
    public string save = "New World1";
    private void OnGUI()
    {
        save = EditorGUILayout.TextField("save", save);

        pos = EditorGUILayout.Vector3IntField("pos", pos);
        GUILayout.Label("type=" + type);
        GUILayout.Label("data=" + blockdata);
        GUILayout.Label("biome=" + biomeType);
        GUILayout.Label("skyLight=" + skyLight);
        GUILayout.Label("blockLight=" + blockLight);
        if (GUILayout.Button("Update"))
        {
            int chunkX = Mathf.FloorToInt(pos.x / 16f);
            int chunkY = Mathf.FloorToInt(pos.y / 16f);
            int chunkZ = Mathf.FloorToInt(pos.z / 16f);

            int xInChunk = pos.x - chunkX * 16;
            int yInChunk = pos.y - chunkY * 16;
            int zInChunk = pos.z - chunkZ * 16;

            TagNodeCompound Chunk = null;

            int regionX = NBTHelper.GetRegionCoordinate(chunkX);
            int regionZ = NBTHelper.GetRegionCoordinate(chunkZ);

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
            RegionFile region = new RegionFile(path);

            if (region != null)
            {
                int _x = chunkX - regionX * 32;
                int _z = chunkZ - regionZ * 32;
                if (region.HasChunk(_x, _z))
                {
                    NbtTree _tree = new NbtTree();
                    using (Stream stream = region.GetChunkDataInputStream(_x, _z))
                    {
                        _tree.ReadFrom(stream);
                    }
                    Chunk = _tree.Root;
                }
            }
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
                    type = blocks[blockPos];

                    TagNodeByteArray Data = section["Data"] as TagNodeByteArray;
                    blockdata = NBTHelper.GetNibble(Data.Data, blockPos);

                    TagNodeByteArray SkyLight = section["SkyLight"] as TagNodeByteArray;
                    skyLight = NBTHelper.GetNibble(SkyLight.Data, blockPos);

                    TagNodeByteArray BlockLight = section["BlockLight"] as TagNodeByteArray;
                    blockLight = NBTHelper.GetNibble(BlockLight.Data, blockPos);
                }

                TagNodeByteArray Biomes = Level["Biomes"] as TagNodeByteArray;
                biomeType = Biomes[xInChunk * 16 + zInChunk];
            }

            Biome biome;
            if (biomeType < 6)
            {
                biome = gBiomes[biomeType];
            }
            else
            {
                biome = gBiomes[0];
                Debug.Log("no biome,type=" + biomeType);
            }
            float temp = biome.temp - Mathf.Max(pos.y - 64, 0) * 0.0016f;
            AdjTemp = Mathf.Clamp01(temp);
            AdjRainfall = Mathf.Clamp01(biome.rainfall) * AdjTemp;

            Vector2 uv = new Vector2(AdjTemp, AdjRainfall);
            Texture2D grass = Resources.Load<Texture2D>("GUI/grass");
            c = grass.GetPixelBilinear(1 - AdjTemp, AdjRainfall);
        }
        GUILayout.Label("temp=" + AdjTemp);
        GUILayout.Label("rainfall=" + AdjRainfall);
        EditorGUILayout.ColorField(c, GUILayout.Height(30));
    }
}
