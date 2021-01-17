using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSapling : NBTPlant
{
    public override string name { get { return "Sapling"; } }
    public override string id { get { return "minecraft:sapling"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "sapling_oak", "sapling_spruce", "sapling_birch", "sapling_jungle" };
    }

    public override string GetIconPathByData(short data)
    {
        if (data == 0) return "sapling_oak";
        else if (data == 1) return "sapling_spruce";
        else if (data == 2) return "sapling_birch";
        else if (data == 3) return "sapling_jungle";
        else return null;
    }

    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    {
        string path;
        switch (data)
        {
            case 0:
                path = "oak_sapling";
                return Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
            case 1:
                path = "spruce_sapling";
                return Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
            case 2:
                path = "birch_sapling";
                return Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
            case 3:
                path = "jungle_sapling";
                return Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
        }
        throw new System.Exception("no mesh");
    }

    public override int GetPlantIndexByData(int data)
    {
        switch (data)
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
        switch (data)
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
