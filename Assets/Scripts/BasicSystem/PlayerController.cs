using UnityEngine;
using Theircraft;

public class PlayerController : MonoBehaviour {
    
    public float horizontalScale;
    public float verticalScale;

    private Vector3 verticalSpeed;
    
    private bool isMoving;
    private new Camera camera;
    private CharacterController cc;
    
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

        NetworkManager.Register(CSMessageType.ADD_BLOCK_RES, AddBlockRes);
        NetworkManager.Register(CSMessageType.ADD_BLOCK_NOTIFY, OnAddBlockNotify);
        NetworkManager.Register(CSMessageType.DELETE_BLOCK_RES, DeleteBlockRes);
        NetworkManager.Register(CSMessageType.DELETE_BLOCK_NOTIFY, OnDeleteBlockNotify);
    }

    bool GetPointingBlockInfo(out RaycastHit rh)
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        RaycastHit hit;
        bool b = Physics.Raycast(Camera.main.ScreenPointToRay(center), out hit, 5f);
        if (b && hit.transform.tag == "Block")
        {
            rh = hit;
            return true;
        }
        else
        {
            rh = default(RaycastHit);
            return false;
        }
    }

    void OnLeftMouseClick()
    {
        RaycastHit hit;
        bool b = GetPointingBlockInfo(out hit);
        if (b)
            DeleteBlockReq(hit.transform.localPosition);
    }

    static Vector3Int forward = new Vector3Int(0, 0, 1);
    static Vector3Int back = new Vector3Int(0, 0, -1);
    void OnRightMouseClick()
    {
        if (ItemSelectPanel.curBlockType != CSBlockType.None)
        {
            RaycastHit hit;
            bool b = GetPointingBlockInfo(out hit);
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

    //Material lastMaterial;
    void ShowWireFrame()
    {
        RaycastHit hit;
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        bool b = Physics.Raycast(Camera.main.ScreenPointToRay(center), out hit, 5f);
        if (b && hit.transform.tag == "Block")
        {
            WireFrameHelper.render = true;
            WireFrameHelper.pos = hit.transform.localPosition;
            //Material material = hit.transform.GetComponent<Renderer>().material;
            //if (lastMaterial != null && lastMaterial != material)
            //    lastMaterial.SetFloat("_Lightness", 1);
            //material.SetFloat("_Lightness", 1.5f);
            //lastMaterial = material;
        }
        else
        {
            WireFrameHelper.render = false;
            //if (lastMaterial != null)
            //    lastMaterial.SetFloat("_Lightness", 1);
            //lastMaterial = null;
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
        CSBlock b = new CSBlock();
        b.position = new CSVector3Int();
        b.position.x = pos.x;
        b.position.y = pos.y;
        b.position.z = pos.z;
        b.type = type;
        CSAddBlockReq addBlockReq = new CSAddBlockReq
        {
            block = b
        };
        NetworkManager.Enqueue(CSMessageType.ADD_BLOCK_REQ, addBlockReq);
    }

    void AddBlockRes(byte[] data)
    {
        CSAddBlockRes rsp = NetworkManager.Deserialzie<CSAddBlockRes>(data);
        //Debug.Log("AddBlockRes,retCode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            CSVector3Int p = rsp.block.position;
            TerrainGenerator.GenerateBlock(new Vector3Int(p.x, p.y, p.z), rsp.block.type);
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
        CSAddBlockNotify notify = NetworkManager.Deserialzie<CSAddBlockNotify>(data);
        CSVector3Int p = notify.block.position;
        TerrainGenerator.GenerateBlock(new Vector3Int(p.x, p.y, p.z), notify.block.type);
    }

    void DeleteBlockReq(Vector3 pos)
    {
        CSVector3Int position = new CSVector3Int();
        position.x = Mathf.RoundToInt(pos.x);
        position.y = Mathf.RoundToInt(pos.y);
        position.z = Mathf.RoundToInt(pos.z);
        CSDeleteBlockReq req = new CSDeleteBlockReq();
        req.position = position;
        NetworkManager.Enqueue(CSMessageType.DELETE_BLOCK_REQ, req);
    }

    void DeleteBlockRes(byte[] data)
    {
        CSDeleteBlockRes rsp = NetworkManager.Deserialzie<CSDeleteBlockRes>(data);
        //Debug.Log("DeleteBlockRes,retCode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            CSVector3Int pos = rsp.position;
            string name = string.Format("block({0},{1},{2})", pos.x, pos.y, pos.z);
            GameObject obj = GameObject.Find(name);
            Destroy(obj);
            DestroySystem.AsyncDestroyBlock(pos);
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }

    void OnDeleteBlockNotify(byte[] data)
    {
        //Debug.Log("OnDeleteBlockNotify");
        CSDeleteBlockNotify notify = NetworkManager.Deserialzie<CSDeleteBlockNotify>(data);
        CSVector3Int pos = notify.position;
        string name = string.Format("block({0},{1},{2})", pos.x, pos.y, pos.z);
        GameObject obj = GameObject.Find(name);
        Destroy(obj);
        DestroySystem.AsyncDestroyBlock(pos);
    }
}
