using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FastTips : MonoBehaviour {

    public static void Show(int retCode)
    {
        string content;
        switch(retCode)
        {
            case -1:
                content = "you are not in room";
                break;
            case 1:
                content = "cannot send empty message";
                break;
            default:
                content = "unknown error";
                break;
        }
        Show(content);
    }

    public static void Show(string content)
    {
        GameObject obj = UISystem.InstantiateUI("FastTips");
        Text text = obj.transform.Find("Image/Text").GetComponent<Text>();
        text.text = content;
    }

    Transform image;
    float createTime;

	// Use this for initialization
	void Start () {
        image = transform.Find("Image");
        createTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - createTime > 1)
        {
            Destroy(gameObject);
        }
	}

    void FixedUpdate()
    {
        Vector3 pos = image.transform.localPosition;
        pos.y = pos.y + 1;
        image.transform.localPosition = pos;
    }
}
