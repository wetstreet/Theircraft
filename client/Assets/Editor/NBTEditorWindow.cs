using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NBTEditorWindow : EditorWindow
{
    [MenuItem("Window/NBT Editor Window")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(NBTEditorWindow));
        window.Show();
    }

    Vector3Int pos;
    byte type;
    byte data;
    private void OnGUI()
    {
        pos = EditorGUILayout.Vector3IntField("pos", pos);
        GUILayout.Label("type=" + type);
        GUILayout.Label("data=" + data);
        if (GUILayout.Button("Update"))
        {
            NBTHelper.GetBlockData(pos.x, pos.y, pos.z, ref type, ref data);
        }
    }
}
