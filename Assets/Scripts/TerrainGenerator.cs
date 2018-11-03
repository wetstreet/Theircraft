using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region GenerateCoroutine, do not directly reference this class from outside this script.
class GenerateCoroutine : MonoBehaviour
{
    static private Object _prefab;
    static Object prefab
    {
        get
        {
            if (_prefab == null)
            {
                _prefab = Resources.Load("Cube");
            }
            return _prefab;
        }
    }

    private static Queue<Vector2Int> LoadQueue = new Queue<Vector2Int>();

    static int scale = 35;
    static int maxHeight = 15;

    private void Start()
    {
        StartCoroutine(GenerateLoop());
    }

    public void Enqueue(Vector2Int chunk)
    {
        LoadQueue.Enqueue(chunk);
    }

    public void GenerateChunk(Vector2Int chunk)
    {
        Transform chunkParent = new GameObject(string.Format("chunk({0},{1})", chunk.x, chunk.y)).transform;
        chunkParent.parent = transform;
        chunkParent.localPosition = Vector3.zero;
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                GameObject obj = Instantiate(prefab) as GameObject;
                obj.transform.parent = chunkParent;
                float x = (float)0.5 + i + chunk.x * 16;
                float z = (float)0.5 + j + chunk.y * 16;
                float noise = Mathf.PerlinNoise(x / scale, z / scale);
                int height = Mathf.RoundToInt(maxHeight * noise);
                obj.transform.localPosition = new Vector3(x, height, z);
            }
        }
    }
    IEnumerator GenerateLoop()
    {
        while (true)
        {
            if (LoadQueue.Count > 0)
            {
                Vector2Int chunk = LoadQueue.Dequeue();
                GenerateChunk(chunk);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
#endregion

public static class TerrainGenerator {
    
    static GenerateCoroutine gc;

    public static void Init()
    {
        if (gc != null)
            return;

        GameObject obj = new GameObject("GenerateCoroutine");
        gc = obj.AddComponent<GenerateCoroutine>();
    }

    public static void GenerateChunk(Vector2Int chunk)
    {
        gc.Enqueue(chunk);
    }

    public static void SyncGenerateChunk(Vector2Int chunk)
    {
        gc.GenerateChunk(chunk);
    }

    public static void SyncGenerateChunks(List<Vector2Int> list)
    {
        foreach (Vector2Int chunk in list)
        {
            SyncGenerateChunk(chunk);
        }
    }
}
