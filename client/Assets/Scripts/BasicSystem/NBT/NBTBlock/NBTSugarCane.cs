using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSugarCane : NBTPlant
{
    public override string name { get { return "Sugar Cane"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "reeds" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("reeds");
    }
}