using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBeetroots : NBTCrops
{
    public override string name => "Beetroot";
    public override string id => "minecraft:beetroot";

    public override string GetIconPathByData(short data) { return "beetroot"; }

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        return "beetroots_stage_3";
    }

    public override string GetBreakEffectTexture(byte data) { return "beetroots_stage_3"; }
}
