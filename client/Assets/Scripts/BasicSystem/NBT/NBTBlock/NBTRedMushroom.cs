using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedMushroom : NBTPlant
{
    public override string name { get { return "Red Mushroom"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "mushroom_red" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("mushroom_red");
    }
}