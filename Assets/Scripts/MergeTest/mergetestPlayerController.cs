using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mergetestPlayerController : MonoBehaviour
{

    public float horizontalScale = 0.1f;
    public float verticalScale = 1;

    private Vector3 verticalSpeed;

    private bool isMoving;
    private new Camera camera;
    private CharacterController cc;

    private static GameObject instance;

    public static void Init()
    {
        if (instance == null)
        {
            Object prefab = Resources.Load("merge-test/Character");
            instance = Instantiate(prefab) as GameObject;
            instance.transform.position = new Vector3(0, 10, 0);
        }
    }

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateView();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (!cc.isGrounded)
        {
            return;
        }

        verticalSpeed = -Physics.gravity / 2;
    }

    void ProcessMovement()
    {
        verticalSpeed += Physics.gravity * Time.fixedDeltaTime;
        Vector3 verticalMovement = verticalSpeed * Time.fixedDeltaTime;
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        Vector3 horizontalMovement = transform.forward * v + transform.right * h;
        cc.Move(horizontalMovement * horizontalScale + verticalMovement * verticalScale);

        if (cc.isGrounded)
        {
            verticalSpeed = Vector3.zero;
        }
    }

    void RotateView()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        camera.transform.localRotation *= Quaternion.Euler(-y, 0, 0);
        transform.localRotation *= Quaternion.Euler(0, x, 0);
    }

    void FixedUpdate()
    {
        ProcessMovement();
    }
}
