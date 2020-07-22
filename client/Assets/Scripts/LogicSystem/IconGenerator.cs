using protocol.cs_theircraft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IconGenerator : ScriptableWizard
{
    public CSBlockType type = CSBlockType.Dirt;
    public int TypeInt = 1;
    public bool UseInt = false;

    public bool DestroyAfterFinish = true;

    [MenuItem("GameObject/Create Icon Wizard")]
    static void CreateWizard()
    {
        DisplayWizard<IconGenerator>("创建icon", "创建", "全部创建");
    }

    public int size = 128;

    string Render(CSBlockType type, string dir = null)
    {
        GameObject go = null;
        if (type != CSBlockType.Chest)
        {
            go = new GameObject();
            go.AddComponent<MeshFilter>().sharedMesh = ChunkMeshGenerator.GetBlockMesh(type);
            go.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Custom/CutoutDiffuse"));
            go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = ChunkMeshGenerator.GetBlockTexture(type);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Entity/Chest");
            go = Instantiate(prefab);
        }

        RenderTexture rt = RenderTexture.GetTemporary(size * 2, size * 2, 24, RenderTextureFormat.ARGB32);
        Camera.main.targetTexture = rt;
        Camera.main.Render();
        Camera.main.targetTexture = null;
        
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, true);

        //降分辨率
        RenderTexture tempRt = RenderTexture.GetTemporary(tex.width, tex.height, 24, RenderTextureFormat.ARGB32);
        Graphics.Blit(rt, tempRt);

        RenderTexture.active = tempRt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        rt.Release();
        tempRt.Release();

        string path = "";
        if (dir == null)
        {
            path = EditorUtility.SaveFilePanel("保存", "", type.ToString(), "png");
            if (path.Length == 0)
            {
                DestroyImmediate(tex);
                DestroyImmediate(go);
                return null;
            }
            path = path.Substring(path.IndexOf("Assets"));
        }
        else
        {
            path = dir + "/" + type.ToString() + ".png";
            path = path.Substring(path.IndexOf("Assets"));
        }

        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        TextureImporter TextureI = AssetImporter.GetAtPath(path) as TextureImporter;
        TextureI.textureCompression = TextureImporterCompression.Uncompressed;
        TextureI.mipmapEnabled = false;
        TextureI.wrapMode = TextureWrapMode.Clamp;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        AssetDatabase.Refresh();

        DestroyImmediate(tex);
        if (DestroyAfterFinish)
        {
            DestroyImmediate(go);
        }

        return path;
    }

    void OnWizardCreate()
    {
        string path = null;

        if (UseInt)
        {
            path = Render((CSBlockType)TypeInt);
        }
        else
        {
            path = Render(type);
        }

        if (path != null)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Selection.activeObject = tex;
        }
    }

    private void OnWizardOtherButton()
    {
        string dir = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
        dir = dir.Substring(dir.IndexOf("Assets"));

        var types = Enum.GetValues(typeof(CSBlockType));
        for (int i = 0; i < types.Length; i++)
        {
            Render((CSBlockType)types.GetValue(i), dir);
            EditorUtility.DisplayProgressBar("提示", "创建中.." + i + "/" + types.Length, (float)i / types.Length);
        }
        EditorUtility.ClearProgressBar();
    }

    void OnWizardUpdate()
    {

    }
}
