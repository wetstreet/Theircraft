using UnityEngine;
public enum GameMode
{
    Local,
    Online,
}

public class Main : MonoBehaviour {

    public static Main instance;

    //禁止在运行时改变游戏模式
    public GameMode mode = GameMode.Local;
    //外网服务器ip
    public string OnlineServerIP = "39.108.139.67";
    //内网服务器ip
    public string LocalServerIP = "10.0.4.254";

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start ()
    {
        InputManager.Init();

        if (NetworkManager.Connect())
        {
            LoginPanel.ShowLoginPanel();
        }
        else
        {
            DisconnectedUI.Show();
        }
    }
}
