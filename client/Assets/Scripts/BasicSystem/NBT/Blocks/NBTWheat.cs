using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWheat : NBTCrops
{
    public override string name { get { return "Wheat"; } }
    public override string id { get { return "minecraft:wheat"; } }

    public override string GetIconPathByData(short data) { return "wheat"; }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("wheat_stage_7");
    }

    public override string GetBreakEffectTexture(byte data) { return "wheat_stage_7"; }
}
