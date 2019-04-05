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

    // raycast的结果有精度问题，所以要加上精度补偿（每个轴的正负两个方向都试着取一下，最坏需要尝试2^3=8种情况）
    Vector3Int GetBlockPosByRaycast(Vector3 point)
    {
        float precisionCompensation = 0.01f;
        Vector3Int pos = new Vector3Int();

        pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
        if (test.ContainBlock(pos))
            return pos;
        else
        {
            pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
            if (test.ContainBlock(pos))
                return pos;
            else
            {
                pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
                if (test.ContainBlock(pos))
                    return pos;
                else
                {
                    pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
                    if (test.ContainBlock(pos))
                        return pos;
                    else
                    {
                        pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
                        if (test.ContainBlock(pos))
                            return pos;
                        else
                        {
                            pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
                            if (test.ContainBlock(pos))
                                return pos;
                            else
                            {
                                pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
                                if (test.ContainBlock(pos))
                                    return pos;
                                else
                                {
                                    pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
                                    if (test.ContainBlock(pos))
                                        return pos;
                                }
                            }
                        }
                    }
                }
            }
        }
        return Vector3Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        RotateView();
        DrawWireFrame();

        if (Input.GetMouseButtonDown(0))
        {
            if (WireFrameHelper.render)
            {
                test.RemoveBlock(WireFrameHelper.pos);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void DrawWireFrame()
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        int cubeLayerIndex = LayerMask.NameToLayer("Block");
        if (cubeLayerIndex != -1)
        {
            int layerMask = (1 << cubeLayerIndex);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(center), out RaycastHit hit, 5f, layerMask))
            {
                Vector3Int pos = GetBlockPosByRaycast(hit.point);
                //Debug.Log(hit.point + "," + pos);

                WireFrameHelper.render = true;
                WireFrameHelper.pos = pos;
            }
            else
            {
                WireFrameHelper.render = false;
            }
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
