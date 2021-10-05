using protocol.cs_enum;
using protocol.cs_theircraft;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 position = new Vector3();
    public Vector3 forward = new Vector3();
    public float horizontalScale = 1;
    public float verticalScale = 1;
    public Transform camera;

    private Vector3 verticalSpeed;
    private Vector3 horizontalSpeed;
    private CharacterController cc;
    static private Camera handCam;
    Animator handAnimator;
    MeshFilter blockMeshFilter;
    MeshRenderer blockMeshRenderer;
    MeshRenderer handMeshRenderer;
    GameObject vcamWide;

    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.7f;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();

    bool isMoving;
    static bool acceptInput = true;
    public static PlayerController instance;

    public static bool isInitialized = false;

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
            Object prefab = Resources.Load("Prefabs/Character");
            GameObject obj = Instantiate(prefab) as GameObject;
            instance = obj.GetComponent<PlayerController>();
            LockCursor(true);

            //Monster.CreateMonster(1, new Vector3(1, 20, 1));
        }
    }

    public static void Destroy()
    {
        DestroyImmediate(instance);
        instance = null;
        isInitialized = false;
    }

    // Use this for initialization
    void Start()
    {
        Physics.queriesHitBackfaces = true;

        Cursor.lockState = CursorLockMode.Locked;
        camera = transform.Find("camera");
        vcamWide = camera.Find("vcam_wide").gameObject;
        cc = GetComponent<CharacterController>();
        handCam = Camera.main.transform.Find("Camera").GetComponent<Camera>();
        handAnimator = Camera.main.transform.Find("hand").GetComponent<Animator>();

        m_HeadBob.Setup(camera, 5);

        transform.position = DataCenter.spawnPosition;
        transform.localEulerAngles = new Vector3(0, -DataCenter.spawnRotation.y, 0);
        camera.transform.localEulerAngles = new Vector3(DataCenter.spawnRotation.z, 0, 0);
        
        NetworkManager.Register(ENUM_CMD.CS_ADD_BLOCK_NOTIFY, OnAddBlockNotify);
        NetworkManager.Register(ENUM_CMD.CS_DELETE_BLOCK_NOTIFY, OnDeleteBlockNotify);

        LoadingUI.Close();
        CrossHair.Show();
        Hand.Show();

        blockMeshFilter = Camera.main.transform.Find("hand/block").GetComponent<MeshFilter>();
        blockMeshRenderer = Camera.main.transform.Find("hand/block").GetComponent<MeshRenderer>();
        handMeshRenderer = Camera.main.transform.Find("hand").GetComponent<MeshRenderer>();

        position = transform.position;
        forward = transform.forward;

        isInitialized = true;
    }

    static Vector3 compensation = new Vector3(0, 0.0001f, 0);

    public static Vector3Int GetCurrentBlock(Vector3 pos)
    {
        return Vector3Int.RoundToInt(pos + compensation);
    }

    public static Vector3Int GetCurrentBlock()
    {
        return GetCurrentBlock(instance.transform.position);
    }

    static Vector2Int chunkPos = new Vector2Int();

    public static Vector2Int GetCurrentChunkPos()
    {
        Vector3Int block;
        if (instance != null)
        {
            block = GetCurrentBlock();
        }
        else
        {
            block = GetCurrentBlock(DataCenter.spawnPosition);
        }
        chunkPos.x = Mathf.FloorToInt(block.x / 16f);
        chunkPos.y = Mathf.FloorToInt(block.z / 16f);
        return chunkPos;
    }

    public static Chunk GetCurrentChunk()
    {
        return ChunkManager.GetChunk(GetCurrentChunkPos());
    }

    // get the dot product between the player front vector and chunk to player vector.
    public static float GetChunkToFrontDot(NBTChunk chunk)
    {
        Vector2 chunk2player = new Vector2(chunk.globalX - instance.position.x, chunk.globalZ - instance.position.z);
        Vector2 playerForward = new Vector2(instance.forward.x, instance.forward.z);
        return Vector2.Dot(playerForward.normalized, chunk2player.normalized);
    }

    public static void ShowHand()
    {
        instance.handMeshRenderer.enabled = true;
        instance.blockMeshFilter.transform.gameObject.SetActive(false);
    }

    public static bool IsNearByChunk(NBTChunk chunk)
    {
        return Mathf.Abs(chunk.x - chunkPos.x) <= 1 && Mathf.Abs(chunk.z - chunkPos.y) <= 1;
    }
    
    public static float GetChunkDistance(NBTChunk chunk)
    {
        return Mathf.Sqrt(Mathf.Pow(chunk.x - chunkPos.x, 2) + Mathf.Pow(chunk.z - chunkPos.y, 2));
    }

    public static void ShowBlock(NBTObject generator, short data)
    {
        instance.handMeshRenderer.enabled = false;
        instance.blockMeshFilter.sharedMesh = generator.GetItemMesh(NBTHelper.GetChunk(GetCurrentBlock()), Vector3Int.RoundToInt(instance.position), (byte)data);
        instance.blockMeshRenderer.GetComponent<MeshRenderer>().sharedMaterial = generator.GetItemMaterial((byte)data);
        instance.blockMeshFilter.transform.gameObject.SetActive(true);
    }

    void PositionCorrection()
    {
        if (transform.localPosition.y < -100)
        {
            Vector3 pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, 100, pos.z);
            FastTips.Show("Position has been corrected!");
        }
    }

    bool leftMouseDown = false;

    void BreakBlock(Vector3Int pos)
    {
        breakTime = 0;

        handAnimator.SetBool("isBreaking", false);

        HideBreakingEffect();
        //DeleteBlockReq(WireFrameHelper.pos);

        NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(WireFrameHelper.type);
        if (generator.hasDropItem)
        {
            Item.CreateBlockDropItem(generator.GetDropItemByData(WireFrameHelper.data), generator.GetDropItemData(WireFrameHelper.data), pos);
        }

        NBTHelper.SetBlockByte(WireFrameHelper.pos, 0, true);

        //Item.CreateBlockDropItem(type, WireFrameHelper.pos);
        BreakBlockEffect.Create(WireFrameHelper.type, WireFrameHelper.data, WireFrameHelper.pos);
        SoundManager.PlayBreakSound(WireFrameHelper.type, instance.gameObject);
    }

    void OnLeftMouseDown()
    {
        leftMouseDown = true;

        handAnimator.SetTrigger("interactTrigger");
    }

    void OnLeftMouseUp()
    {
        leftMouseDown = false;
        breakTime = 0;
        HideBreakingEffect();

        handAnimator.SetBool("isBreaking", false);
    }

    bool CanAddBlock(Vector3Int pos)
    {
        NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(InventorySystem.items[ItemSelectPanel.curIndex].id);
        // 手上的
        if (generator == null) return false;

        byte type = NBTHelper.GetBlockByte(pos);
        NBTBlock targetGenerator = NBTGeneratorManager.GetMeshGenerator(type);

        if (generator is NBTPlant)
        {
            if (generator == targetGenerator) { return false; }

            byte belowType = NBTHelper.GetBlockByte(pos + Vector3Int.down);

            //如果手上拿的是植物，则判断下方是否是否是实体
            NBTBlock targetBelowGenerator = NBTGeneratorManager.GetMeshGenerator(belowType);
            return targetBelowGenerator != null && !(targetBelowGenerator is NBTPlant);
        }
        else
        {
            //如果手上拿的不是植物，则判断碰撞盒是否与玩家相交
            return !cc.bounds.Intersects(new Bounds(pos, Vector3.one));
        }
    }

    void OnRightClick()
    {
        string id = InventorySystem.items[ItemSelectPanel.curIndex].id;
        if (WireFrameHelper.render && id != null)
        {
            Vector3Int pos = WireFrameHelper.pos;

            pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

            if (CanAddBlock(pos))
            {
                handAnimator.SetTrigger("interactTrigger");

                byte type = NBTGeneratorManager.id2type[id];
                byte data = (byte)InventorySystem.items[ItemSelectPanel.curIndex].damage;
                NBTHelper.SetBlockData(pos, type, data, true);

                InventorySystem.DecrementCurrent();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
    }

    float timeInterval = 0.2f;
    bool needUpdate;
    float lastUpdateTime;
    float lastSpace;
    void Update()
    {
        DrawWireFrame();
        PositionCorrection();

        if (acceptInput)
        {
            RotateView();
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                OnLeftMouseDown();
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                OnLeftMouseUp();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                OnRightClick();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                uint slot = ItemSelectPanel.curIndex;
                InventoryItem item = InventorySystem.items[ItemSelectPanel.curIndex];
                if (item.id != null)
                {
                    NBTObject generator = NBTGeneratorManager.GetObjectGenerator(item.id);
                    if (generator != null)
                    {
                        short data = InventorySystem.items[slot].damage;
                        Item.CreatePlayerDropItem(generator, (byte)data);
                        InventorySystem.DecrementCurrent();
                        ItemSelectPanel.instance.RefreshUI();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Utilities.Capture();
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
            NetworkManager.SendPkgToServer(ENUM_CMD.CS_HERO_MOVE_REQ, req);
        }
    }

    MeshRenderer breakingEffect;
    MeshFilter breakingEffectMesh;
    void UpdateBreakingEffect(NBTBlock generator, Vector3Int pos, int stage)
    {
        if (breakingEffect == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/BreakingEffect");
            GameObject go = Instantiate(prefab);
            breakingEffect = go.GetComponent<MeshRenderer>();
            breakingEffectMesh = go.GetComponent<MeshFilter>();
            go.SetActive(false);
        }

        if (stage == 0)
        {
            breakingEffect.gameObject.SetActive(false);
        }
        else
        {
            breakingEffect.gameObject.SetActive(true);
            breakingEffect.transform.position = pos;
            breakingEffectMesh.sharedMesh = generator.GetItemMesh(NBTHelper.GetChunk(GetCurrentChunkPos()), pos, WireFrameHelper.data);

            breakingEffect.sharedMaterial.mainTexture = Resources.Load<Texture2D>("GUI/block/destroy_stage_" + (stage - 1));
        }
    }

    void HideBreakingEffect()
    {
        if (breakingEffect != null)
        {
            breakingEffect.gameObject.SetActive(false);
        }
    }

    RaycastHit hit;
    float breakTime;
    int stage;
    void DrawWireFrame()
    {
        WireFrameHelper.render = false;

        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        int cubeLayerIndex = LayerMask.NameToLayer("Chunk");
        int otherPlayerLayerIndex = LayerMask.NameToLayer("OtherPlayer");
        int plantLayerIndex = LayerMask.NameToLayer("Plant");
        if (cubeLayerIndex != -1 && otherPlayerLayerIndex != -1 && plantLayerIndex != -1)
        {
            int layerMask = 1 << cubeLayerIndex | 1 << otherPlayerLayerIndex | 1 << plantLayerIndex;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(center), out hit, 5f, layerMask))
            {
                if (hit.transform.gameObject.layer == cubeLayerIndex || hit.transform.gameObject.layer == plantLayerIndex)
                {
                    Vector3Int pos = Vector3Int.RoundToInt(hit.point - hit.normal / 10);
                    byte type = 0;
                    byte data = 0;
                    NBTHelper.GetBlockData(pos.x, pos.y, pos.z, ref type, ref data);
                    if (type != 0)
                    {
                        if (pos != WireFrameHelper.pos)
                        {
                            breakTime = 0;
                        }

                        WireFrameHelper.render = true;
                        WireFrameHelper.pos = pos;
                        WireFrameHelper.hitPos = hit.point;
                        WireFrameHelper.type = type;
                        WireFrameHelper.data = data;

                        if (leftMouseDown)
                        {
                            handAnimator.SetBool("isBreaking", true);

                            breakTime += Time.deltaTime;

                            NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(type);
                            float breakNeedTime = generator.breakNeedTime;
                            if (breakNeedTime == 0)
                            {
                                BreakBlock(pos);
                            }
                            else
                            {
                                int curStage = Mathf.FloorToInt(breakTime / (breakNeedTime / 12));
                                if (stage != curStage)
                                {
                                    stage = curStage;
                                    UpdateBreakingEffect(generator, pos, stage);
                                }
                                if (stage >= 10)
                                {
                                    BreakBlock(pos);
                                }
                            }

                            return;
                        }
                    }
                }
            }
        }

        handAnimator.SetBool("isBreaking", false);
    }

    bool isFlying = false;
    bool isCreative = false;

    public Vector3 jumpSpeed = new Vector3(0, 9f, 0);
    public Vector3 fallSpeed = new Vector3(0, -28f, 0);
    void Jump()
    {
        if (cc.isGrounded)
        {
            verticalSpeed = jumpSpeed;
        }

        if (isCreative)
        {
            if (Time.time - lastSpace <= 0.3f)
            {
                isFlying = !isFlying;
                vcamWide.SetActive(isFlying);
                //重置时间戳，防止连按三下
                lastSpace = 0;
            }
            else
            {
                lastSpace = Time.time;
            }
        }
    }

    [SerializeField] private float bobSpeed = 5f;

    private void UpdateCameraPosition()
    {
        Vector3 newCameraPosition;
        if (cc.velocity.magnitude > 0 && cc.isGrounded)
        {
            camera.transform.localPosition = m_HeadBob.DoHeadBob(cc.velocity.magnitude + (bobSpeed * m_RunstepLenghten));
            newCameraPosition = camera.transform.localPosition;
            //newCameraPosition.y = camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = camera.transform.localPosition;
            //newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        camera.transform.localPosition = newCameraPosition;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal != Vector3.up)
        {
            //非地面
            float diff = hit.point.y - transform.localPosition.y;
            if (diff > 0 && diff <= 0.5f)
            {
                cc.stepOffset = 0.5f;
            }
            else
            {
                cc.stepOffset = 0f;
            }
        }
        //if ((cc.collisionFlags & CollisionFlags.Above) > 0)
        //{
        //    Debug.Log("hit top");
        //    verticalSpeed = new Vector3(0, -Mathf.Abs(verticalSpeed.y), 0);
        //}
    }

    public float inAirSpeed = 0.1f;
    public float attenuation = 0.75f;
    public float flyVerticalSpeed = 5f;
    public float flyHorizontalSpeed = 3f;
    void ProcessMovement(float v, float h)
    {
        if (cc.isGrounded)
        {
            if (v != 0 || h != 0)
            {
                horizontalSpeed += transform.forward * v + transform.right * h;
            }
            else
            {
                horizontalSpeed *= attenuation;
            }
            isFlying = false;
            vcamWide.SetActive(false);
        }
        else
        {
            if (isFlying)
            {
                if (v != 0 || h != 0)
                {
                    if (horizontalSpeed.magnitude > 1)
                    {
                        horizontalSpeed += (transform.forward * v + transform.right * h) * 0.1f;
                    }
                    else
                    {
                        horizontalSpeed += (transform.forward * v + transform.right * h);
                    }
                    horizontalSpeed = Vector3.ClampMagnitude(horizontalSpeed, flyHorizontalSpeed);
                }
                else
                {
                    horizontalSpeed *= 0.9f;
                }
            }
            else
            {
                horizontalSpeed = horizontalSpeed + (transform.forward * v + transform.right * h) * inAirSpeed;
            }
        }
        if (!isFlying)
        {
            horizontalSpeed = Vector3.ClampMagnitude(horizontalSpeed, 1);
        }
        Vector3 horizontalMovement = horizontalSpeed * Time.fixedDeltaTime;

        if (isFlying)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                verticalSpeed += Vector3.up;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                verticalSpeed -= Vector3.up;
            }
            else
            {
                verticalSpeed *= attenuation;
            }
            verticalSpeed.y = Mathf.Clamp(verticalSpeed.y, -flyVerticalSpeed, flyVerticalSpeed);
        }
        else
        {
            verticalSpeed += fallSpeed * Time.fixedDeltaTime;
        }
        Vector3 verticalMovement = verticalSpeed * Time.fixedDeltaTime;

        //有移动则告诉服务器
        float precision = 0.001f;
        if (verticalMovement.sqrMagnitude > precision || horizontalMovement != Vector3.zero)
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

        position = transform.position;
        forward = transform.forward;

        if (cc.isGrounded)
        {
            verticalSpeed = Vector3.zero;
            if (verticalMovement.sqrMagnitude > precision)
            {
                AkSoundEngine.PostEvent("Player_Footstep", this.gameObject);
            }
            
            if (isMoving && horizontalSpeed.sqrMagnitude > 0.2f)
            {
                Vector3Int pos = Vector3Int.RoundToInt(transform.position - Vector3.up / 10);
                bool hasBlock = ChunkManager.HasBlock(pos);
                if (hasBlock)
                {
                    CSBlockType type = ChunkManager.GetBlockType(pos.x, pos.y, pos.z);
                    if (Time.realtimeSinceStartup - lastFootstepTime > footstepInterval)
                    {
                        //SoundManager.PlayFootstepSound(type, gameObject);
                        lastFootstepTime = Time.realtimeSinceStartup;
                    }
                }
            }
        }
    }
    float lastFootstepTime;
    public float footstepInterval = 0.4f;

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -90, 90);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    public float sensitivity = 2;

    void RotateView()
    {
        float x = Input.GetAxis("Mouse X") * sensitivity;
        float y = Input.GetAxis("Mouse Y") * sensitivity;
        if (x != 0 || y != 0)
        {
            camera.transform.localRotation *= Quaternion.Euler(-y, 0, 0);
            camera.transform.localRotation = ClampRotationAroundXAxis(camera.transform.localRotation);
            transform.localRotation *= Quaternion.Euler(0, x, 0);
            needUpdate = true;
        }
    }

    float v;
    float h;
    public float accumulation = 0.1f;
    void FixedUpdate()
    {
        float rawV = Input.GetAxisRaw("Vertical");
        float rawH = Input.GetAxisRaw("Horizontal");
        bool hasInput = rawH != 0 || rawV != 0;
        if (acceptInput)
        {
            v = rawV != 0 ? v + rawV * accumulation : 0;
            v = Mathf.Clamp(v, -1, 1);

            h = rawH != 0 ? h + rawH * accumulation : 0;
            h = Mathf.Clamp(h, -1, 1);
        }
        else
        {
            v = 0;
            h = 0;
        }
        //Debug.Log("rawV="+ rawV+",rawH="+rawH+",v=" + v + ",h=" + h);
        ProcessMovement(v, h);
        if (hasInput)
        {
            UpdateCameraPosition();
        }
    }

    void AddBlockReq(Vector3Int pos, CSBlockType type, CSBlockOrientation orient = CSBlockOrientation.Default)
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
                type = type,
                orient = orient,
            }
        };
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_ADD_BLOCK_REQ, addBlockReq, AddBlockRes);
    }

    void AddBlockReq(Vector3Int pos, CSBlockType type, Vector3Int dependPos)
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
                type = type,
                depentPos = dependPos.ToCSVector3Int(),
            }
        };
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_ADD_BLOCK_REQ, addBlockReq, AddBlockRes);
    }

    void AddBlockRes(object data)
    {
        CSAddBlockRes rsp = NetworkManager.Deserialize<CSAddBlockRes>(data);
        //Debug.Log("AddBlockRes,retCode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            ChunkManager.AddBlock(rsp.block);
            //SoundManager.PlayPlaceSound(rsp.block.type, gameObject);
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }

    void OnAddBlockNotify(object data)
    {
        //Debug.Log("OnAddBlockNotify");
        CSAddBlockNotify notify = NetworkManager.Deserialize<CSAddBlockNotify>(data);
        ChunkManager.AddBlock(notify.block);
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
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_DELETE_BLOCK_REQ, req, DeleteBlockRes);
    }

    void DeleteBlockRes(object data)
    {
        CSDeleteBlockRes rsp = NetworkManager.Deserialize<CSDeleteBlockRes>(data);
        //Debug.Log("DeleteBlockRes,retCode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            foreach (CSVector3Int pos in rsp.position)
            {
                ChunkManager.RemoveBlock(pos.ToVector3Int());
            }
            ChunkManager.RebuildChunks();
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }

    void OnDeleteBlockNotify(object data)
    {
        //Debug.Log("OnDeleteBlockNotify");
        CSDeleteBlockNotify notify = NetworkManager.Deserialize<CSDeleteBlockNotify>(data);
        foreach (CSVector3Int _pos in notify.position)
        {
            Vector3Int pos = _pos.ToVector3Int();
            ChunkManager.RemoveBlock(pos);
        }
        ChunkManager.RebuildChunks();
    }
}
