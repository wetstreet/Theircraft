using UnityEngine;

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
        {
            Destroy(instance.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Utilities.SetClickCallback(transform, "ButtonSingle", OnClickSingle);
        //Utilities.SetClickCallback(transform, "ButtonLogin", OnClickLogin);
        //Utilities.SetClickCallback(transform, "ButtonRegister", OnClickRegister);
        Utilities.SetClickCallback(transform, "ButtonQuit", OnClickQuit);
        Utilities.SetClickCallback(transform, "ButtonClear", OnClickClear);
    }

    void OnClickSingle()
    {
        LoginSystem.LoginSingle();
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

    void OnClickClear()
    {
        LocalServer.ClearData();
        FastTips.Show("Clear Map Data & Player Data Done!");
    }
}
