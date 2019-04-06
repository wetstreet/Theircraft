using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using protocol.cs_enum;
using protocol.cs_theircraft;

public class LoginPanel : MonoBehaviour {

    InputField accountInput;
    InputField passwordInput;

    static string AccountKey = "account";

    public static void ShowLoginPanel()
    {
        UISystem.InstantiateUI("LoginPanel");
    }

    // Use this for initialization
    void Start ()
    {
        NetworkManager.Register(ENUM_CMD.CS_LOGIN_RES, OnLoginRes);

        accountInput = transform.Find("Account/InputField").GetComponent<InputField>();
        passwordInput = transform.Find("Password/InputField").GetComponent<InputField>();
        transform.Find("ButtonLogin").GetComponent<Button>().onClick.AddListener(OnClickLogin);
        transform.Find("ButtonRegister").GetComponent<Button>().onClick.AddListener(OnClickRegister);

        string account = PlayerPrefs.GetString(AccountKey);
        if (account != null)
            accountInput.text = account;
	}

    void OnClickLogin()
    {
        if (accountInput.text == "")
        {
            FastTips.Show("用户名不能为空！");
            return;
        }
        if (passwordInput.text == "")
        {
            FastTips.Show("密码不能为空！");
            return;
        }

        LoginReq(accountInput.text, passwordInput.text);
        PlayerPrefs.SetString(AccountKey, accountInput.text);
    }

    void OnClickRegister()
    {
        RegisterAccountUI.Show();
    }
    
    void LoginReq(string account, string password)
    {
        CSLoginReq req = new CSLoginReq
        {
            Account = account,
            Password = password,
        };
        NetworkManager.Enqueue(ENUM_CMD.CS_LOGIN_REQ, req);
    }

    void OnLoginRes(byte[] data)
    {
        CSLoginRes rsp = NetworkManager.Deserialize<CSLoginRes>(data);
        Debug.Log("OnLoginRes," + rsp.RetCode);
        if(rsp.RetCode == 0)
        {
            DataCenter.playerID = rsp.PlayerData.PlayerID;
            DataCenter.name = rsp.PlayerData.Name;
            DataCenter.spawnPosition = Ultiities.CSVector3_To_Vector3(rsp.PlayerData.Position);
            DataCenter.spawnRotation = Ultiities.CSVector3_To_Vector3(rsp.PlayerData.Rotation);
            DataCenter.state = ClientState.InRoom;
            Destroy(gameObject);
            LoadingUI.Show();
            //SceneManager.LoadScene("MultiplayerScene");
            SceneManager.LoadScene("MergeTest");
            ChatPanel.AddLine(DataCenter.name + ", 欢迎回来！");
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
