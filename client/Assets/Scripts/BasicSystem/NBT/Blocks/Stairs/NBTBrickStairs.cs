using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBrickStairs : NBTStairs
{
    public override string name { get { return "Brick Stairs"; } }
    public override string id { get { return "minecraft:brick_stairs"; } }

    public override string stairsName { get { return "brick"; } }

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "brick"; }
}
