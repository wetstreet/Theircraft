using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DisconnectedUI : MonoBehaviour
{

    public static void Show()
    {
        UISystem.InstantiateUI("DisconnectedUI");
    }

    // Use this for initialization
    void Start () {
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnClickRetry);
    }

    void OnClickRetry()
    {
        if (NetworkManager.Connect())
        {
            Destroy(gameObject);
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            FastTips.Show("connection failed");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
