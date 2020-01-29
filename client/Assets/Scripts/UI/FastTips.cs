using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FastTips : MonoBehaviour {

    public static void Show(int retCode)
    {
        string content;
        switch(retCode)
        {
            case -1:
                content = "Not Connected";
                break;
            case 1:
                content = "Cannot Send Empty Message";
                break;
            case 2:
                content = "Block Already Exists";
                break;
            case 3:
                content = "Block Does Not Exist";
                break;
            case 4:
                content = "Enter/Leave Chunk View Error";
                break;
            case 5:
                content = "Account Already Logged In";
                break;
            case 6:
                content = "Account Does Not Exist";
                break;
            case 7:
                content = "Password Does Not Match";
                break;
            case 8:
                content = "Account Already Exists";
                break;
            default:
                content = "Unknkown Error";
                break;
        }
        Show(content);
    }

    public static void Show(string content)
    {
        GameObject obj = UISystem.InstantiateUI("FastTips");
        TextMeshProUGUI text = obj.transform.Find("RawImage/Image/Text").GetComponent<TextMeshProUGUI>();
        text.text = content;
    }
    
    float createTime;

	// Use this for initialization
	void Start () {
        createTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - createTime > 2)
        {
            Destroy(gameObject);
        }
	}

    void FixedUpdate()
    {
        Vector3 pos = transform.localPosition;
        pos.y = pos.y + 1;
        transform.localPosition = pos;
    }
}
