using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour {

    void OnGenerateFinish()
    {
        PlayerController.Init();
    }

	// Use this for initialization
	void Start () {
        TerrainGenerator.GenerateTerrain(OnGenerateFinish);
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
