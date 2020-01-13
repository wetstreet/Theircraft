using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public float speed = 0.01f;
    void Update()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y - speed, 0);
    }
}
