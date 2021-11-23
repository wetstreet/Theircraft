using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//这个是ui的手，要控制模型的手的话去PlayerController
public class Hand : MonoBehaviour
{
    static Hand instance;
    public static void Show()
    {
        if (instance == null)
        {
            instance = UISystem.InstantiateUI("hand").GetComponent<Hand>();
            instance.transform.SetAsFirstSibling();
        }
        else
            instance.gameObject.SetActive(true);
    }

    RenderTexture rt;
    RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = transform.GetComponent<RawImage>();

        rt = new RenderTexture(Screen.width, Screen.height, 16);
        PlayerController.SetHandRT(rt);
        rawImage.texture = rt;
    }
}
