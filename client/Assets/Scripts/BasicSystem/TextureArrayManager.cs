using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureArrayManager
{
    static Texture2DArray array;

    public static int GetIndexByName(string name)
    {
        if (name2index.ContainsKey(name))
        {
            return name2index[name];
        }
        return 0;
    }

    static Dictionary<string, int> name2index;
    static List<Texture2D> textureList;

    static void AddTexture(string name)
    {
        if (!string.IsNullOrEmpty(name) && !name2index.ContainsKey(name))
        {
            Texture2D tex = Resources.Load<Texture2D>("GUI/block/" + name);
            if (tex == null)
            {
                Debug.Log("add texture is null! name = " + name);
            }
            name2index.Add(name, textureList.Count);
            textureList.Add(tex);
        }
    }

    static Texture2D[] GetTextures(int size)
    {
        foreach (NBTBlock generator in NBTGeneratorManager.generatorDict.Values)
        {
            if (generator.UsedTextures != null)
            {
                foreach (string name in generator.UsedTextures)
                {
                    AddTexture(name);
                }
            }
            AddTexture(generator.frontName);
            AddTexture(generator.backName);
            AddTexture(generator.topName);
            AddTexture(generator.bottomName);
            AddTexture(generator.leftName);
            AddTexture(generator.rightName);
        }

        return textureList.ToArray();
    }
    
    public static void Init()
    {
        name2index = new Dictionary<string, int>();
        textureList = new List<Texture2D>();

        int size = 16;
        Texture2D[] textures = GetTextures(size);
        CreateArray(textures, size);
    }

    public static void Uninit()
    {
        if (array != null)
        {
            Object.DestroyImmediate(array);
            array = null;
        }

        name2index = null;
        textureList = null;
    }

    static void CreateArray(Texture2D[] textures, int size)
    {
        Texture2DArray newArray = new Texture2DArray(size, size, textures.Length, TextureFormat.ARGB32, true);
        newArray.filterMode = FilterMode.Point;
        newArray.wrapMode = TextureWrapMode.Clamp;
        newArray.anisoLevel = 3;

        try
        {
            int mip = GetMipCountBySize(size);
            RenderTexture rt = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32);
            Texture2D temp = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true);

            for (int i = 0; i < textures.Length; i++)
            {
                Texture2D tex = textures[i];

                Graphics.Blit(tex, rt);

                RenderTexture.active = rt;
                temp.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                temp.Apply(true);

                for (int j = 0; j < mip; j++)
                {
                    Graphics.CopyTexture(temp, 0, j, newArray, i, j);
                }
            }
            RenderTexture.ReleaseTemporary(rt);
            Object.DestroyImmediate(temp);
            temp = null;

            if (array != null)
            {
                Object.DestroyImmediate(array);
            }
            array = newArray;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    static Texture2D toTexture2D(RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply(true);
        return tex;
    }

    public static Texture2DArray GetArray()
    {
        return array;
    }

    static int GetMipCountBySize(int size)
    {
        return (int)Mathf.Log(size, 2) + 1;
    }
}
