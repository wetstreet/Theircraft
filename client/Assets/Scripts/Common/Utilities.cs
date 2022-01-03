using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public struct Tag
{
    public static string Player = "Player";
}

public struct Layer
{
    public static string Chunk = "Chunk";
    public static string ItemTrigger = "ItemTrigger";
}

public static class Utilities
{
    public static T AddMissingComponent<T>(this GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    public static List<Vector2Int> GetSurroudingChunks(Vector2Int chunk, int chunkRange)
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

    public static List<Vector2Int> GetSurroudingChunks(Vector2Int chunk)
    {
        return GetSurroudingChunks(chunk, SettingsPanel.RenderDistance);
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
            SoundManager.Play2DSound("UI_Click");
            call();
        });
    }

    public class Vector3IntHelper
    {
        public Vector3Int up;
        public Vector3Int down;
        public Vector3Int left;
        public Vector3Int right;
        public Vector3Int forward;
        public Vector3Int back;
    }

    static Vector3IntHelper _Vector3Int;
    public static Vector3IntHelper vector3Int
    {
        get
        {
            if (_Vector3Int == null)
            {
                _Vector3Int = new Vector3IntHelper
                {
                    up = Vector3Int.up,
                    down = Vector3Int.down,
                    left = Vector3Int.left,
                    right = Vector3Int.right,
                    forward = new Vector3Int(0, 0, 1),
                    back = new Vector3Int(0, 0, -1),
                };
            }
            return _Vector3Int;
        }
    }

    public static string screenshotDir
    {
        get
        {
            return Application.persistentDataPath + "/screenshots/";
        }
    }

    public static void Capture()
    {
        DateTime time = DateTime.Now;
        string file = string.Format("{0}-{1:00}-{2:00}_{3:00}.{4:00}.{5:00}.png", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

        string path = screenshotDir + file;
        if (!System.IO.Directory.Exists(screenshotDir))
        {
            System.IO.Directory.CreateDirectory(screenshotDir);
        }
        ScreenCapture.CaptureScreenshot(path);
        
        string log = "Saved screenshot as <u>" + file + "</u>";
        ChatPanel.AddLine(log);
    }
}
