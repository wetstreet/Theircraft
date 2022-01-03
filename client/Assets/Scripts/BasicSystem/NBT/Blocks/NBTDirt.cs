using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDirt : NBTBlock
{
    public override string name => "Dirt";
    public override string id => "minecraft:dirt";

    public override string allName => "dirt";

    public override float hardness => 0.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;
    public override SoundMaterial soundMaterial => SoundMaterial.Gravel;

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }
}
