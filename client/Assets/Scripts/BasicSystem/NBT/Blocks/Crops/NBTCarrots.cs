using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCarrots : NBTCrops
{
    public override string name => "Carrot";
    public override string id => "minecraft:carrot";

    public override string GetIconPathByData(short data) { return "carrot"; }

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        return "carrots_stage_3";
    }

    public override string GetBreakEffectTexture(byte data) { return "carrots_stage_3"; }
}
