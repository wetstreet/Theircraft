using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public static class TerrainGenerator {
    private class GenerateCoroutine : MonoBehaviour
    {
        private static GenerateCoroutine instance;
        public static GenerateCoroutine Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("GenerateCoroutine");
                    instance = obj.AddComponent<GenerateCoroutine>();
                }
                return instance;
            }
        }
        int scale = 35;
        int maxHeight = 15;

        IEnumerator GenerateEnumerator()
        {
            UnityEngine.Object prefab = Resources.Load("Cube");
            while (true)
            {
                if (LoadQueue.Count > 0)
                {
                    Vector2 chunk = LoadQueue.Dequeue();
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
                    if (finishCallback != null)
                    {
                        finishCallback();
                        finishCallback = null;
                    }
                }
                yield return null;
            }
        }

        public void Generate()
        {
            StartCoroutine(GenerateEnumerator());
        }
    }

    private static Queue<Vector2> LoadQueue = new Queue<Vector2>();

    static Action finishCallback;

    public static void GenerateTerrain(Action _finishCallback = null)
    {
        finishCallback = _finishCallback;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                LoadQueue.Enqueue(new Vector2(i, j));
            }
        }
        GenerateCoroutine.Instance.Generate();
    }
}
