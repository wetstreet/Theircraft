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
    static Dictionary<Vector2Int, GameObject> blockmap = new Dictionary<Vector2Int, GameObject>();

    static LinkedList<Vector2Int> linkedList = new LinkedList<Vector2Int>();

    static int scale = 35;
    static int maxHeight = 15;

    private void Start()
    {
        StartCoroutine(GenerateLoop());
    }

    public void GenerateBlock(Vector3 pos)
    {
        Vector2Int chunk = Ultiities.GetChunk(pos);
        GameObject obj = Instantiate(prefab) as GameObject;
        if (!blockmap.ContainsKey(chunk))
        {
            Transform chunkParent = GenerateChunkParent(chunk);
            blockmap[chunk] = chunkParent.gameObject;
        }
        obj.transform.parent = blockmap[chunk].transform;
        obj.transform.localPosition = pos;
    }

    public void GenerateTnt(Vector3 pos)
    {
        Vector2Int chunk = Ultiities.GetChunk(pos);
        GameObject obj = BlockGenerator.CreateCube();
        if (!blockmap.ContainsKey(chunk))
        {
            Transform chunkParent = GenerateChunkParent(chunk);
            blockmap[chunk] = chunkParent.gameObject;
        }
        obj.transform.parent = blockmap[chunk].transform;
        obj.transform.localPosition = pos;
    }

    public void ShowChunk(Vector2Int chunk, bool isSync)
    {
        if (blockmap.ContainsKey(chunk))
        {
            blockmap[chunk].SetActive(true);
            return;
        }
        
        if (isSync)
            blockmap[chunk] = GenerateChunk(chunk);
        else
        {
            lock (linkedList)
            {
                linkedList.AddLast(chunk);
            }
        }
    }

    public bool HideChunk(Vector2Int chunk)
    {
        if (!blockmap.ContainsKey(chunk))
        {
            lock (linkedList)
            {
                if (linkedList.Contains(chunk))
                {
                    linkedList.Remove(chunk);
                    return true;
                }
            }
            return false;
        }

        GameObject obj = blockmap[chunk];
        obj.SetActive(false);
        return true;
    }

    Transform GenerateChunkParent(Vector2Int chunk)
    {
        Transform chunkParent = new GameObject(string.Format("chunk({0},{1})", chunk.x, chunk.y)).transform;
        chunkParent.parent = transform;
        chunkParent.localPosition = Vector3.zero;
        return chunkParent;
    }

    public GameObject GenerateChunk(Vector2Int chunk)
    {
        Transform chunkParent = GenerateChunkParent(chunk);
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
                obj.transform.name = string.Format("block({0},{1})", i + chunk.x * 16, j + chunk.y * 16);
            }
        }
        return chunkParent.gameObject;
    }
    IEnumerator GenerateLoop()
    {
        while (true)
        {
            lock (linkedList)
            {
                if (linkedList.Count > 0)
                {
                    Vector2Int chunk = linkedList.First.Value;
                    linkedList.RemoveFirst();
                    blockmap[chunk] = GenerateChunk(chunk);
                }
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

    public static bool HideChunk(Vector2Int chunk)
    {
        return gc.HideChunk(chunk);
    }

    public static void HideChunks(List<Vector2Int> list)
    {
        foreach (Vector2Int chunk in list)
        {
            HideChunk(chunk);
        }
    }

    public static void ShowChunk(Vector2Int chunk, bool isSync = false)
    {
        gc.ShowChunk(chunk, isSync);
    }

    public static void ShowChunks(List<Vector2Int> list, bool isSync = false)
    {
        foreach (Vector2Int chunk in list)
        {
            ShowChunk(chunk, isSync);
        }
    }

    public static void GenerateBlock(Vector3 pos)
    {
        gc.GenerateBlock(pos);
    }

    public static void GenerateTnt(Vector3 pos)
    {
        gc.GenerateTnt(pos);
    }
}
