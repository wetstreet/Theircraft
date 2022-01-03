using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSpruceFence : NBTFence
{
    public override string fenceName => "planks_spruce";
    public override string name => "Spruce Fence";
    public override string id => "minecraft:spruce_fence";

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_spruce"; }
}
