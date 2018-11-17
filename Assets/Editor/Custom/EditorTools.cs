#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class EditorTools
{

    [MenuItem("Assets/Preview UI Prefab")]
    public static void PreviewUIPrefab()
    {
        GameObject prefab = Selection.activeGameObject;
        if (prefab != null)
        {
            UISystem.InstantiatePrefab(prefab);
        }
    }
}

#endif