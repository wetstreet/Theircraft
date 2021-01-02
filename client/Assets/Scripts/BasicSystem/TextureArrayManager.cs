using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureArrayManager
{
    static Texture2DArray array;

    static Texture2D[] GetTextures(int size)
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("GUI/block");

        List<Texture2D> list = new List<Texture2D>();

        Debug.Log(textures.Length);
        foreach (Texture2D tex in textures)
        {
            if (tex.width == size && tex.height == size)
            {
                list.Add(tex);
            }
            //Debug.Log(tex.name + ",width=" + tex.width + ",height=" + tex.height + ",mip=" + tex.mipmapCount);
        }

        return list.ToArray();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Array/Init")]
#endif
    public static void InitArray()
    {
        int size = 16;
        Texture2D[] textures = GetTextures(size);
        CreateArray(textures, size);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Array/Test")]
#endif
    public static void Test()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Custom/TextureArrayShader"));
        mat.SetTexture("_Array", array);
        mr.material = mat;
    }

    static void CreateArray(Texture2D[] textures, int size)
    {
        Texture2DArray newArray = new Texture2DArray(size, size, textures.Length, TextureFormat.ARGB32, true);
        newArray.filterMode = FilterMode.Point;

        try
        {
            int mip = GetMipCountBySize(size);
            //RenderTexture rt = new RenderTexture(size, size, 0, RenderTextureFormat.ARGB32, mip);
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
