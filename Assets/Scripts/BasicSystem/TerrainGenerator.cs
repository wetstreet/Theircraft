using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //chunk对应的方块列表
    static Dictionary<CSVector2Int, List<CSBlock>> chunkMap = new Dictionary<CSVector2Int, List<CSBlock>>();
    
    static readonly float scale = 10;
    static readonly int maxHeight = 15;
    public static int GetHeight(int x, int z)
    {
        float noise = Mathf.PerlinNoise(x / scale, z / scale);
        int height = (int)Mathf.Round(maxHeight * noise);
        //Debug.Log("getheight,x=" + x + ",z=" + z + ",height=" + height);
        return height;
    }

    public static List<CSBlock> GetChunkBlocks(CSVector2Int chunk)
    {
        if (!chunkMap.ContainsKey(chunk))
        {
            List<CSBlock> blockList = new List<CSBlock>();
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    int x = i + chunk.x * 16;
                    int z = j + chunk.y * 16;
                    int maxHeight = GetHeight(i + chunk.x * 16, j + chunk.y * 16) + 20;
                    for (int k = maxHeight; k >= 0; k--)
                    {
                        CSBlock item = new CSBlock
                        {
                            position = new CSVector3Int()
                        };
                        item.position.x = x;
                        item.position.y = k;
                        item.position.z = z;
                        //Ultilities.Print($"i={i},j={j},height{item.position.y}");
                        if (k == maxHeight)
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
