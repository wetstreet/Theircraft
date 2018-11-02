using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public float HorizontalSpeed;
    public float VerticalSpeed;
    
    private bool isMoving;
    private Rigidbody rigidbody;
    private Camera camera;

    bool inTheAir { get { return Mathf.Abs(rigidbody.velocity.y) > 0.3f; } }

    private static GameObject instance;

    public static GameObject Instance
    {
        get
        {
            if (instance == null)
            {
                Object prefab = Resources.Load("Character");
                instance = Instantiate(prefab) as GameObject;
            }
            return instance;
        }
    }

    public static void Init()
    {
        if (instance == null)
        {
            Object prefab = Resources.Load("Character");
            instance = Instantiate(prefab) as GameObject;
            instance.transform.localPosition = new Vector3(0, 10, 0);
        }
    }

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        rigidbody = GetComponent<Rigidbody>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update ()
    {
        RotateView();
        ChangeFieldOfView();
    }
    
    void Jump()
    {
        if (inTheAir)
        {
            print("cannot jump now");
            return;
        }

        //rigidbody.velocity = new Vector3(0, 7f, 0);
        rigidbody.AddForce(Vector3.up * VerticalSpeed);
    }

    float verticalSpeed = 0;
    void ProcessMovement()
    {
        //print(rigidbody.velocity);
        
        Vector3 verticalMovement = new Vector3(0, -1, 0) * Time.fixedDeltaTime;
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        Vector3 horizontalMovement = transform.forward * v + transform.right * h;
        rigidbody.MovePosition(transform.localPosition + horizontalMovement * HorizontalSpeed * Time.fixedDeltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void ChangeFieldOfView()
    {
        //float fieldOfView = camera.fieldOfView + (isMoving ? -1 : 1);
        //camera.fieldOfView = Mathf.Clamp(fieldOfView, 57, 60);

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
        Debug.DrawLine(Vector3.zero, new Vector3(0, 5, 0));
    }
}
