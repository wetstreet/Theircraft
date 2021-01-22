using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTSapling : NBTPlant
{
    public override string name { get { return "Sapling"; } }
    public override string id { get { return "minecraft:sapling"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "sapling_oak", "sapling_spruce", "sapling_birch", "sapling_jungle" };
    }

    public override Material GetItemMaterial(byte data)
    {
        byte index = (byte)(data % 4);
        if (!itemMaterialDict.ContainsKey(index))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>("GUI/icon/" + GetIconPathByData(index));
            mat.mainTexture = tex;
            itemMaterialDict.Add(index, mat);
        }
        return itemMaterialDict[index];
    }

    public override string GetIconPathByData(short data)
    {
        switch (data % 4)
        {
            case 0:
                return "sapling_oak";
            case 1:
                return "sapling_spruce";
            case 2:
                return "sapling_birch";
            case 3:
                return "sapling_jungle";
        }
        return null;
    }

    public override bool hasDropItem { get { return true; } }

    public override int GetPlantIndexByData(int data)
    {
        switch (data % 4)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("sapling_oak");
            case 1:
                return TextureArrayManager.GetIndexByName("sapling_spruce");
            case 2:
                return TextureArrayManager.GetIndexByName("sapling_birch");
            case 3:
                return TextureArrayManager.GetIndexByName("sapling_jungle");
        }
        throw new System.Exception("no index");
    }

    public override string GetBreakEffectTexture(byte data)
    {
        switch (data % 4)
        {
            case 0:
                return "sapling_oak";
            case 1:
                return "sapling_spruce";
            case 2:
                return "sapling_birch";
            case 3:
                return "sapling_jungle";
        }
        Debug.Log("no break effect texture, data=" + data);
        return "sapling_oak";
    }
}
