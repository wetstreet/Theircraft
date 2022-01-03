using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCobblestone : NBTBlock
{
    public override string name => "Cobblestone";
    public override string id => "minecraft:cobblestone";

    public override string allName => "cobblestone";

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "cobblestone"; }
}
