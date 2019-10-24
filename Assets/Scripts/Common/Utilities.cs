using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;
using UnityEngine.UI;

public static class Utilities {

    public static void GetChunk(ref Vector2Int chunkPos, Vector3 position)
    {
        chunkPos.x = Mathf.FloorToInt(position.x / 16f);
        chunkPos.y = Mathf.FloorToInt(position.z / 16f);
    }

    public static List<Vector2Int> GetSurroudingChunks(Vector2Int chunk)
    {
        int chunkRange = SettingsPanel.RenderDistance;
        chunkRange = chunkRange > 6 ? 6 : chunkRange;

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
