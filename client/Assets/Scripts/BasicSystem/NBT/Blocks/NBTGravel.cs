using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGravel : NBTBlock
{
    public override string name => "Gravel";
    public override string id => "minecraft:gravel";

    public override string allName => "gravel";

    public override float hardness => 0.6f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;
    public override SoundMaterial soundMaterial => SoundMaterial.Gravel;

    public override string GetBreakEffectTexture(byte data) { return "gravel"; }
}
