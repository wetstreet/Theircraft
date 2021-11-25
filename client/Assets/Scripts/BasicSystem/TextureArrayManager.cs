using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureArrayManager
{
    static Texture2DArray array;

    static string[] usedTextures = new string[]
    {
        "mushroom_brown",
        "mushroom_red",

        "mushroom_block_skin_brown", "mushroom_block_inside", "mushroom_block_skin_stem",
        "mushroom_block_skin_red", "mushroom_block_inside", "mushroom_block_skin_stem",

        "carrots_stage_3",
        "potatoes_stage_3",
        "wheat_stage_7",

        "deadbush",
        "farmland_dry", "farmland_wet",
        "grass_top", "grass_side", "dirt", "snow", "grass_side_snowed",
        "ladder",

        "double_plant_grass_bottom",
        "double_plant_grass_top",
        "double_plant_fern_bottom",
        "double_plant_fern_top",
        "double_plant_syringa_bottom",
        "double_plant_syringa_top",
        "double_plant_rose_bottom",
        "double_plant_rose_top",
        "double_plant_paeonia_bottom",
        "double_plant_paeonia_top",

        "leaves_oak", "leaves_spruce", "leaves_birch", "leaves_jungle",
        "leaves_acacia", "leaves_big_oak",
        "log_oak_top", "log_oak", "log_spruce_top", "log_spruce", "log_birch_top", "log_birch", "log_jungle_top", "log_jungle",
        "log_acacia_top", "log_acacia", "log_big_oak_top", "log_big_oak",

        "stone", "cobblestone",
        "door_wood_lower", "door_wood_upper",
        "planks_oak", "planks_spruce", "planks_birch", "planks_jungle", "planks_acacia", "planks_big_oak",

        "flower_rose", "flower_houstonia", "flower_tulip_red", "flower_tulip_orange", "flower_tulip_pink", "flower_tulip_white", "flower_oxeye_daisy",
        "flower_dandelion", "flower_oxeye_daisy", "flower_houstonia",

        "sapling_oak", "sapling_spruce", "sapling_birch", "sapling_jungle",
        "reeds",
        "tallgrass",

        "wool_colored_white",
        "wool_colored_orange",
        "wool_colored_magenta",
        "wool_colored_light_blue",
        "wool_colored_yellow",
        "wool_colored_lime",
        "wool_colored_pink",
        "wool_colored_gray",
        "wool_colored_silver",
        "wool_colored_cyan",
        "wool_colored_purple",
        "wool_colored_blue",
        "wool_colored_brown",
        "wool_colored_green",
        "wool_colored_red",
        "wool_colored_black",
};

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
            foreach (string name in usedTextures)
            {
                AddTexture(name);
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
