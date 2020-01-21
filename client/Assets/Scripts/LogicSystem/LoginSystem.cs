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

    static void OnLoginRes(object data)
    {
        CSLoginRes rsp = NetworkManager.Deserialize<CSLoginRes>(data);
        Debug.Log("OnLoginRes,retcode=" + rsp.RetCode);
        if (rsp.RetCode == 0)
        {
            DataCenter.playerID = rsp.PlayerData.PlayerID;
            DataCenter.name = rsp.PlayerData.Name;
            DataCenter.spawnPosition = rsp.PlayerData.Position.ToVector3();
            DataCenter.spawnRotation = rsp.PlayerData.Rotation.ToVector3();
            DataCenter.state = ClientState.InRoom;
            LoginPanel.Close();
            MainMenu.Close();
            LoadingUI.Show();
            //SceneManager.LoadScene("MultiplayerScene");
            SceneManager.LoadScene("GameScene");
            ChatPanel.AddLine(DataCenter.name + ", welcome!");
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
}
