using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    static CrossHair instance;
    public static void Show()
    {
        if (instance == null)
        {
            instance = UISystem.InstantiateUI("crosshair").GetComponent<CrossHair>();
            instance.transform.SetAsFirstSibling();
        }
        else
            instance.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if (instance != null)
            instance.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
