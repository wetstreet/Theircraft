using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSpruceFence : NBTFence
{
    public override string fenceName { get { return "planks_spruce"; } }
    public override string name { get { return "Spruce Fence"; } }
    public override string id { get { return "minecraft:spruce_fence"; } }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_spruce"; }
}
