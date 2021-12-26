using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeOfDay))]
public class TimeOfDayInspector : Editor
{
    public override void OnInspectorGUI()
    {
        TimeOfDay tod = target as TimeOfDay;
        float time = tod.tick / 1000 + 6;
        if (time > 24) time -= 24;
        time = EditorGUILayout.Slider("Time", time, 0, 24);
        time -= 6;
        if (time < 0) time += 24;
        tod.tick = time * 1000;

        DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });
    }
}
