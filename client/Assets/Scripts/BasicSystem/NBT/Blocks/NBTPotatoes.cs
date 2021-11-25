using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPotatoes : NBTCrops
{
    public override string name { get { return "Potato"; } }
    public override string id { get { return "minecraft:potato"; } }

    public override string GetIconPathByData(short data) { return "potato"; }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("potatoes_stage_3");
    }

    public override string GetBreakEffectTexture(byte data) { return "potatoes_stage_3"; }
}