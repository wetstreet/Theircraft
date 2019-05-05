using UnityEngine;
using UnityEngine.SceneManagement;

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
        SoundManager.Init();

        if (NetworkManager.Connect())
        {
            SceneManager.LoadScene("LoginScene");
            MainMenu.Show();
        }
        else
        {
            DisconnectedUI.Show();
        }
    }
}
