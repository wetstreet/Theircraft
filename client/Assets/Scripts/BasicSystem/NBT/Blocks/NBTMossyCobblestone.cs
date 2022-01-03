using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTMossyCobblestone : NBTBlock
{
    public override string name => "Mossy Cobblestone";
    public override string id => "minecraft:mossy_cobblestone";

    public override string allName => "cobblestone_mossy";

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "cobblestone_mossy"; }
}
