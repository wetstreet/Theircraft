using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    static List<Action> callbacks;
    public static bool enabled = true;

	// Use this for initialization
	void Start () {
        callbacks = new List<Action>();
        enabled = true;

        RegisterCallback(SettingsPanel.HandleInput);
        RegisterCallback(DebugUI.HandleInput);
        RegisterCallback(InventorySystem.HandleInput);
        RegisterCallback(ItemSelectPanel.instance.HandleInput);
    }
	
	// Update is called once per frame
	void Update () {
        if (enabled)
        {
            foreach (Action callback in callbacks)
            {
                callback();
            }
        }
	}

    static GameObject instance;

    public static void Init()
    {
        if (instance != null)
        {
            return;
        }

        instance = new GameObject("InputManager");
        instance.AddComponent<InputManager>();
        DontDestroyOnLoad(instance);
    }

    public static void Destroy()
    {
        Destroy(instance);
        callbacks = null;
    }
    

    public static void RegisterCallback(Action callback)
    {
        callbacks.Add(callback);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && enabled)
        {
            SettingsPanel.Show();
        }
    }
}
