using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform character;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {

	}

    void ProcessMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = character.transform.forward * v;
        character.transform.localPosition += transform.forward * v + transform.right * h;
    }

    void RotateView()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        transform.localRotation *= Quaternion.Euler(-y, 0, 0);
        character.transform.localRotation *= Quaternion.Euler(0, x, 0);
    }

    void FixedUpdate()
    {
        ProcessMovement();
        RotateView();
    }

    int enterCount;
    private void OnTriggerEnter(Collider other)
    {
        enterCount++;
        BlackScreen.Show();
    }

    private void OnTriggerExit(Collider other)
    {
        enterCount--;
        if (enterCount <= 0)
            BlackScreen.Hide();
    }
}
