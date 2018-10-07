using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    IEnumerator Generate()
    {
        Object prefab = Resources.Load("Cube");
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject obj = Instantiate(prefab) as GameObject;
                obj.transform.parent = transform;
                float x = (float)0.5 * (i + 1);
                float z = (float)0.5 * (j + 1);
                obj.transform.localPosition = new Vector3(x, 0 , z);
            }

            yield return null;
        }
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(Generate());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
