using protocol.cs_theircraft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IconGenerator : ScriptableWizard
{
    public CSBlockType type = CSBlockType.Dirt;

    [MenuItem("GameObject/Create Icon Wizard")]
    static void CreateWizard()
    {
        DisplayWizard<IconGenerator>("创建icon", "创建", "全部创建");
    }

    public int size = 128;

    void Render(CSBlockType type, string dir = null)
    {
        GameObject go = new GameObject();
        go.AddComponent<MeshFilter>().sharedMesh = ChunkMeshGenerator.GetBlockMesh(type);
        go.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = ChunkMeshGenerator.GetBlockTexture(type);

        if (Camera.main.targetTexture == null)
        {
            Camera.main.targetTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
        }
        Camera.main.Render();

        RenderTexture rt = Camera.main.targetTexture;
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, true);
        tex.filterMode = FilterMode.Point;

        //降分辨率
        RenderTexture tempRt = new RenderTexture(tex.width, tex.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(rt, tempRt);

        RenderTexture.active = tempRt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        tempRt.Release();
        tempRt = null;

        string path = "";
        if (dir == null)
        {
            path = EditorUtility.SaveFilePanel("保存", "", "icon", "png");
            if (path.Length == 0)
            {
                return;
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
        DestroyImmediate(go);
    }

    void OnWizardCreate()
    {
        Render(type);
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
