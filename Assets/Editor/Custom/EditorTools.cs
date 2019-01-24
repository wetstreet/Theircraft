#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Theircraft;
using System;

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

    [MenuItem("Assets/Generate Block Prefabs")]
    public static void GenerateBlockPrefabs()
    {
        foreach (CSBlockType blockType in Enum.GetValues(typeof(CSBlockType)))
        {
            if (blockType != CSBlockType.None)
            {
                BlockGenerator.GenerateBlockPrefab(blockType);
            }
        }
    }

}

#endif