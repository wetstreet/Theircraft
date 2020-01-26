using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    static readonly float scale = 35;
    static readonly int maxHeight = 15;

    public static byte[] GenerateChunkData(CSVector2Int chunk, byte[] blocks)
    {
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
                                else if (dice <= 199 && dice > 197)
                                {
                                    int treedice = Random.Range(1, 10);
                                    if (treedice == 1)
                                    {
                                        GenerateTree(blocks, i, k, j, chunk.ToVector2Int());
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
                    if (type != CSBlockType.None)
                    {
                        blocks[256 * k + 16 * i + j] = (byte)type;
                    }
                }
            }
        }
        return blocks;
    }

    static void GenerateTree(byte[] blocks, int x, int y, int z, Vector2Int chunk)
    {
        for (int k = y + 2; k <= y + 4; k++)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = z - 1; j <= z + 1; j++)
                {
                    LocalServer.SetBlockType(i + chunk.x * 16, k, j + chunk.y * 16, CSBlockType.OakLeaves);
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            LocalServer.SetBlockType(x + chunk.x * 16, y + i, z + chunk.y * 16, CSBlockType.OakWood);
        }
    }
}
