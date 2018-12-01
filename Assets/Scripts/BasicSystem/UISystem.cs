using UnityEngine;
using UnityEditor;

public static class UISystem
{
    static GameObject canvas;

    static void InitializeCanvas()
    {
        GameObject prefab = Resources.Load("Prefabs/UI/ui_root") as GameObject;
        Object.Instantiate(prefab);
        canvas = GameObject.Find("Canvas");
    }

    public static GameObject InstantiateUI(string name)
    {
        GameObject prefab = Resources.Load("Prefabs/UI/" + name) as GameObject;
        GameObject uiobj = Object.Instantiate(prefab);

        if (canvas == null)
        {
            InitializeCanvas();
        }
        uiobj.transform.SetParent(canvas.transform);
        uiobj.transform.localPosition = Vector3.zero;
        return uiobj;
    }

    #if UNITY_EDITOR
    public static GameObject InstantiatePrefab(GameObject prefab)
    {
        GameObject uiobj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        if (canvas == null)
        {
            InitializeCanvas();
        }
        uiobj.transform.SetParent(canvas.transform);
        uiobj.transform.localPosition = Vector3.zero;
        return uiobj;
    }
    #endif

}
