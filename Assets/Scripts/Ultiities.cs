using System;
using System.Collections.Generic;
using UnityEngine;

public static class Ultiities {
    public static Vector2Int GetChunk(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / 16);
        int chunkZ = Mathf.FloorToInt(position.z / 16);
        return new Vector2Int(chunkX, chunkZ);
    }

    public static List<Vector2Int> GetSurroudingChunks(Vector2Int chunk)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = chunk.x - 1; i <= chunk.x + 1; i++)
        {
            for (int j = chunk.y - 1; j <= chunk.y + 1; j++)
            {
                list.Add(new Vector2Int(i, j));
            }
        }
        return list;
    }

    public static void PrintList<T>(List<T> list)
    {
        int count = 1;
        foreach (T t in list)
        {
            Debug.Log(count + "," + t);
            count++;
        }
    }
}
