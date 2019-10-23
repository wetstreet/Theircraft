using protocol.cs_enum;
using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterAccountUI : MonoBehaviour
{
    TMP_InputField accountInput;
    TMP_InputField nameInput;
    TMP_InputField passwordInput;
    TMP_InputField passwordRepeatInput;

    static RegisterAccountUI instance;

    public static void Show()
    {
        if (instance == null)
        {
            instance = UISystem.InstantiateUI("RegisterAccountUI").GetComponent<RegisterAccountUI>();
        }

    }
    public static void Close()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        accountInput = transform.Find("inputGrid/Account/RawImage/InputField").GetComponent<TMP_InputField>();
        nameInput = transform.Find("inputGrid/Name/RawImage/InputField").GetComponent<TMP_InputField>();
        passwordInput = transform.Find("inputGrid/Password/RawImage/InputField").GetComponent<TMP_InputField>();
        passwordRepeatInput = transform.Find("inputGrid/RepeatPassword/RawImage/InputField").GetComponent<TMP_InputField>();
        Utilities.SetClickCallback(transform, "btnGrid/ButtonOk", OnClickOk);
        Utilities.SetClickCallback(transform, "btnGrid/ButtonCancel", OnClickCancel);
    }

    void OnClickOk()
    {
        if (passwordInput.text != passwordRepeatInput.text)
        {
            FastTips.Show("密码不一致");
            return;
        }
        CSRegisterReq req = new CSRegisterReq
        {
            Account = accountInput.text,
            Name = nameInput.text,
            Password = passwordInput.text
        };
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_REGISTER_REQ, req, OnRegisterAccountRes);
    }

    void OnClickCancel()
    {
        Close();
    }


    void OnRegisterAccountRes(byte[] data)
    {
        CSRegisterRes rsp = NetworkManager.Deserialize<CSRegisterRes>(data);
        Debug.Log("CSRegisterRes," + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            FastTips.Show("注册成功");
            Close();
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
