using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTYellowFlower : NBTPlant
{
    public override string name { get { return "Yellow Flower"; } }

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
}