using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    Survival,
    Creative
}

public class GameModeManager : MonoBehaviour
{
    static GameMode _mode;
    public static GameMode mode
    {
        get { return _mode; }
    }

    public static bool isCreative
    {
        get { return _mode == GameMode.Creative; }
    }

    public static bool isSurvival
    {
        get { return _mode == GameMode.Survival; }
    }

    public static void SetSurvival()
    {
        _mode = GameMode.Survival;
        ItemSelectPanel.instance.RefreshStatus();
    }

    public static void SetCreative()
    {
        _mode = GameMode.Creative;
        ItemSelectPanel.instance.RefreshStatus();
    }
}
