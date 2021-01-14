using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDeadBush : NBTPlant
{
    public override string name { get { return "DeadBush"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "deadbush" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("deadbush");
    }
}