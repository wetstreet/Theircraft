using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //chunk对应的方块列表
    static Dictionary<CSVector2Int, List<CSBlock>> chunkMap = new Dictionary<CSVector2Int, List<CSBlock>>();
    
    static readonly float scale = 35;
    static readonly int maxHeight = 15;

    public static List<CSBlock> GetChunkBlocks(CSVector2Int chunk)
    {
        if (!chunkMap.ContainsKey(chunk))
        {
            List<CSBlock> blockList = new List<CSBlock>();
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
                        CSBlock item = new CSBlock
                        {
                            position = new CSVector3Int()
                        };
                        item.position.x = i + chunk.x * 16;
                        item.position.y = k;
                        item.position.z = j + chunk.y * 16;
                        //Ultilities.Print($"i={i},j={j},height{item.position.y}");
                        if (k == height)
                            item.type = CSBlockType.Grass;
                        else
                            item.type = CSBlockType.Dirt;
                        blockList.Add(item);
                    }
                }
            }
            chunkMap[chunk] = blockList;
        }
        return chunkMap[chunk];
    }
}
