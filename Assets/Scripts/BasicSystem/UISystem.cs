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
        return InstantiateUI(uiobj);
    }

    public static GameObject InstantiateUI(GameObject uiobj)
    {
        if (canvas == null)
        {
            InitializeCanvas();
        }
        uiobj.transform.SetParent(canvas.transform);
        RectTransform rt = uiobj.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        return uiobj;
    }

#if UNITY_EDITOR
    public static GameObject InstantiatePrefab(GameObject prefab)
    {
        GameObject uiobj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        return InstantiateUI(uiobj);
    }
    #endif

}
