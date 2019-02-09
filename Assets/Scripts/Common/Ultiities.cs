using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public static class Ultiities {

    static int chunkRange = 1;

    public static Vector2Int GetChunk(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / 16);
        int chunkZ = Mathf.FloorToInt(position.z / 16);
        return new Vector2Int(chunkX, chunkZ);
    }

    public static List<Vector2Int> GetSurroudingChunks(Vector2Int chunk)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = chunk.x - chunkRange; i <= chunk.x + chunkRange; i++)
        {
            for (int j = chunk.y - chunkRange; j <= chunk.y + chunkRange; j++)
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

    public static Vector2Int CSVector2Int_To_Vector2Int(CSVector2Int v)
    {
        return new Vector2Int { x = v.x, y = v.y };
    }

    public static Vector3Int CSVector3Int_To_Vector3Int(CSVector3Int v)
    {
        return new Vector3Int { x = v.x, y = v.y, z = v.z };
    }

    public static Vector3 CSVector3_To_Vector3(CSVector3 v)
    {
        return new Vector3 { x = v.x, y = v.y, z = v.z };
    }
}
