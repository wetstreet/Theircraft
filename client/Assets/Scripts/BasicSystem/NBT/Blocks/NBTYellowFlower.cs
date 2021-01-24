using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTYellowFlower : NBTPlant
{
    public override string name { get { return "Yellow Flower"; } }
    public override string id { get { return "minecraft:yellow_flower"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "flower_dandelion", "flower_oxeye_daisy", "flower_houstonia" };
    }

    public override int GetPlantIndexByData(int data)
    {
        if (data == 0)
        {
            return TextureArrayManager.GetIndexByName("flower_dandelion");
        }
        else if (data == 3)
        {
            return TextureArrayManager.GetIndexByName("flower_houstonia");
        }
        else if (data == 8)
        {
            return TextureArrayManager.GetIndexByName("flower_oxeye_daisy");
        }
        throw new System.Exception("no index");
    }

    public override string GetBreakEffectTexture(byte data)
    {
        if (data == 0)
        {
            return "flower_dandelion";
        }
        else if (data == 3)
        {
            return "flower_houstonia";
        }
        else if (data == 8)
        {
            return "flower_oxeye_daisy";
        }
        throw new System.Exception("no texture");
    }

    public override string GetIconPathByData(short data)
    {
        if (data == 0)
        {
            return "flower_dandelion";
        }
        else if (data == 3)
        {
            return "flower_houstonia";
        }
        else if (data == 8)
        {
            return "flower_oxeye_daisy";
        }
        throw new System.Exception("no icon");
    }

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Dandelion";
            case 3:
                return "Houstonia";
            case 8:
                return "Oxeye Daisy";
        }
        throw new System.Exception("no name, data=" + data);
    }
}