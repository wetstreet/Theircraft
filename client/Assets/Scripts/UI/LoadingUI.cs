using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    static LoadingUI instance;
    public static bool isLoading;

    public static void Show()
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
        }
        else
            instance = UISystem.InstantiateUI("loading").GetComponent<LoadingUI>();
        isLoading = true;
    }

    public static void Close()
    {
        if (instance != null) Destroy(instance.gameObject);
        isLoading = false;
        InputManager.Init();
    }

    public static void SetOnTop()
    {
        if (instance != null) instance.transform.SetAsLastSibling();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
