using UnityEngine;
using Theircraft;

public class PlayerController : MonoBehaviour {
    
    public float horizontalScale;
    public float verticalScale;

    private Vector3 verticalSpeed;
    
    private bool isMoving;
    private new Camera camera;
    private CharacterController cc;

    public static bool acceptInput = true;

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
        Cursor.visible = false;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cc = GetComponent<CharacterController>();
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
            Destroy(hit.transform.gameObject);
    }

    void OnRightMouseClick()
    {
        if (ItemSelectPanel.curBlockType != CSBlockType.None)
        {
            RaycastHit hit;
            bool b = GetPointingBlockInfo(out hit);
            if (b)
            {
                if (hit.normal == Vector3.right)
                {
                    //AddBlockReq(Vector3Int.FloorToInt(hit.transform.localPosition) + Vector3Int.right, ItemSelectPanel.curBlockType);
                    TerrainGenerator.GenerateBlock(hit.transform.localPosition + Vector3.right, ItemSelectPanel.curBlockType);
                }
                else if (hit.normal == Vector3.left)
                    TerrainGenerator.GenerateBlock(hit.transform.localPosition + Vector3.left, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.forward)
                    TerrainGenerator.GenerateBlock(hit.transform.localPosition + Vector3.forward, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.back)
                    TerrainGenerator.GenerateBlock(hit.transform.localPosition + Vector3.back, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.up)
                    TerrainGenerator.GenerateBlock(hit.transform.localPosition + Vector3.up, ItemSelectPanel.curBlockType);
                else if (hit.normal == Vector3.down)
                    TerrainGenerator.GenerateBlock(hit.transform.localPosition + Vector3.down, ItemSelectPanel.curBlockType);
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

    Material lastMaterial;
    void ShowWireFrame()
    {
        RaycastHit hit;
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        bool b = Physics.Raycast(Camera.main.ScreenPointToRay(center), out hit, 5f);
        if (b && hit.transform.tag == "Block")
        {
            Material material = hit.transform.GetComponent<Renderer>().material;
            if (lastMaterial != null && lastMaterial != material)
                lastMaterial.SetFloat("_Lightness", 1);
            material.SetFloat("_Lightness", 1.5f);
            lastMaterial = material;
        }
        else
        {
            if (lastMaterial != null)
                lastMaterial.SetFloat("_Lightness", 1);
            lastMaterial = null;
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
        Debug.Log("AddBlockRes,retCode=" + rsp.RetCode);
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
}
