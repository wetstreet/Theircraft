using Substrate.Nbt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NBTEditorWindow : EditorWindow
{
    [MenuItem("Window/NBT Editor Window")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(NBTEditorWindow));
        window.Show();
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
    Color c;
    float AdjTemp;
    float AdjRainfall;
    private void OnGUI()
    {
        pos = EditorGUILayout.Vector3IntField("pos", pos);
        GUILayout.Label("type=" + type);
        GUILayout.Label("data=" + blockdata);
        GUILayout.Label("biome=" + biomeType);
        if (GUILayout.Button("Update"))
        {
            int chunkX = Mathf.FloorToInt(pos.x / 16f);
            int chunkY = Mathf.FloorToInt(pos.y / 16f);
            int chunkZ = Mathf.FloorToInt(pos.z / 16f);

            int xInChunk = pos.x - chunkX * 16;
            int yInChunk = pos.y - chunkY * 16;
            int zInChunk = pos.z - chunkZ * 16;

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

                    int blockPos = yInChunk * 16 * 16 + zInChunk * 16 + xInChunk;
                    type = blocks[blockPos];

                    TagNodeByteArray Data = section["Data"] as TagNodeByteArray;
                    byte[] data = Data.Data;
                    blockdata = NBTHelper.GetNibble(data, blockPos);
                }

                TagNodeByteArray Biomes = Level["Biomes"] as TagNodeByteArray;
                biomeType = Biomes[xInChunk * 16 + zInChunk];
            }

            Biome biome = gBiomes[biomeType];
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
