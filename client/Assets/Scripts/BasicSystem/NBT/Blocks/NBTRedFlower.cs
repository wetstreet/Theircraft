using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedFlower : NBTPlant
{
    public override string name { get { return "Red Flower"; } }
    public override string id { get { return "minecraft:red_flower"; } }
    public override float topOffset => 0.1875f;
    public override float radius => 0.25f;
    public override bool useRadius => true;
    protected override int size => 4;
    protected override int height => 12;

    public override void Init()
    {
        UsedTextures = new string[] { "flower_rose", "flower_houstonia", "flower_tulip_red", "flower_tulip_orange", "flower_tulip_white", "flower_oxeye_daisy" };
    }

    public override int GetPlantIndexByData(int data)
    {
        if (data == 0)
        {
            return TextureArrayManager.GetIndexByName("flower_rose");
        }
        else if (data == 3)
        {
            return TextureArrayManager.GetIndexByName("flower_houstonia");
        }
        else if (data == 4)
        {
            return TextureArrayManager.GetIndexByName("flower_tulip_red");
        }
        else if (data == 5)
        {
            return TextureArrayManager.GetIndexByName("flower_tulip_orange");
        }
        else if (data == 6)
        {
            return TextureArrayManager.GetIndexByName("flower_houstonia");
        }
        else if (data == 8)
        {
            return TextureArrayManager.GetIndexByName("flower_oxeye_daisy");
        }
        throw new System.Exception("no index, data=" + data);
    }

    public override string GetBreakEffectTexture(byte data)
    {
        if (data == 0)
        {
            return "flower_rose";
        }
        else if (data == 3)
        {
            return "flower_houstonia";
        }
        else if (data == 4)
        {
            return "flower_tulip_red";
        }
        else if (data == 5)
        {
            return "flower_tulip_orange";
        }
        else if (data == 6)
        {
            return "flower_houstonia";
        }
        else if (data == 8)
        {
            return "flower_oxeye_daisy";
        }
        Debug.Log("red flower no break effect texture, data=" + data);
        return "flower_rose";
    }

    public override string GetIconPathByData(short data)
    {
        if (data == 0)
        {
            return "flower_rose";
        }
        else if (data == 3)
        {
            return "flower_houstonia";
        }
        else if (data == 4)
        {
            return "flower_tulip_red";
        }
        else if (data == 5)
        {
            return "flower_tulip_orange";
        }
        else if (data == 6)
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
                return "Rose";
            case 3:
                return "Houstonia";
            case 4:
                return "Red Tulip";
            case 5:
                return "Orange Tulip";
            case 6:
                return "Houstonia";
            case 8:
                return "Oxeye Daisy";
        }
        throw new System.Exception("no name, data=" + data);
    }
}