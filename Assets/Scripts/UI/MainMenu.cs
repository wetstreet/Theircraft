using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    static MainMenu instance;

    public static void Show()
    {
        instance = UISystem.InstantiateUI("MainMenu").GetComponent<MainMenu>();
    }

    public static void Close()
    {
        if (instance != null)
            Destroy(instance.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("ButtonLogin").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        transform.Find("ButtonRegister").GetComponent<Button>().onClick.AddListener(OnClickRegister);
        transform.Find("ButtonQuit").GetComponent<Button>().onClick.AddListener(OnClickQuit);
    }

    void OnClickLogin()
    {
        LoginPanel.ShowLoginPanel();
    }

    void OnClickRegister()
    {
        RegisterAccountUI.Show();
    }

    void OnClickQuit()
    {
        Application.Quit();
    }
}
