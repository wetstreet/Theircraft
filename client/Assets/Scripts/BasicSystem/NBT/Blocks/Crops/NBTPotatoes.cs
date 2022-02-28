using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPotatoes : NBTCrops
{
    public override string name => "Potato";
    public override string id => "minecraft:potato";

    public override string GetIconPathByData(short data) { return "potato"; }

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        return "potatoes_stage_3";
    }

    public override string GetBreakEffectTexture(byte data) { return "potatoes_stage_3"; }
}
