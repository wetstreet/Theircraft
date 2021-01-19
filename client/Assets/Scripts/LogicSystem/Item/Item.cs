using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int Count
    {
        get;
        private set;
    }

    [HideInInspector] public float coolDownTime = 2f;
    [HideInInspector] public CSBlockType type;
    [HideInInspector] public byte blockType;
    [HideInInspector] public byte blockData;
    [HideInInspector] public bool destroyed;

    bool singleMesh = true;
    Transform shadowTrans;
    Material shadow;
    Color shadowColor = Color.white;
    bool move2player;
    Vector3 startPosition;
    float time;
    Transform meshTrans;

    static GameObject prefab;
    static Vector3 offset = new Vector3(0, 0.01f, 0);
    static Vector3 collectOffset = new Vector3(0, 0.75f, 0);
    static float maxDistance = 2f;
    static float maxAlpha = 0.6f;
    static float minDistance = 0.2f;
    static float moveTime = 0.2f;

    public static Item CreateBlockDropItem(byte type, byte data, Vector3 pos)
    {
        float right = Random.Range(-1f, 1f);
        float forward = Random.Range(-1f, 1f);
        Vector3 dir = Vector3.up + right * Vector3.right + forward * Vector3.forward;
        Item item = Create(type, data, pos, dir.normalized);
        item.coolDownTime = 0.5f;
        return item;
    }

    public static Item Create(byte type, byte data, Vector3 pos, Vector3 dir, int count = 1)
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/Item");
        }
        GameObject obj = Instantiate(prefab);
        obj.transform.position = pos;
        obj.GetComponent<Rigidbody>().AddForce(dir);
        Item item = obj.GetComponent<Item>();
        item.blockType = type;
        item.blockData = data;
        item.Count = count;
        return item;
    }

    public static Vector3 dropOffset = new Vector3(0, 1.1f, 0);
    public static Item CreatePlayerDropItem(byte type, byte data, int count = 1)
    {
        return Create(type, data, PlayerController.instance.position + dropOffset, PlayerController.instance.camera.transform.forward * 3, count);
    }

    void RefreshMesh()
    {
        if (Count > 1 && singleMesh)
        {
            singleMesh = false;
            Transform mesh = Instantiate(meshTrans);
            Destroy(mesh.GetComponent<Animator>());
            mesh.parent = meshTrans;
            NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(blockType);
            mesh.localPosition = generator is NBTPlant ? plantOffset : cubeOffset;
            mesh.localEulerAngles = Vector3.zero;
            mesh.localScale = Vector3.one;
        }
    }

    static Vector3 plantOffset = new Vector3(0.0625f, 0.0625f, 0.1f);
    static Vector3 cubeOffset = new Vector3(0.5f, 0.2f, 0.2f);
    public void AddCount(int count)
    {
        Count += count;
        RefreshMesh();
    }

    // Start is called before the first frame update
    void Start()
    {
        shadowTrans = transform.Find("shadow");
        shadow = shadowTrans.GetComponent<Renderer>().material;

        meshTrans = transform.Find("mesh_parent/mesh");
        MeshFilter meshFilter = meshTrans.GetComponent<MeshFilter>();

        NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(blockType);
        
        meshFilter.sharedMesh = generator.GetItemMesh(NBTHelper.GetChunk(PlayerController.GetCurrentBlock()), blockData);
        if (!(generator is NBTPlant))
        {
            meshFilter.transform.localScale = Vector3.one / 2;
        }

        meshTrans.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Array", TextureArrayManager.GetArray());

        RefreshMesh();
    }

    public void StartMove()
    {
        GetComponent<Rigidbody>().useGravity = false;
        move2player = true;
        startPosition = transform.position;
    }

    void Update()
    {
        if (Physics.Raycast(transform.position + offset, Vector3.down, out RaycastHit hit))
        {
            shadowTrans.position = hit.point + offset;
            shadowColor.a = maxAlpha * ((maxDistance - Mathf.Min(hit.distance, maxDistance)) / maxDistance);
            shadow.color = shadowColor;
        }
        if (move2player)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, PlayerController.instance.position + collectOffset, time / moveTime);
            float distance = Vector3.Distance(transform.position, PlayerController.instance.position + collectOffset);
            if (distance < minDistance)
            {
                Destroy(gameObject);
                SoundManager.PlayPopSound();
                InventorySystem.Increment(blockType, blockData, (byte)Count);
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }
}
