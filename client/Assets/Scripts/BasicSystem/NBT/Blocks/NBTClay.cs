using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTClay : NBTBlock
{
    public override string name => "Clay";
    public override string id => "minecraft:clay";

    public override string allName => "clay";

    public override float hardness => 0.6f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;
    public override SoundMaterial soundMaterial => SoundMaterial.Gravel;

    public override string GetBreakEffectTexture(byte data) { return "clay"; }
}
