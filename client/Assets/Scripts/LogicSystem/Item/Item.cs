using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector] public float coolDownTime = 2f;

    Transform shadowTrans;
    Material shadow;
    Color shadowColor = Color.white;
    bool move2player;
    Vector3 startPosition;
    float time;
    CSBlockType type;

    static GameObject prefab;
    static Vector3 offset = new Vector3(0, 0.01f, 0);
    static Vector3 collectOffset = new Vector3(0, 0.75f, 0);
    static float maxDistance = 2f;
    static float maxAlpha = 0.6f;
    static float minDistance = 0.2f;
    static float moveTime = 0.2f;

    public static Item CreateBlockDropItem(CSBlockType type, Vector3 pos)
    {
        float right = Random.Range(-1f, 1f);
        float forward = Random.Range(-1f, 1f);
        Vector3 dir = Vector3.up + right * Vector3.right + forward * Vector3.forward;
        Item item = Create(type, pos, dir.normalized);
        item.coolDownTime = 0.5f;
        return item;
    }

    public static Item Create(CSBlockType type, Vector3 pos, Vector3 dir)
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/Item");
        }
        GameObject obj = Instantiate(prefab);
        obj.transform.position = pos;
        obj.GetComponent<Rigidbody>().AddForce(dir);
        Item item = obj.GetComponent<Item>();
        item.type = type;
        return item;
    }

    static Dictionary<CSBlockType, Mesh> type2mesh = new Dictionary<CSBlockType, Mesh>();

    static Dictionary<CSBlockType, string> type2path = new Dictionary<CSBlockType, string>
    {
        { CSBlockType.Dandelion, "dandelion" },
        { CSBlockType.Poppy, "poppy" },
        { CSBlockType.Grass, "grass" },
        { CSBlockType.Cobweb, "cobweb" },
        { CSBlockType.Torch, "torch" },
    };

    // Start is called before the first frame update
    void Start()
    {
        shadowTrans = transform.Find("shadow");
        shadow = shadowTrans.GetComponent<Renderer>().material;

        Transform meshTrans = transform.Find("mesh_parent/mesh");
        MeshFilter meshFilter = meshTrans.GetComponent<MeshFilter>();

        TexCoords coords = ChunkMeshGenerator.type2texcoords[(byte)type];
        if (!type2mesh.ContainsKey(type))
        {
            if (coords.isPlant || type == CSBlockType.Torch)
            {
                string path = type2path[type];
                type2mesh[type] = Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
            }
            else
            {
                type2mesh[type] = ChunkMeshGenerator.GetCubeMesh(type);
            }
        }

        meshFilter.sharedMesh = type2mesh[type];

        MeshRenderer renderer = meshTrans.GetComponent<MeshRenderer>();
        if (coords.isPlant || type == CSBlockType.Torch)
        {
            string path = type2path[type];
            renderer.material.mainTexture = Resources.Load<Texture2D>("Meshes/items/" + path + "/" + path);
        }
        else
        {
            renderer.material = Resources.Load<Material>("Materials/block");
            meshFilter.transform.localScale = Vector3.one / 2;
        }
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
                ItemSelectPanel.AddItem(type);
            }
        }
    }
}
