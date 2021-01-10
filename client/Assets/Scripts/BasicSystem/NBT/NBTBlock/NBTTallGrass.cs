using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTTallGrass : NBTPlant
{
    public override string name { get { return "Tall Grass"; } }

    protected override Color GetTintColorByData(NBTChunk chunk, byte data)
    {
        return TintManager.tintColor;
    }

    public override void Init()
    {
        UsedTextures = new string[] { "tallgrass" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("tallgrass");
    }
}