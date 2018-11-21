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
        foreach (BlockType blockType in Enum.GetValues(typeof(BlockType)))
        {
            if (blockType != BlockType.None)
            {
                BlockGenerator.GenerateBlockPrefab(blockType);
            }
        }
    }

}

#endif