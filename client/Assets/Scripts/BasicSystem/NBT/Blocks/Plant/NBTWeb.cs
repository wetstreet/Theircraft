using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWeb : NBTPlant
{
    public override string name => "Web";
    public override string id => "minecraft:web";

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data) { return "web"; }

    protected override string itemMeshPath => "web";

    public override string GetIconPathByData(short data) { return "web"; }

    public override string GetBreakEffectTexture(byte data) { return "web"; }
}