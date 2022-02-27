using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxMover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 offset = new Vector3(0, 16, 0);
    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerController.instance.position + offset;
    }
}
