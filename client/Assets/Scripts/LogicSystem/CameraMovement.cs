using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 0.03f;
    void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y - speed, 0);
    }
}
