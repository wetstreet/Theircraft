using System;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine;

/// <summary>
/// 简单的挂载Lua到gameObject组件 目前只包含了Start,Update, OnTriggerEnter,OnTriggerStay, OnTriggerExit分发到lua
/// Add by Roshan
/// 2018-11-17 16:55:29
/// </summary>
public class LuaBase : MonoBehaviour
{
    [SerializeField] public int ID;
    [SerializeField] public bool isMulti; //判断是否是多实例
    [SerializeField] public string fullLuaFileName;

    private string _luaFileName;
    private string _luaClassName;
    private bool isFinishLuaInit;
    private bool isStart;
    private bool isClose;

    protected static LinkedList<LuaBase> uiList = new LinkedList<LuaBase>();
    private LuaState luaState;
    private LuaTable _luaTable;
    private LuaFunction update = null;
    private LuaFunction lateUpdate = null;
    private LuaFunction fixedUpdate = null;
    private LuaFunction onTriggerEnterFunc = null;
    private LuaFunction onTriggerStayFunc = null;
    private LuaFunction onTriggerExitFunc = null;


    protected void Awake()
    {
        luaState = LuaClient.GetMainState();
        InitLuaSetting();
        if (isFinishLuaInit)
        {
            uiList.AddLast(this);
        }
    }

    protected void Start()
    {
        if (isFinishLuaInit)
        {
            _luaTable.Call("Start", _luaTable);
        }

        InitUpdateEvent();
        isStart = true;
    }

    protected void OnEnable()
    {
        if (isStart)
        {
            InitUpdateEvent();
        }
    }

    protected void OnDisable()
    {
        RemoveUpdateEvent();
    }

    protected void OnTriggerEnter(Collider collider)
    {
        if (onTriggerEnterFunc != null)
        {
            onTriggerEnterFunc.BeginPCall();
            onTriggerEnterFunc.Push(_luaTable);
            onTriggerEnterFunc.Push(collider);
            onTriggerEnterFunc.PCall();
            onTriggerEnterFunc.EndPCall();
        }
    }

    protected void OnTriggerStay(Collider collider)
    {
        if (onTriggerStayFunc != null)
        {
            onTriggerStayFunc.BeginPCall();
            onTriggerStayFunc.Push(_luaTable);
            onTriggerStayFunc.Push(collider);
            onTriggerStayFunc.PCall();
            onTriggerStayFunc.EndPCall();
        }
    }

    protected void OnTriggerExit(Collider collider)
    {
        if (onTriggerExitFunc != null)
        {
            onTriggerExitFunc.BeginPCall();
            onTriggerExitFunc.Push(_luaTable);
            onTriggerExitFunc.Push(collider);
            onTriggerExitFunc.PCall();
            onTriggerExitFunc.EndPCall();
        }
    }

    private void OnDestroy()
    {
        if (isClose == false)
        {
            isClose = true;

            if (isFinishLuaInit && luaState != null)
                _luaTable.Call("OnDestroy", _luaTable);

            ReleaseLua();
        }
    }


    void InitLuaSetting()
    {
        try
        {
            if (string.IsNullOrEmpty(fullLuaFileName)) return;
            fullLuaFileName = fullLuaFileName.ReplaceEx('\\', '/');
            _luaClassName = fullLuaFileName.Substring(fullLuaFileName.LastIndexOf('/') + 1);

            if (string.IsNullOrEmpty(_luaClassName)) return;

            luaState.Require(fullLuaFileName);
            _luaTable = luaState.GetTable(_luaClassName);

            update = _luaTable.RawGetLuaFunction("Update");
            lateUpdate = _luaTable.RawGetLuaFunction("LateUpdate");
            fixedUpdate = _luaTable.RawGetLuaFunction("FixedUpdate");
            onTriggerEnterFunc = _luaTable.RawGetLuaFunction("OnTriggerEnter");
            onTriggerStayFunc = _luaTable.RawGetLuaFunction("OnTriggerStay");
            onTriggerExitFunc = _luaTable.RawGetLuaFunction("OnTriggerExit");

            if (isMulti)
            {
                LuaTable table = _luaTable.Invoke<LuaTable>("New");

                if (table == null)
                {
                    throw new LuaException(_luaClassName + " does not have a New function, GameObject is" + gameObject.name);
                }

                _luaTable.Dispose();
                _luaTable = table;
            }

            LuaFunction luafunc = luaState.GetFunction("luaObjMgr.AddLuaUIObject");
            luafunc.Call(gameObject, _luaTable);
            _luaTable.name = _luaClassName;
            _luaTable.RawSet("gameObject", gameObject);
            _luaTable.RawSet("transform", transform);
            _luaTable.RawSet("ID", ID);
            _luaTable.Call("Awake", _luaTable);
            isFinishLuaInit = true;
        }
        catch (Exception e)
        {
            luaState.ThrowLuaException(e);
        }
    }

    void InitUpdateEvent()
    {
        LuaLooper loop = LuaClient.Instance.GetLooper();
        if (update != null)
        {
            loop.UpdateEvent.Add(update, _luaTable);
        }

        if (lateUpdate != null)
        {
            loop.LateUpdateEvent.Add(lateUpdate, _luaTable);
        }

        if (fixedUpdate != null)
        {
            loop.FixedUpdateEvent.Add(fixedUpdate, _luaTable);
        }
    }

    void RemoveUpdateEvent()
    {
        LuaClient client = LuaClient.Instance;
        if (LuaClient.Instance)
        {
            LuaLooper loop = client.GetLooper();
            loop.UpdateEvent.Remove(update, _luaTable);
            loop.LateUpdateEvent.Remove(lateUpdate, _luaTable);
            loop.FixedUpdateEvent.Remove(fixedUpdate, _luaTable);
        }
    }

    void ReleaseLua()
    {
        SafeRelease(ref update);
        SafeRelease(ref lateUpdate);
        SafeRelease(ref fixedUpdate);
        SafeRelease(ref _luaTable);
        SafeRelease(ref onTriggerEnterFunc);
        SafeRelease(ref onTriggerStayFunc);
        SafeRelease(ref onTriggerExitFunc);
    }

    void SafeRelease(ref LuaFunction func)
    {
        if (func != null)
        {
            func.Dispose();
            func = null;
        }
    }

    void SafeRelease(ref LuaTable table)
    {
        if (table != null)
        {
            table.Dispose();
            table = null;
        }
    }

    //释放所有引用，程序退出时调用
    public static void DisposeAll()
    {
        foreach (LuaBase ui in uiList)
        {
            GameObject go = ui.gameObject;
            ui.isClose = true;
            ui.ReleaseLua();
            Destroy(go);
        }
    }
}