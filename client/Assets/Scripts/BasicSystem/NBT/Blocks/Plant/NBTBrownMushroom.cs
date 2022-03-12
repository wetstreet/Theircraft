using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBrownMushroom : NBTPlant
{
    public override string name => "Brown Mushroom";
    public override string id => "minecraft:brown_mushroom";

    public override byte GetDropItemData(byte data)
    {
        return 0;
    }

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data) { return "mushroom_brown"; }

    public override string GetBreakEffectTexture(byte data) { return "mushroom_brown"; }
}