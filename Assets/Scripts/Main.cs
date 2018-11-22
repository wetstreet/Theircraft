using UnityEngine;

//连接本地服务器还是外网服务器
public enum ServerMode
{
    Local,
    Public,
}

public class Main : MonoBehaviour {

    public static Main instance;

    //禁止在建立链接后改变服务器
    public ServerMode mode = ServerMode.Public;
    //外网服务器ip
    public string PublicServerIP = "39.108.139.67";
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
