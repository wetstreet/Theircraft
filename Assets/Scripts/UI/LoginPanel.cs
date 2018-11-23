using UnityEngine;
using UnityEngine.UI;
using Theircraft;
using UnityEngine.SceneManagement;

public class LoginPanel : MonoBehaviour {

    InputField input;

    static string AccountKey = "account";

    public static void ShowLoginPanel()
    {
        UISystem.InstantiateUI("LoginPanel");
    }

    // Use this for initialization
    void Start ()
    {
        NetworkManager.Register(CSMessageType.LOGIN_RES, OnLoginRes);

        input = transform.Find("InputField").GetComponent<InputField>();
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnClickEnterRoom);

        string account = PlayerPrefs.GetString(AccountKey);
        if (account != null)
            input.text = account;
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnClickEnterRoom()
    {
        if (input.text != "")
        {
            LoginReq(input.text);
            PlayerPrefs.SetString(AccountKey, input.text);
        }
        else
            FastTips.Show("用户名不能为空！");
    }

    string playername;
    void LoginReq(string _name)
    {
        playername = _name;
        CSLoginReq enterRoomReq = new CSLoginReq
        {
            Name = _name
        };
        NetworkManager.Enqueue(CSMessageType.LOGIN_REQ, enterRoomReq);
    }

    void OnLoginRes(byte[] data)
    {
        CSLoginRes rsp = NetworkManager.Deserialzie<CSLoginRes>(data);
        Debug.Log("OnEnterRoomRes,retCode="+ rsp.RetCode);
        if(rsp.RetCode == 0)
        {
            DataCenter.name = playername;
            DataCenter.initialPosition = new Vector3(0, 10, 0);
            playername = null;
            DataCenter.state = ClientState.InRoom;
            SceneManager.LoadScene("MultiplayerScene");
            ChatPanel.AddLine(DataCenter.name + ", 欢迎回来！");
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
