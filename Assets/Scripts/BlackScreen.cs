using UnityEngine;

public static class BlackScreen
{
    static GameObject _instance;
    static GameObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("Canvas").transform.Find("BlackScreen").gameObject;
            }
            return _instance;
        }
    }

    public static void Show()
    {
        Instance.SetActive(true);
    }

    public static void Hide()
    {
        Instance.SetActive(false);
    }
}