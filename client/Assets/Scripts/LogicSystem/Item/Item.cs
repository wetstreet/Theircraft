using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool move2player;

    Transform shadowTrans;
    Material shadow;

    static GameObject prefab;
    public static Item Create(CSBlockType type, Vector3 pos, Vector3 dir)
    {
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/Item");
        }
        GameObject obj = Instantiate(prefab);
        obj.transform.position = pos;
        obj.GetComponent<Rigidbody>().AddForce(dir);
        return obj.GetComponent<Item>();
    }


    // Start is called before the first frame update
    void Start()
    {
        shadowTrans = transform.Find("shadow");
        shadow = shadowTrans.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    static Vector3 offset = new Vector3(0, 0.01f, 0);
    static float maxDistance = 2f;
    static float maxAlpha = 0.6f;
    static float minDistance = 0.1f;
    static float speed = 20;

    Color shadowColor = Color.white;
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
            transform.position = Vector3.Lerp(transform.position, PlayerController.instance.position + Vector3.up, speed * Time.deltaTime);
            float distance = Vector3.Distance(transform.position, PlayerController.instance.position + Vector3.up);
            if (distance < minDistance)
            {
                Destroy(gameObject);
                ItemSelectPanel.AddItem(CSBlockType.Dandelion);
            }
        }
    }
}
