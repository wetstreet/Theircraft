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
                content = "未连接";
                break;
            case 1:
                content = "无法发送空数据";
                break;
            case 2:
                content = "方块已存在";
                break;
            case 3:
                content = "方块不存在";
                break;
            case 4:
                content = "该区块不存在";
                break;
            case 5:
                content = "用户已登录";
                break;
            default:
                content = "未知错误";
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
