using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedMushroom : NBTPlant
{
    public override string name => "Red Mushroom";
    public override string id => "minecraft:red_mushroom";
    protected override int size => 6;
    protected override int height => 8;

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        return "mushroom_red";
    }

    public override string GetBreakEffectTexture(byte data) { return "mushroom_red"; }
}