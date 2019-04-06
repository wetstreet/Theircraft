using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mergetestPlayerController : MonoBehaviour
{
    public float horizontalScale = 0.1f;
    public float verticalScale = 1;

    private Vector3 verticalSpeed;
    private Vector3 horizontalSpeed;
    private new Camera camera;
    private CharacterController cc;
    private Transform head;
    private Animator animator;

    bool isMoving;
    static bool acceptInput = true;
    private static GameObject instance;

    public static void LockCursor(bool isLock)
    {
        if (isLock)
        {
            acceptInput = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            acceptInput = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public static void Init()
    {
        if (instance == null)
        {
            Object prefab = Resources.Load("merge-test/Character");
            instance = Instantiate(prefab) as GameObject;
            instance.transform.position = DataCenter.spawnPosition;
            Debug.Log(DataCenter.spawnPosition);
            //instance.transform.eulerAngles = DataCenter.spawnRotation;
        }
        LoadingUI.Close();
        CrossHair.Show();
    }

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cc = GetComponent<CharacterController>();
        head = transform.Find("steve/Armature/Move/Body_Lower/Body_Upper/Head.001");
        Transform steve = transform.Find("steve");
        animator = steve.GetComponent<Animator>();

        NetworkManager.Register(ENUM_CMD.CS_ADD_BLOCK_RES, AddBlockRes);
        NetworkManager.Register(ENUM_CMD.CS_ADD_BLOCK_NOTIFY, OnAddBlockNotify);
        NetworkManager.Register(ENUM_CMD.CS_DELETE_BLOCK_RES, DeleteBlockRes);
        NetworkManager.Register(ENUM_CMD.CS_DELETE_BLOCK_NOTIFY, OnDeleteBlockNotify);
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
        throw new System.Exception("what the fuck?");
    }

    // Update is called once per frame
    void Update()
    {
        if (acceptInput)
        {
            RotateView();
        }
        DrawWireFrame();

        if (Input.GetMouseButtonDown(0))
        {
            if (WireFrameHelper.render)
            {
                DeleteBlockReq(WireFrameHelper.pos);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (WireFrameHelper.render)
            {
                AddBlockReq(Vector3Int.RoundToInt(WireFrameHelper.pos + hit.normal), ItemSelectPanel.curBlockType);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    RaycastHit hit;
    void DrawWireFrame()
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        int cubeLayerIndex = LayerMask.NameToLayer("Block");
        if (cubeLayerIndex != -1)
        {
            int layerMask = (1 << cubeLayerIndex);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(center), out hit, 5f, layerMask))
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

    void ProcessMovement(float v, float h)
    {
        verticalSpeed += Physics.gravity * Time.fixedDeltaTime;
        Vector3 verticalMovement = verticalSpeed * Time.fixedDeltaTime;
        Vector3 horizontalMovement = forward * v + right * h;
        if (horizontalMovement != Vector3.zero)
        {
            if (!isMoving)
            {
                isMoving = true;
                animator.CrossFade("walk", 0);
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                animator.CrossFade("idle", 0);
            }
        }
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
        head.transform.localRotation *= Quaternion.Euler(0, 0, -y);
    }

    float v;
    float h;
    Vector3 forward;
    Vector3 right;
    void FixedUpdate()
    {
        if (acceptInput && cc.isGrounded)
        {
            v = Input.GetAxisRaw("Vertical");
            h = Input.GetAxisRaw("Horizontal");
            forward = transform.forward;
            right = transform.right;
        }
        ProcessMovement(v, h);
    }

    void AddBlockReq(Vector3Int pos, CSBlockType type)
    {
        CSAddBlockReq addBlockReq = new CSAddBlockReq
        {
            block = new CSBlock
            {
                position = new CSVector3Int
                {
                    x = pos.x,
                    y = pos.y,
                    z = pos.z
                },
                type = type
            }
        };
        NetworkManager.Enqueue(ENUM_CMD.CS_ADD_BLOCK_REQ, addBlockReq);
    }

    void AddBlockRes(byte[] data)
    {
        CSAddBlockRes rsp = NetworkManager.Deserialize<CSAddBlockRes>(data);
        //Debug.Log("AddBlockRes,retCode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            Vector3Int blockPos = new Vector3Int(rsp.block.position.x, rsp.block.position.y, rsp.block.position.z);
            test.AddBlock(blockPos, rsp.block.type);
            FastTips.Show("放置了一个方块");
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }

    void OnAddBlockNotify(byte[] data)
    {
        //Debug.Log("OnAddBlockNotify");
        CSAddBlockNotify notify = NetworkManager.Deserialize<CSAddBlockNotify>(data);
        Vector3Int blockPos = new Vector3Int(notify.block.position.x, notify.block.position.y, notify.block.position.z);
        test.AddBlock(blockPos, notify.block.type);
    }

    void DeleteBlockReq(Vector3 pos)
    {
        CSDeleteBlockReq req = new CSDeleteBlockReq
        {
            position = new CSVector3Int
            {
                x = Mathf.RoundToInt(pos.x),
                y = Mathf.RoundToInt(pos.y),
                z = Mathf.RoundToInt(pos.z)
            }
        };
        NetworkManager.Enqueue(ENUM_CMD.CS_DELETE_BLOCK_REQ, req);
    }

    void DeleteBlockRes(byte[] data)
    {
        CSDeleteBlockRes rsp = NetworkManager.Deserialize<CSDeleteBlockRes>(data);
        //Debug.Log("DeleteBlockRes,retCode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            Vector3Int blockPos = new Vector3Int(rsp.position.x, rsp.position.y, rsp.position.z);
            test.RemoveBlock(blockPos);
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }

    void OnDeleteBlockNotify(byte[] data)
    {
        //Debug.Log("OnDeleteBlockNotify");
        CSDeleteBlockNotify notify = NetworkManager.Deserialize<CSDeleteBlockNotify>(data);
        Vector3Int blockPos = new Vector3Int(notify.position.x, notify.position.y, notify.position.z);
        test.RemoveBlock(blockPos);
    }
}
