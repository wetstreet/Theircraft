using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginPanel : MonoBehaviour {
    TMP_InputField accountInput;
    TMP_InputField passwordInput;

    static LoginPanel instance;

    static string AccountKey = "account";

    public static void ShowLoginPanel()
    {
        instance = UISystem.InstantiateUI("LoginPanel").GetComponent<LoginPanel>();
    }

    // Use this for initialization
    void Start ()
    {

        accountInput = transform.Find("inputGrid/Account/RawImage/InputField").GetComponent<TMP_InputField>();
        passwordInput = transform.Find("inputGrid/Password/RawImage/InputField").GetComponent<TMP_InputField>();
        Utilities.SetClickCallback(transform, "btnGrid/ButtonOk", OnClickOk);
        Utilities.SetClickCallback(transform, "btnGrid/ButtonCancel", OnClickCancel);

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

        LoginSystem.LoginReq(accountInput.text, passwordInput.text);
        PlayerPrefs.SetString(AccountKey, accountInput.text);
    }

    public static void Close()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    void OnClickCancel()
    {
        Close();
    }

    void OnClickRegister()
    {
        RegisterAccountUI.Show();
    }
}
