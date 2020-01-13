using LuaInterface;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

/// <summary>
/// 游戏内核 内容包括:
///
/// 
/// 管理Lua的初始化和加载 todo:热更界面后再启动gameKernel,暂时放到GameEntry
/// 
/// Add by Roshan
/// 2018-11-18 10:04:10
/// </summary>
public class GameKernel : LuaClient
{
    private bool hasLoadLua;
    private const string GAME_KERNEL_GO_NAME = "GAME_KERNEL";

    private static GameKernel _instance = null;

    public static void Create()
    {
        _instance = new GameObject(GAME_KERNEL_GO_NAME).AddComponent<GameKernel>();
        DontDestroyOnLoad(_instance.gameObject);
    }

    public static void Dispose()
    {
        if (_instance != null)
        {
            GameObject go = GameObject.Find(GAME_KERNEL_GO_NAME);
            if (go) GameObject.Destroy(go);
            _instance = null;
        }
    }


    protected override LuaFileUtils InitLoader()
    {
        return new LuaResLoader();
    }

    protected override void LoadLuaFiles()
    {
#if UNITY_EDITOR
        luaState.AddSearchPath(LuaConst.luaDir);
#endif
        OnLoadFinished();
        hasLoadLua = true;
    }
    
    private void OnApplicationQuit()
    {
        LuaBase.DisposeAll();
        Dispose();
    }
}