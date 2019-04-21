using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mergetestPlayerController : MonoBehaviour
{
    public float horizontalScale = 1;
    public float verticalScale = 1;

    private Vector3 verticalSpeed;
    private Vector3 horizontalSpeed;
    private new Camera camera;
    private CharacterController cc;
    static private Camera handCam;
    Animator handAnimator;

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

    public static void SetHandRT(RenderTexture rt)
    {
        handCam.targetTexture = rt;
    }

    public static void Init()
    {
        if (instance == null)
        {
            Object prefab = Resources.Load("merge-test/Character");
            instance = Instantiate(prefab) as GameObject;
        }
    }

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cc = GetComponent<CharacterController>();
        handCam = camera.transform.Find("Camera").GetComponent<Camera>();
        handAnimator = camera.transform.Find("hand").GetComponent<Animator>();

        transform.position = DataCenter.spawnPosition;
        transform.localEulerAngles = new Vector3(0, DataCenter.spawnRotation.y, 0);
        camera.transform.localEulerAngles = new Vector3(DataCenter.spawnRotation.z, 0, 0);

        NetworkManager.Register(ENUM_CMD.CS_ADD_BLOCK_RES, AddBlockRes);
        NetworkManager.Register(ENUM_CMD.CS_ADD_BLOCK_NOTIFY, OnAddBlockNotify);
        NetworkManager.Register(ENUM_CMD.CS_DELETE_BLOCK_RES, DeleteBlockRes);
        NetworkManager.Register(ENUM_CMD.CS_DELETE_BLOCK_NOTIFY, OnDeleteBlockNotify);

        LoadingUI.Close();
        CrossHair.Show();
        Hand.Show();
    }

    float timeInterval = 0.2f;
    bool needUpdate;
    float lastUpdateTime;
    void Update()
    {
        DrawWireFrame();

        if (acceptInput)
        {
            RotateView();

            if (Input.GetMouseButtonDown(0))
            {
                handAnimator.SetTrigger("interactTrigger");
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

        if (needUpdate && Time.realtimeSinceStartup - lastUpdateTime > timeInterval)
        {
            needUpdate = false;
            lastUpdateTime = Time.realtimeSinceStartup;
            CSHeroMoveReq req = new CSHeroMoveReq
            {
                Position = new CSVector3 { x = transform.position.x, y = transform.position.y, z = transform.position.z },
                Rotation = new CSVector3 { x = 0, y = transform.localEulerAngles.y, z = camera.transform.localEulerAngles.x }
            };
            NetworkManager.Enqueue(ENUM_CMD.CS_HERO_MOVE_REQ, req);
        }
    }

    RaycastHit hit;
    void DrawWireFrame()
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        int cubeLayerIndex = LayerMask.NameToLayer("Block");
        int otherPlayerLayerIndex = LayerMask.NameToLayer("OtherPlayer");
        if (cubeLayerIndex != -1 && otherPlayerLayerIndex != -1)
        {
            int layerMask = 1 << cubeLayerIndex | 1 << otherPlayerLayerIndex;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(center), out hit, 5f, layerMask) && hit.transform.tag == "Block")
            {
                Vector3Int actualPos = WireFrameHelper.GetBlockPosByRaycast(hit.point);
                //Debug.Log(hit.point + "," + pos);
                WireFrameHelper.render = true;
                WireFrameHelper.pos = actualPos;
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
    
    public float inAirSpeed = 0.1f;
    public float attenuation = 0.75f;
    void ProcessMovement(float v, float h)
    {
        if (cc.isGrounded)
        {
            if (v != 0 || h != 0)
            {
                horizontalSpeed = horizontalSpeed + (transform.forward * v + transform.right * h);
            }
            else
            {
                horizontalSpeed *= attenuation;
            }
        }
        else
        {
            horizontalSpeed = horizontalSpeed + (transform.forward * v + transform.right * h) * inAirSpeed;
        }
        horizontalSpeed = Vector3.ClampMagnitude(horizontalSpeed, 1);
        Vector3 horizontalMovement = horizontalSpeed * Time.fixedDeltaTime;

        verticalSpeed += Physics.gravity * Time.fixedDeltaTime;
        Vector3 verticalMovement = verticalSpeed * Time.fixedDeltaTime;

        //有移动则告诉服务器
        float precision = 1f;
        //妈蛋，明明verticalMovement打印出来是(0,0,0)却!=Vector3.zero，有精度问题，这样搞下
        bool b1 = Mathf.Abs(verticalMovement.x) > precision || Mathf.Abs(verticalMovement.y) > precision || Mathf.Abs(verticalMovement.z) > precision;
        bool b2 = horizontalMovement != Vector3.zero;
        if (b1 || b2)
            needUpdate = true;

        if (horizontalMovement != Vector3.zero)
        {
            if (!isMoving)
            {
                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
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
        if (x != 0 || y != 0)
        {
            camera.transform.localRotation *= Quaternion.Euler(-y, 0, 0);
            transform.localRotation *= Quaternion.Euler(0, x, 0);
            needUpdate = true;
        }
    }

    float v;
    float h;
    public float accumulation = 0.1f;
    void FixedUpdate()
    {
        if (acceptInput)
        {
            float rawV = Input.GetAxisRaw("Vertical");
            v = rawV != 0 ? v + rawV * accumulation : 0;
            v = Mathf.Clamp(v, -1, 1);

            float rawH = Input.GetAxisRaw("Horizontal");
            h = rawH != 0 ? h + rawH * accumulation : 0;
            h = Mathf.Clamp(h, -1, 1);
        }
        else
        {
            v = 0;
            h = 0;
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
            Vector3Int pos = new Vector3Int(rsp.position.x, rsp.position.y, rsp.position.z);
            CSBlockType type = test.GetBlockAtPos(pos).type;
            test.RemoveBlock(pos);
            BreakBlockEffect.Create(type, pos);
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
