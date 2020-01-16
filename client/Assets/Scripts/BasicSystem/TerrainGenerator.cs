﻿using protocol.cs_theircraft;
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
                    CSBlockType type = CSBlockType.None;
                    if (k == 0)
                    {
                        type = CSBlockType.BedRock;
                    }
                    else
                    {
                        int distanceFromHighestBlock = height - k;
                        //Ultilities.Print($"i={i},j={j},height{item.position.y}");
                        switch (distanceFromHighestBlock)
                        {
                            case 0:
                                //random surface block
                                int dice = Random.Range(1, 200);
                                if (dice <= 20)
                                {
                                    int plantdice = Random.Range(1, 5);
                                    switch (plantdice)
                                    {
                                        case 1:
                                        case 2:
                                            type = CSBlockType.Grass;
                                            break;
                                        case 3:
                                            type = CSBlockType.Poppy;
                                            break;
                                        case 4:
                                            type = CSBlockType.Dandelion;
                                            break;
                                    }
                                }
                                else if (dice == 199)
                                {
                                    int treedice = Random.Range(1, 10);
                                    if (treedice == 1)
                                    {
                                        GenerateTree(blocks, i, k, j);
                                    }
                                }
                                break;
                            case 1:
                                type = CSBlockType.GrassBlock;
                                break;
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                type = CSBlockType.Dirt;
                                break;
                            default:
                                type = CSBlockType.Stone;
                                break;
                        }
                    }
                    blocks[256 * k + 16 * i + j] = (byte)type;
                }
            }
        }
        return blocks;
    }

    static void SetBlockType(byte[] blocks, int x, int y, int z, CSBlockType type)
    {
        blocks[256 * y + 16 * x + z] = (byte)type;
    }

    static void GenerateTree(byte[] blocks, int x, int y, int z)
    {
        for (int k = y + 3; k <= y + 5; k++)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = z - 1; j <= z + 1; j++)
                {
                    SetBlockType(blocks, i, k, j, CSBlockType.OakLeaves);
                }
            }
        }
        for (int i = 0; i < 4; i++)
        {
            SetBlockType(blocks, x, y + i, z, CSBlockType.OakWood);
        }
    }
}