using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTOakStairs : NBTStairs
{
    public override string name { get { return "Oak Wood Stairs"; } }
    public override string id { get { return "minecraft:oak_stairs"; } }

    public override string stairsName { get { return "planks_oak"; } }

    public override float hardness => 2;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_oak"; }
}
