using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Draws all properties like base.OnInspectorGUI() but excludes a field by name.
    /// </summary>
    /// <param name="fieldToSkip">The name of the field that should be excluded. Example: "m_Script" will skip the default Script field.</param>
    public static void DrawInspectorExcept(this SerializedObject serializedObject, string fieldToSkip)
    {
        serializedObject.DrawInspectorExcept(new string[1] { fieldToSkip });
    }

    /// <summary>
    /// Draws all properties like base.OnInspectorGUI() but excludes the specified fields by name.
    /// </summary>
    /// <param name="fieldsToSkip">
    /// An array of names that should be excluded.
    /// Example: new string[] { "m_Script" , "myInt" } will skip the default Script field and the Integer field myInt.
    /// </param>
    public static void DrawInspectorExcept(this SerializedObject serializedObject, string[] fieldsToSkip)
    {
        serializedObject.Update();
        SerializedProperty prop = serializedObject.GetIterator();
        if (prop.NextVisible(true))
        {
            do
            {
                if (fieldsToSkip.Any(prop.name.Contains))
                    continue;

                EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
            }
            while (prop.NextVisible(false));
        }
        serializedObject.ApplyModifiedProperties();
    }
}
