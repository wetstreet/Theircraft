using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWheat : NBTCrops
{
    public override string name => "Wheat";
    public override string id => "minecraft:wheat";

    public override string GetIconPathByData(short data) { return "wheat"; }

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        return "wheat_stage_7";
    }

    public override string GetBreakEffectTexture(byte data) { return "wheat_stage_7"; }
}
