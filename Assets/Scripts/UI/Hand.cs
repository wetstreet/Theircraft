using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        rt = new RenderTexture(Screen.width, Screen.height, 0);
        mergetestPlayerController.SetHandRT(rt);
        rawImage.texture = rt;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
