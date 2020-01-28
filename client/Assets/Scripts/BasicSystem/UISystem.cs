using UnityEngine;
using UnityEditor;

public class UISystem : MonoBehaviour
{
    static GameObject canvas;

    static void InitializeCanvas()
    {
        GameObject prefab = Resources.Load("Prefabs/UI/ui_root") as GameObject;
        GameObject uiroot = Instantiate(prefab);
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(uiroot);
        }
        canvas = GameObject.Find("Canvas");
    }

    public static void DestroyUIRoot()
    {
        Debug.Log("canvas =" + canvas);
        DestroyImmediate(canvas.transform.parent.gameObject);
        canvas = null;
    }

    public static GameObject InstantiateUI(string name)
    {
        GameObject prefab = Resources.Load("Prefabs/UI/" + name) as GameObject;
        GameObject uiobj = Instantiate(prefab);
        return InstantiateUI(uiobj);
    }

    public static GameObject InstantiateUI(GameObject uiobj)
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                InitializeCanvas();
            }
        }
        uiobj.transform.SetParent(canvas.transform);
        uiobj.transform.localScale = Vector3.one;

        if (LoadingUI.isLoading)
        {
            LoadingUI.SetOnTop();
        }

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
