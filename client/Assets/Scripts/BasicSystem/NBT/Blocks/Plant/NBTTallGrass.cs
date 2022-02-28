using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTTallGrass : NBTPlant
{
    public override string name => "Grass";
    public override string id => "minecraft:tallgrass";

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data) { return "tallgrass"; }

    protected override Color GetTintColorByData(NBTChunk chunk, byte data)
    {
        return TintManager.tintColor;
    }

    protected override string itemMeshPath => "grass";

    public override string GetIconPathByData(short data) { return "tallgrass"; }

    public override string GetBreakEffectTexture(byte data) { return "tallgrass"; }
}