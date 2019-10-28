using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    static readonly float scale = 35;
    static readonly int maxHeight = 15;

    public static byte[] GenerateChunkData(CSVector2Int chunk)
    {
        byte[] blocks = new byte[65536];
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                float x = 0.5f + i + chunk.x * 16;
                float z = 0.5f + j + chunk.y * 16;
                float noise = Mathf.PerlinNoise(x / scale, z / scale);
                int height = Mathf.RoundToInt(maxHeight * noise);
                for (int k = height; k >= 0; k--)
                {
                    //Ultilities.Print($"i={i},j={j},height{item.position.y}");
                    CSBlockType type;
                    if (k == height)
                    {
                        type = CSBlockType.GrassBlock;
                    }
                    else
                    {
                        type = CSBlockType.Dirt;
                    }
                    blocks[256 * k + 16 * i + j] = (byte)type;
                }
            }
        }
        return blocks;
    }
}
