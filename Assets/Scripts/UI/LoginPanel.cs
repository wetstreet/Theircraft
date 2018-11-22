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
        NetworkManager.Register(MessageType.ENTER_ROOM_RES, OnEnterRoomRes);

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
        EnterRoomReq enterRoomReq = new EnterRoomReq
        {
            Name = _name
        };
        NetworkManager.Enqueue(MessageType.ENTER_ROOM_REQ, enterRoomReq);
    }

    void OnEnterRoomRes(byte[] data)
    {
        EnterRoomRes rsp = NetworkManager.Deserialzie<EnterRoomRes>(data);
        Debug.Log("OnEnterRoomRes,retCode="+ rsp.RetCode);
        if(rsp.RetCode == 0)
        {
            DataCenter.name = playername;
            playername = null;
            DataCenter.state = ClientState.InRoom;
            SceneManager.LoadScene("MultiplayerScene");
            ChatPanel.AddLine("hello " + DataCenter.name + ", now you can chat freely in this room!");
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
