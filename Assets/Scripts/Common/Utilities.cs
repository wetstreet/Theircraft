using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;
using UnityEngine.UI;

public static class Utilities {

    static int chunkRange = 2;

    public static Vector2Int GetChunk(Vector3 position)
    {
        int chunkX = Mathf.FloorToInt(position.x / 16f);
        int chunkZ = Mathf.FloorToInt(position.z / 16f);
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

    public static List<Vector2Int> GetNearbyChunks(Vector2Int chunk, int sight)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        int start;
        int max;
        if (sight % 2 == 0)
        {
            int half = sight / 2;
            start = -half;
            max = half - 1;
        }
        else
        {
            int half = Mathf.FloorToInt(sight / 2f);
            start = -half;
            max = half;
        }

        for (int i = chunk.x + start; i <= chunk.x + max; i++)
        {
            for (int j = chunk.y + start; j <= chunk.y + max; j++)
            {
                ret.Add(new Vector2Int(i, j));
            }
        }

        return ret;
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

    public static void SetClickCallback(Transform trans, string path, UnityEngine.Events.UnityAction call)
    {
        Button button = trans.Find(path).GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            SoundManager.PlayClickSound();
            call();
        });
    }
}
