using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using protocol.cs_enum;
using protocol.cs_theircraft;
using TMPro;

public class LoginPanel : MonoBehaviour {
    TMP_InputField accountInput;
    TMP_InputField passwordInput;

    static string AccountKey = "account";

    public static void ShowLoginPanel()
    {
        UISystem.InstantiateUI("LoginPanel");
    }

    // Use this for initialization
    void Start ()
    {
        NetworkManager.Register(ENUM_CMD.CS_LOGIN_RES, OnLoginRes);

        accountInput = transform.Find("Account/InputField").GetComponent<TMP_InputField>();
        passwordInput = transform.Find("Password/InputField").GetComponent<TMP_InputField>();
        transform.Find("ButtonOk").GetComponent<Button>().onClick.AddListener(OnClickOk);
        transform.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(OnClickCancel);

        string account = PlayerPrefs.GetString(AccountKey);
        if (account != null)
            accountInput.text = account;
	}

    void OnClickOk()
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

    void OnClickCancel()
    {
        Destroy(gameObject);
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
        Debug.Log("OnLoginRes,retcode=" + rsp.RetCode);
        if(rsp.RetCode == 0)
        {
            DataCenter.playerID = rsp.PlayerData.PlayerID;
            DataCenter.name = rsp.PlayerData.Name;
            DataCenter.spawnPosition = Ultiities.CSVector3_To_Vector3(rsp.PlayerData.Position);
            DataCenter.spawnRotation = Ultiities.CSVector3_To_Vector3(rsp.PlayerData.Rotation);
            DataCenter.state = ClientState.InRoom;
            if (gameObject != null)
                Destroy(gameObject);
            MainMenu.Close();
            LoadingUI.Show();
            //SceneManager.LoadScene("MultiplayerScene");
            SceneManager.LoadScene("MergeTest");
            ChatPanel.AddLine(DataCenter.name + ", welcome!");
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
