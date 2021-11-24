using protocol.cs_theircraft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IconGenerator : EditorWindow
{
    [MenuItem("Window/Icon Generator")]
    static void Init()
    {
        IconGenerator window = (IconGenerator)GetWindow(typeof(IconGenerator));
        window.Show();
    }

    private void Awake()
    {
        NBTGeneratorManager.Init();
        TextureArrayManager.Init();

        Texture2D day = Resources.Load<Texture2D>("GUI/Day");
        Shader.SetGlobalTexture("_DayLightTexture", day);
        Texture2D night = Resources.Load<Texture2D>("GUI/Night");
        Shader.SetGlobalTexture("_NightLightTexture", night);

        AddGridItem(NBTGeneratorManager.GetMeshGenerator(1), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(2), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(3), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(4), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(5), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(5), 1);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(5), 2);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(5), 3);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(5), 4);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(5), 5);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(7), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(12), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(13), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(14), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(15), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(16), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(17), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(17), 1);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(17), 2);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(17), 3);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(18), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(18), 1);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(18), 2);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(18), 3);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(20), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(21), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(24), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(26), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 1);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 2);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 3);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 4);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 5);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 6);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 7);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 8);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 9);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 10);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 11);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 12);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 13);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 14);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(35), 15);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(45), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(49), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(53), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(54), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(56), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(58), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(67), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(73), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(81), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(82), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(85), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(108), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(125), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(126), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(126), 1);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(126), 2);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(126), 3);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(126), 4);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(126), 5);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(134), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(135), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(136), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(161), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(161), 1);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(162), 0);
        AddGridItem(NBTGeneratorManager.GetMeshGenerator(162), 1);

        optionArray = optionList.ToArray();
    }

    void AddGridItem(NBTBlock generator, byte data)
    {
        generatorList.Add(generator);
        dataList.Add(data);
        optionList.Add(generator.ToString() + ", " + data);
    }

    private void OnDestroy()
    {
        NBTGeneratorManager.Uninit();
        TextureArrayManager.Uninit();
    }

    public byte type = 2;
    public byte data = 0;

    string dir = "Assets/Resources/GUI/icon";

    List<string> optionList = new List<string>();
    List<NBTBlock> generatorList = new List<NBTBlock>();
    List<byte> dataList = new List<byte>();

    string[] optionArray;

    GameObject go;

    int index = 0;
    Vector2 scrollPos;
    private void OnGUI()
    {
        dir = EditorGUILayout.TextField(dir);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        index = GUILayout.SelectionGrid(index, optionArray, 1);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("创建", GUILayout.Height(100)))
        {
            Render(generatorList[index], dataList[index], dir);
        }

        EditorGUILayout.EndHorizontal();
    }

    public int size = 128;

    string Render(NBTBlock generator, byte data = 0, string dir = null)
    {
        Mesh mesh = generator.GetItemMesh(data);

        if (go != null)
        {
            DestroyImmediate(go);
        }

        go = new GameObject();
        go.name = generator.GetIconPathByData(data);
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
            path = EditorUtility.SaveFilePanel("保存", "", go.name, "png");
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
            path = dir + "/" + go.name + ".png";
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

        return path;
    }
}
