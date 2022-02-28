using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDeadBush : NBTPlant
{
    public override string name => "Dead Bush";

    public override string id => "minecraft:deadbush";

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data) { return "deadbush"; }

    public override string GetBreakEffectTexture(byte data) { return "deadbush"; }
}