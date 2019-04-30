using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public static void Show()
    {
        UISystem.InstantiateUI("MainMenu");
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
