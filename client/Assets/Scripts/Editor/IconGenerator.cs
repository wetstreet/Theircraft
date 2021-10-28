using protocol.cs_theircraft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IconGenerator : ScriptableWizard
{
    public byte type = 2;
    public byte data = 0;

    [MenuItem("GameObject/Create Icon Wizard")]
    static void CreateWizard()
    {
        NBTGeneratorManager.Init();
        TextureArrayManager.Init();
        DisplayWizard<IconGenerator>("创建icon", "创建", "批量创建");
    }

    public int size = 128;

    string Render(NBTBlock generator, byte data = 0, string dir = null, bool destroyAfterFinish = false)
    {
        Mesh mesh = generator.GetItemMesh(data);

        GameObject go = new GameObject();
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = generator.GetItemMaterial(0);

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
            path = EditorUtility.SaveFilePanel("保存", "", generator.GetIconPathByData(data), "png");
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
            path = dir + "/" + generator.GetIconPathByData(data) + ".png";
            path = path.Substring(path.IndexOf("Assets"));
        }

        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        TextureImporter TextureI = AssetImporter.GetAtPath(path) as TextureImporter;
        TextureI.textureCompression = TextureImporterCompression.Uncompressed;
        TextureI.mipmapEnabled = false;
        TextureI.wrapMode = TextureWrapMode.Clamp;
        TextureI.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        AssetDatabase.Refresh();

        DestroyImmediate(tex);
        if (destroyAfterFinish)
        {
            DestroyImmediate(go);
        }

        return path;
    }

    void OnWizardCreate()
    {
        NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(type);
        if (generator != null)
        {
            string path = Render(generator, data);

            if (!string.IsNullOrEmpty(path))
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                Selection.activeObject = tex;
            }
        }
    }

    private void OnWizardOtherButton()
    {
        string dir = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath + "Assets/Resources/GUI/icon", "");
        dir = dir.Substring(dir.IndexOf("Assets"));

        Render(NBTGeneratorManager.GetMeshGenerator(1), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(2), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(3), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(4), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(5), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(5), 1, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(5), 2, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(5), 3, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(7), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(12), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(13), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(14), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(15), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(16), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(17), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(17), 1, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(17), 2, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(17), 3, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(18), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(18), 1, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(18), 2, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(18), 3, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(20), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(21), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(24), 0, dir, true);
        //Render(NBTGeneratorManager.GetMeshGenerator(26), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 1, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 2, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 3, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 4, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 5, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 6, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 7, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 8, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 9, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 10, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 11, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 12, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 13, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 14, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(35), 15, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(45), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(49), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(53), 0, dir, true);
        //Render(NBTGeneratorManager.GetMeshGenerator(54), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(56), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(58), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(67), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(73), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(81), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(82), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(85), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(108), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(125), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(126), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(134), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(135), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(136), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(161), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(161), 1, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(162), 0, dir, true);
        Render(NBTGeneratorManager.GetMeshGenerator(162), 1, dir, true);

    }

    void OnWizardUpdate()
    {

    }
}
