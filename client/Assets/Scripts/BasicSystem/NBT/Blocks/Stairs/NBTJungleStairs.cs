using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTJungleStairs : NBTStairs
{
    public override string name { get { return "Jungle Wood Stairs"; } }
    public override string id { get { return "minecraft:jungle_stairs"; } }

    public override string stairsName { get { return "planks_jungle"; } }

    public override short burningTime => 300;

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_jungle"; }
}
