using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_enum;
using protocol.cs_theircraft;
using UnityEngine.SceneManagement;

public class LoginSystem
{
    public static void LoginSingle()
    {
        NetworkManager.IsSingle = true;
        LoginReq("test", "test");
    }

    public static void LoginReq(string account, string password)
    {
        CSLoginReq req = new CSLoginReq
        {
            Account = account,
            Password = password,
        };
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_LOGIN_REQ, req, OnLoginRes);
    }

    static void OnLoginRes(byte[] data)
    {
        CSLoginRes rsp = NetworkManager.Deserialize<CSLoginRes>(data);
        Debug.Log("OnLoginRes,retcode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            DataCenter.playerID = rsp.PlayerData.PlayerID;
            DataCenter.name = rsp.PlayerData.Name;
            DataCenter.spawnPosition = Utilities.CSVector3_To_Vector3(rsp.PlayerData.Position);
            DataCenter.spawnRotation = Utilities.CSVector3_To_Vector3(rsp.PlayerData.Rotation);
            DataCenter.state = ClientState.InRoom;
            LoginPanel.Close();
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
