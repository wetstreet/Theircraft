using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBrownMushroom : NBTPlant
{
    public override string name { get { return "Brown Mushroom"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "mushroom_brown" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("mushroom_brown");
    }
}