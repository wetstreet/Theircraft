using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    static List<Action> callbacks = new List<Action>();

	// Use this for initialization
	void Start () {
        RegisterCallback(SettingsPanel.HandleInput);
        RegisterCallback(DebugUI.HandleInput);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
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
    

    public static void RegisterCallback(Action callback)
    {
        callbacks.Add(callback);
    }
}
