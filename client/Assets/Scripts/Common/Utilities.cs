using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;
using UnityEngine.UI;

public static class Utilities {

    public static Vector3 ToVector3(this CSVector3 csv)
    {
        return new Vector3(csv.x, csv.y, csv.z);
    }

    public static Vector3Int ToVector3Int(this CSVector3Int csv)
    {
        return new Vector3Int(csv.x, csv.y, csv.z);
    }

    public static void GetChunk(ref Vector2Int chunkPos, Vector3 position)
    {
        chunkPos.x = Mathf.FloorToInt(position.x / 16f);
        chunkPos.y = Mathf.FloorToInt(position.z / 16f);
    }

    public static List<Vector2Int> GetSurroudingChunks(Vector2Int chunk)
    {
        int chunkRange = SettingsPanel.RenderDistance;

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
