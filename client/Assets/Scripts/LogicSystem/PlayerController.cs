using Substrate.Nbt;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 position = new Vector3();
    public Vector3 forward = new Vector3();
    public float horizontalScale = 4;
    public float runSpeed = 8;
    public float verticalScale = 1;
    public Transform camera;

    public float healthInternal = 20f;
    public float Health
    {
        get { return healthInternal; }
        set
        {
            healthInternal = value;

            ItemSelectPanel.instance.RefreshStatus();

            if (healthInternal <= 0)
            {
                DeathUI.Show();
            }
        }
    }
    public int foodLevel = 20;

    private Vector3 verticalSpeed;
    private Vector3 horizontalSpeed;
    public CharacterController cc;
    static private Camera handCam;
    Animator handAnimator;
    MeshFilter blockMeshFilter;
    MeshRenderer blockMeshRenderer;
    MeshRenderer handMeshRenderer;
    GameObject vcamWide;

    NBTObject handObject
    {
        get
        {
            return NBTGeneratorManager.GetObjectGenerator(InventorySystem.items[ItemSelectPanel.curIndex].id);
        }
    }

    NBTItem handItem
    {
        get
        {
            string id = InventorySystem.items[ItemSelectPanel.curIndex].id;
            if (id == null) return null;
            NBTObject obj = NBTGeneratorManager.GetObjectGenerator(id);
            if (obj == null) return null;
            return obj as NBTItem;
        }
    }

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

        TagNodeCompound levelDat = NBTHelper.GetLevelDat();
        TagNodeCompound player = levelDat["Player"] as TagNodeCompound;
        TagNodeCompound abilities = player["abilities"] as TagNodeCompound;
        TagNodeByte flying = abilities["flying"] as TagNodeByte;
        isFlying = flying == 1 ? true : false;

        transform.position = DataCenter.spawnPosition;
        transform.localEulerAngles = new Vector3(0, -DataCenter.spawnRotation.y, 0);
        camera.transform.localEulerAngles = new Vector3(DataCenter.spawnRotation.z, 0, 0);
        
        //NetworkManager.Register(ENUM_CMD.CS_ADD_BLOCK_NOTIFY, OnAddBlockNotify);
        //NetworkManager.Register(ENUM_CMD.CS_DELETE_BLOCK_NOTIFY, OnDeleteBlockNotify);

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
        try
        {
            instance.blockMeshFilter.sharedMesh = generator.GetItemMesh(NBTHelper.GetChunk(GetCurrentBlock()), Vector3Int.RoundToInt(instance.position), (byte)data);
        }
        catch (System.Exception e)
        {
            Debug.LogError("showblock error, generator=" + generator + ",message=\n" + e.Message);
        }
        instance.blockMeshRenderer.GetComponent<MeshRenderer>().sharedMaterial = generator.GetItemMaterial((byte)data);
        instance.blockMeshFilter.transform.gameObject.SetActive(true);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        lastPosOnGround = pos;
    }

    Vector3 lastPosOnGround;
    void PositionCorrection()
    {
        if (cc.isGrounded)
        {
            float fallDistance = lastPosOnGround.y - transform.position.y;
            if (fallDistance > 4)
            {
                int fallDamage = (int)fallDistance - 3;
                OnHit(fallDamage);
            }
            lastPosOnGround = transform.position;
        }
        if (transform.position.y < -10)
        {
            transform.position = lastPosOnGround;
            horizontalSpeed = Vector3.zero;
            verticalSpeed = Vector3.zero;
            FastTips.Show("Position has been corrected!");
        }
    }

    public void OnHit(float damage)
    {
        if (GameModeManager.isSurvival)
        {
            Health -= damage;
            SoundManager.Play3DSound("Player_Hit", gameObject);
        }
    }

    float lastBreakTime;
    void BreakBlock(Vector3Int pos)
    {
        if (Time.time - lastBreakTime < 0.1f)
        {
            return;
        }
        lastBreakTime = Time.time;

        breakingTime = 0;

        HideBreakingEffect();
        //DeleteBlockReq(WireFrameHelper.pos);

        NBTBlock generator = WireFrameHelper.generator;
        if (generator.hasDropItem)
        {
            try
            {
                string id = generator.GetDropItemByData(WireFrameHelper.data);
                byte data = generator.GetDropItemData(WireFrameHelper.data);
                Item.CreateBlockDropItem(id, data, pos);
            }
            catch (System.Exception e)
            {
                Debug.LogError("create item error, id=" + generator.GetDropItemByData(WireFrameHelper.data) + ",generator=" + generator);
            }
        }
        generator.OnDestroyBlock(WireFrameHelper.pos, WireFrameHelper.data);

        if (generator.isTileEntity)
        {
            NBTChunk chunk = NBTHelper.GetChunk(WireFrameHelper.pos);
            chunk.RemoveTileEntityObj(WireFrameHelper.pos);
            NBTHelper.SetBlockByteNoUpdate(WireFrameHelper.pos, 0);
        }
        else
        {
            NBTHelper.SetBlockByte(WireFrameHelper.pos, 0);
        }


        //Item.CreateBlockDropItem(type, WireFrameHelper.pos);
        BreakBlockEffect.Create(WireFrameHelper.type, WireFrameHelper.data, WireFrameHelper.pos);
        SoundManager.PlayBreakSound(WireFrameHelper.type, instance.gameObject);
    }

    public void PlayHandAnimation()
    {
        handAnimator.SetTrigger("interactTrigger");
    }

    void OnLeftMouseDown()
    {
        handAnimator.Play("hand-interact", -1, 0);
    }

    void OnLeftMouseUp()
    {
        breakingTime = 0;
        HideBreakingEffect();
    }

    void OnRightClick()
    {
        if (WireFrameHelper.render)
        {
            if (WireFrameHelper.generator.canInteract)
            {
                WireFrameHelper.generator.OnRightClick();
            }
            else
            {
                string id = InventorySystem.items[ItemSelectPanel.curIndex].id;
                if (id != null)
                {
                    NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(id);
                    Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);
                    if (generator != null && generator.CanAddBlock(pos))
                    {
                        PlayHandAnimation();

                        generator.OnAddBlock(hit);

                        // decrese count
                        InventorySystem.DecrementCurrent();
                        ItemSelectPanel.instance.RefreshUI();

                        // play sound
                        SoundManager.SetSwitch(generator);
                        SoundManager.Play2DSound("Player_Place");
                    }
                }
            }
        }
    }

    public void Respawn()
    {
        TagNodeCompound player = NBTHelper.GetPlayerData();
        int x = player["SpawnX"] as TagNodeInt;
        int y = player["SpawnY"] as TagNodeInt;
        int z = player["SpawnZ"] as TagNodeInt;

        Vector3 spawnVec = new Vector3(x, y, z);
        SetPosition(spawnVec);

        NBTHelper.RespawnRefreshChunks();

        Time.timeScale = 1;
        Health = 20;
    }

    public void AddForce(Vector3 force)
    {
        horizontalSpeed.x += force.x;
        verticalSpeed.y += force.y;
        horizontalSpeed.z += force.z;
    }

    float attackInterval = 0.5f;
    float lastAttackTime;
    public float attackStrength = 5f;
    void OnLeftMousePressed()
    {
        //Debug.Log(ChatPanel.HideCode + "OnLeftMousePressed");

        int layerMask = 1 << LayerMask.NameToLayer("Monster");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(center), out RaycastHit hit, 5f, layerMask))
        {
            if (Time.time - lastAttackTime > attackInterval)
            {
                Monster monster = hit.transform.GetComponent<Monster>();
                float attackDamage = 1f;
                if (handItem != null)
                {
                    attackDamage = handItem.attackDamage;
                }
                monster.health -= attackDamage;
                Vector3 hitForce = transform.forward + Vector3.up;
                monster.AddForce(hitForce * attackStrength);
                monster.OnHit();

                if (monster.health <= 0)
                {
                    SoundManager.Play3DSound("Zombie_Death", monster.gameObject);
                }
                else
                {
                    SoundManager.Play3DSound("Zombie_Hurt", monster.gameObject);
                }
                //Debug.Log("monster health = " + monster.health);

                lastAttackTime = Time.time;
            }
        }

        if (WireFrameHelper.render)
        {
            handAnimator.CrossFade("hand-interact", 0);

            breakingTime += Time.deltaTime;

            NBTBlock targetGenerator = WireFrameHelper.generator;

            bool match = false;
            float speed = 1;
            if (handItem != null)
            {
                if (handItem.IsMatch(targetGenerator.blockMaterial))
                {
                    match = true;
                    speed = handItem.toolSpeed;
                }
            }
            if (!match)
            {
                match = targetGenerator.blockMaterial == BlockMaterial.Ground
                    || targetGenerator.blockMaterial == BlockMaterial.Wood
                    || targetGenerator.blockMaterial == BlockMaterial.Glass;
            }
            float breakNeedTime = targetGenerator.hardness * (match ? 1.5f : 5f) / speed;

            if (breakNeedTime == 0)
            {
                BreakBlock(WireFrameHelper.pos);
            }
            else if (breakNeedTime > 0)
            {
                int curStage = Mathf.FloorToInt(breakingTime / (breakNeedTime / 12));
                if (stage != curStage)
                {
                    stage = curStage;
                    UpdateBreakingEffect(targetGenerator, WireFrameHelper.pos, stage);
                }
                if (stage >= 10)
                {
                    BreakBlock(WireFrameHelper.pos);
                    if (handItem != null && handItem.durability != -1)
                    {
                        InventorySystem.items[ItemSelectPanel.curIndex].damage++;

                        if (InventorySystem.items[ItemSelectPanel.curIndex].damage >= handItem.durability)
                        {
                            InventorySystem.items[ItemSelectPanel.curIndex].id = null;
                            InventorySystem.items[ItemSelectPanel.curIndex].damage = 0;
                            InventorySystem.items[ItemSelectPanel.curIndex].count = 0;
                            SoundManager.Play2DSound("Player_Tool_Break");
                        }
                        ItemSelectPanel.instance.RefreshUI();
                    }
                }
            }
            else
            {
                breakingTime = 0;
                HideBreakingEffect();
            }
        }
        else
        {
            breakingTime = 0;
            HideBreakingEffect();
        }
    }

    bool isRun = false;

    float timeInterval = 0.2f;
    float lastSpace;
    float lastW;
    void Update()
    {
        DrawWireFrame();
        PositionCorrection();

        if (acceptInput)
        {
            RotateView();
            
            if (Input.GetKey(KeyCode.Mouse0))
            {
                OnLeftMousePressed();
            }
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
            if (!isFlying)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (Time.time - lastW < timeInterval)
                    {
                        isRun = true;
                        vcamWide.SetActive(true);
                        lastW = 0;
                    }
                    else
                    {
                        lastW = Time.time;
                    }
                }
                if (Input.GetKeyUp(KeyCode.W))
                {
                    isRun = false;
                    vcamWide.SetActive(false);
                }
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
            breakingEffectMesh.sharedMesh = generator.GetBreakingEffectMesh(NBTHelper.GetChunk(GetCurrentChunkPos()), pos, WireFrameHelper.data);

            breakingEffect.material.mainTexture = Resources.Load<Texture2D>("GUI/block/destroy_stage_" + (stage - 1));
        }
    }

    void HideBreakingEffect()
    {
        if (breakingEffect != null)
        {
            breakingEffect.gameObject.SetActive(false);
        }
    }


    Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    RaycastHit hit;
    float breakingTime;
    int stage;
    void DrawWireFrame()
    {
        WireFrameHelper.render = false;
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
                    Vector3Int pos = Vector3Int.RoundToInt(hit.point - hit.normal / 100);
                    NBTHelper.GetBlockData(pos.x, pos.y, pos.z, out byte type, out byte data);
                    if (type != 0)
                    {
                        if (pos != WireFrameHelper.pos)
                        {
                            breakingTime = 0;
                        }

                        WireFrameHelper.render = true;
                        WireFrameHelper.pos = pos;
                        WireFrameHelper.hitPos = hit.point;
                        WireFrameHelper.type = type;
                        WireFrameHelper.data = data;
                        WireFrameHelper.generator = NBTGeneratorManager.GetMeshGenerator(type);
                    }
                }
            }
        }
    }

    public bool isFlying = false;

    public Vector3 jumpSpeed = new Vector3(0, 9f, 0);
    public Vector3 fallSpeed = new Vector3(0, -28f, 0);
    void Jump()
    {
        if (cc.isGrounded)
        {
            verticalSpeed = jumpSpeed;
        }

        if (GameModeManager.isCreative)
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
            if (isFlying)
            {
                isFlying = false;
                vcamWide.SetActive(false);
            }
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

        float precision = 0.001f;

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
        cc.Move(horizontalMovement * (isRun ? runSpeed : horizontalScale) + verticalMovement * verticalScale);

        position = transform.position;
        forward = transform.forward;

        if (cc.isGrounded)
        {
            verticalSpeed = Vector3.zero;
            if ((verticalMovement.sqrMagnitude > precision) || (isMoving && horizontalSpeed.sqrMagnitude > 0.2f))
            {
                Vector3Int pos = Vector3Int.RoundToInt(transform.position - Vector3.up / 10);
                byte type = NBTHelper.GetBlockByte(pos.x, pos.y, pos.z);
                if (type != 0 && Time.realtimeSinceStartup - lastFootstepTime > footstepInterval)
                {
                    SoundManager.PlayFootstepSound(type, gameObject);
                    lastFootstepTime = Time.realtimeSinceStartup;
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

    //void AddBlockReq(Vector3Int pos, CSBlockType type, CSBlockOrientation orient = CSBlockOrientation.Default)
    //{
    //    CSAddBlockReq addBlockReq = new CSAddBlockReq
    //    {
    //        block = new CSBlock
    //        {
    //            position = new CSVector3Int
    //            {
    //                x = pos.x,
    //                y = pos.y,
    //                z = pos.z
    //            },
    //            type = type,
    //            orient = orient,
    //        }
    //    };
    //    NetworkManager.SendPkgToServer(ENUM_CMD.CS_ADD_BLOCK_REQ, addBlockReq, AddBlockRes);
    //}

    //void AddBlockReq(Vector3Int pos, CSBlockType type, Vector3Int dependPos)
    //{
    //    CSAddBlockReq addBlockReq = new CSAddBlockReq
    //    {
    //        block = new CSBlock
    //        {
    //            position = new CSVector3Int
    //            {
    //                x = pos.x,
    //                y = pos.y,
    //                z = pos.z
    //            },
    //            type = type,
    //            depentPos = dependPos.ToCSVector3Int(),
    //        }
    //    };
    //    NetworkManager.SendPkgToServer(ENUM_CMD.CS_ADD_BLOCK_REQ, addBlockReq, AddBlockRes);
    //}

    //void AddBlockRes(object data)
    //{
    //    CSAddBlockRes rsp = NetworkManager.Deserialize<CSAddBlockRes>(data);
    //    //Debug.Log("AddBlockRes,retCode=" + rsp.RetCode);
    //    if (rsp.RetCode == 0)
    //    {
    //        ChunkManager.AddBlock(rsp.block);
    //        //SoundManager.PlayPlaceSound(rsp.block.type, gameObject);
    //    }
    //    else
    //    {
    //        FastTips.Show(rsp.RetCode);
    //    }
    //}

    //void OnAddBlockNotify(object data)
    //{
    //    //Debug.Log("OnAddBlockNotify");
    //    CSAddBlockNotify notify = NetworkManager.Deserialize<CSAddBlockNotify>(data);
    //    ChunkManager.AddBlock(notify.block);
    //}

    //void DeleteBlockReq(Vector3 pos)
    //{
    //    CSDeleteBlockReq req = new CSDeleteBlockReq
    //    {
    //        position = new CSVector3Int
    //        {
    //            x = Mathf.RoundToInt(pos.x),
    //            y = Mathf.RoundToInt(pos.y),
    //            z = Mathf.RoundToInt(pos.z)
    //        }
    //    };
    //    NetworkManager.SendPkgToServer(ENUM_CMD.CS_DELETE_BLOCK_REQ, req, DeleteBlockRes);
    //}

    //void DeleteBlockRes(object data)
    //{
    //    CSDeleteBlockRes rsp = NetworkManager.Deserialize<CSDeleteBlockRes>(data);
    //    //Debug.Log("DeleteBlockRes,retCode=" + rsp.RetCode);
    //    if (rsp.RetCode == 0)
    //    {
    //        foreach (CSVector3Int pos in rsp.position)
    //        {
    //            ChunkManager.RemoveBlock(pos.ToVector3Int());
    //        }
    //        ChunkManager.RebuildChunks();
    //    }
    //    else
    //    {
    //        FastTips.Show(rsp.RetCode);
    //    }
    //}

    //void OnDeleteBlockNotify(object data)
    //{
    //    //Debug.Log("OnDeleteBlockNotify");
    //    CSDeleteBlockNotify notify = NetworkManager.Deserialize<CSDeleteBlockNotify>(data);
    //    foreach (CSVector3Int _pos in notify.position)
    //    {
    //        Vector3Int pos = _pos.ToVector3Int();
    //        ChunkManager.RemoveBlock(pos);
    //    }
    //    ChunkManager.RebuildChunks();
    //}
}
