using UnityEngine;


public class Main : MonoBehaviour {
    public enum ServerMode
    {
        Local,
        Public,
    }
    public ServerMode mode = ServerMode.Public;
    public string PublicServerIP = "39.108.139.67";
    public string LocalServerIP = "10.0.4.254";

    void Awake()
    {
        if (mode == ServerMode.Local)
            NetworkManager.ip = LocalServerIP;
        else if (mode == ServerMode.Public)
            NetworkManager.ip = PublicServerIP;
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
