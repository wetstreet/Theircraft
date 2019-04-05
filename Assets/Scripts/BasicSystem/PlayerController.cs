using UnityEngine;
using protocol.cs_enum;
using protocol.cs_theircraft;

public class PlayerController : MonoBehaviour {
    
    public float horizontalScale;
    public float verticalScale;

    private Vector3 verticalSpeed;
    private new Camera camera;
    private CharacterController cc;
    private Transform head;
    private Animator animator;

    bool isMoving;
    static bool acceptInput = true;
    public static bool isInitialized = false;
    
    public static GameObject Instance;

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
        if (Instance == null)
        {
            Object prefab = Resources.Load("Prefabs/Character");
            Instance = Instantiate(prefab) as GameObject;
            Instance.transform.localPosition = new Vector3(0, 10, 0);
            isInitialized = true;
        }
        LoadingUI.Close();
        CrossHair.Show();
    }

    // Use this for initialization
    void Start () {
        transform.localPosition = DataCenter.initialPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    bool GetPointingBlockInfo(out RaycastHit rh)
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        bool b = Physics.Raycast(Camera.main.ScreenPointToRay(center), out RaycastHit hit, 5f);
        if (b && hit.transform.tag == "Block")
        {
            rh = hit;
            return true;
        }
        else
        {
            rh = default;
            return false;
        }
    }

    void OnLeftMouseClick()
    {
        bool b = GetPointingBlockInfo(out RaycastHit hit);
        if (b)
            DeleteBlockReq(hit.transform.localPosition);
    }

    static Vector3Int forward = new Vector3Int(0, 0, 1);
    static Vector3Int back = new Vector3Int(0, 0, -1);
    void OnRightMouseClick()
    {
        if (ItemSelectPanel.curBlockType != CSBlockType.None)
        {
            bool b = GetPointingBlockInfo(out RaycastHit hit);
            if (b)
            {
                if (hit.normal == Vector3.right)
                    AddBlockReq(Vector3Int.FloorToInt(hit.transform.localPosition) + Vector3Int.right, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.left)
                    AddBlockReq(Vector3Int.FloorToInt(hit.transform.localPosition) + Vector3Int.left, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.forward)
                    AddBlockReq(Vector3Int.FloorToInt(hit.transform.localPosition) + forward, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.back)
                    AddBlockReq(Vector3Int.FloorToInt(hit.transform.localPosition) + back, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.up)
                    AddBlockReq(Vector3Int.FloorToInt(hit.transform.localPosition) + Vector3Int.up, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.down)
                    AddBlockReq(Vector3Int.FloorToInt(hit.transform.localPosition) + Vector3Int.down, ItemSelectPanel.curBlockType);
            }
        }
    }

    void ProcessKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnLeftMouseClick();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnRightMouseClick();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (acceptInput)
        {
            RotateView();
            ProcessKeyBoard();
        }
        ChangeFieldOfView();
        ShowWireFrame();
    }
    
    void ShowWireFrame()
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        bool b = Physics.Raycast(Camera.main.ScreenPointToRay(center), out RaycastHit hit, 5f);
        if (b && hit.transform.tag == "Block")
        {
            WireFrameHelper.render = true;
            WireFrameHelper.pos = hit.transform.localPosition;
        }
        else
        {
            WireFrameHelper.render = false;
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
        Vector3 horizontalMovement = transform.forward * v + transform.right * h;
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
        head.transform.localRotation *= Quaternion.Euler(0, 0, -y);
    }

    void FixedUpdate()
    {
        float v = 0;
        float h = 0;
        if (acceptInput)
        {
            v = Input.GetAxisRaw("Vertical");
            h = Input.GetAxisRaw("Horizontal");
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
            TerrainGenerator.SetBlockData(rsp.block);
            TerrainGenerator.RefreshNearbyBlocks(Ultiities.CSVector3Int_To_Vector3Int(rsp.block.position));
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
        TerrainGenerator.SetBlockData(notify.block);
        TerrainGenerator.RefreshNearbyBlocks(Ultiities.CSVector3Int_To_Vector3Int(notify.block.position));
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
            TerrainGenerator.DestroyBlock(Ultiities.CSVector3Int_To_Vector3Int(rsp.position));
            TerrainGenerator.RefreshNearbyBlocks(Ultiities.CSVector3Int_To_Vector3Int(rsp.position));
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
        TerrainGenerator.DestroyBlock(Ultiities.CSVector3Int_To_Vector3Int(notify.position));
        TerrainGenerator.RefreshNearbyBlocks(Ultiities.CSVector3Int_To_Vector3Int(notify.position));
    }
}
