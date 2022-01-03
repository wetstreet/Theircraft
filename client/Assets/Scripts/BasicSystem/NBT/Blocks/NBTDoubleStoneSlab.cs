using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDoubleStoneSlab : NBTBlock
{
    public override string name => "Double Stone Slab";
    public override string id => "minecraft:double_stone_slab";

    public override string topName => "stone_slab_top";
    public override string bottomName => "stone_slab_top";
    public override string frontName => "stone_slab_side";
    public override string backName => "stone_slab_side";
    public override string leftName => "stone_slab_side";
    public override string rightName => "stone_slab_side";

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "stone_slab_side"; }
}
