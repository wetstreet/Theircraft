using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBrick : NBTBlock
{
    public override string name => "Brick";
    public override string id => "minecraft:brick_block";

    public override float hardness => 2;

    public override string allName => "brick";

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "brick"; }
}
