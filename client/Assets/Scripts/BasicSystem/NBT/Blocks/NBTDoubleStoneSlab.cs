using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDoubleStoneSlab : NBTBlock
{
    public override string name { get { return "Double Stone Slab"; } }
    public override string id { get { return "minecraft:double_stone_slab"; } }

    public override string topName { get { return "stone_slab_top"; } }
    public override string bottomName { get { return "stone_slab_top"; } }
    public override string frontName { get { return "stone_slab_side"; } }
    public override string backName { get { return "stone_slab_side"; } }
    public override string leftName { get { return "stone_slab_side"; } }
    public override string rightName { get { return "stone_slab_side"; } }

    public override float hardness { get { return 2; } }

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "stone_slab_side"; }
}
