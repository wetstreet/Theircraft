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
        NetworkManager.Register(CSMessageType.ENTER_ROOM_RES, OnEnterRoomRes);

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
            EnterRoomReq(input.text);
            PlayerPrefs.SetString(AccountKey, input.text);
        }
        else
            FastTips.Show("用户名不能为空！");
    }

    string playername;
    void EnterRoomReq(string _name)
    {
        playername = _name;
        CSEnterRoomReq enterRoomReq = new CSEnterRoomReq
        {
            Name = _name
        };
        NetworkManager.Enqueue(CSMessageType.ENTER_ROOM_REQ, enterRoomReq);
    }

    void OnEnterRoomRes(byte[] data)
    {
        CSEnterRoomRes rsp = NetworkManager.Deserialzie<CSEnterRoomRes>(data);
        Debug.Log("OnEnterRoomRes,retCode="+ rsp.RetCode);
        if(rsp.RetCode == 0)
        {
            DataCenter.name = playername;
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
