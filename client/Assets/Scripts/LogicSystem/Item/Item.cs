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

    // Start is called before the first frame update
    void Start()
    {
        shadowTrans = transform.Find("shadow");
        shadow = shadowTrans.GetComponent<Renderer>().material;

        Transform meshTrans = transform.Find("mesh_parent/mesh");
        MeshFilter meshFilter = meshTrans.GetComponent<MeshFilter>();

        meshFilter.sharedMesh = ChunkMeshGenerator.GetCubeMesh(type);

        TexCoords coords = ChunkMeshGenerator.type2texcoords[(byte)type];
        if (!coords.isPlant && type != CSBlockType.Torch)
        {
            meshFilter.transform.localScale = Vector3.one / 2;
        }

        meshTrans.GetComponent<MeshRenderer>().material.mainTexture = ChunkMeshGenerator.GetCubeTexture(type);
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
