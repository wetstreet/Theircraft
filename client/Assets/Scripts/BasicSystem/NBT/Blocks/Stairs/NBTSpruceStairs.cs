using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSpruceStairs : NBTStairs
{
    public override string name { get { return "Spruce Wood Stairs"; } }
    public override string id { get { return "minecraft:spruce_stairs"; } }

    public override string stairsName { get { return "planks_spruce"; } }

    public override short burningTime => 300;

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_spruce"; }
}
