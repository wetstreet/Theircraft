using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCarrots : NBTCrops
{
    public override string name { get { return "Carrot"; } }
    public override string id { get { return "minecraft:carrot"; } }

    public override string GetIconPathByData(short data) { return "carrot"; }

    public override void Init()
    {
        UsedTextures = new string[] { "carrots_stage_3" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("carrots_stage_3");
    }

    public override string GetBreakEffectTexture(byte data) { return "carrots_stage_3"; }
}
