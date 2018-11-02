using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour {

    void OnGenerateFinish()
    {
        print("generate finish");
        PlayerController.Init();
    }

	// Use this for initialization
	void Start () {
        TerrainGenerator.GenerateTerrain(OnGenerateFinish);
        //float scale = 100f;
        //int maxHeight = 15;
        //for (int i = 1; i <= 100; i++)
        //{
        //    for (int j = 1; j <= 100; j++)
        //    {
        //        float x = i / scale;
        //        float y = j / scale;
        //        float noise = Mathf.PerlinNoise(x, y);
        //        int height = Mathf.RoundToInt(maxHeight * noise);
        //        print("noise=" + noise + ",height="+ height);
        //    }
        //}
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
