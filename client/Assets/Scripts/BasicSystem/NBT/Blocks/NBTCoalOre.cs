using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCoalOre : NBTBlock
{
    public override string name => "Coal Ore";
    public override string id => "minecraft:coal_ore";

    public override string allName => "coal_ore";

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetDropItemByData(byte data) { return "minecraft:coal"; }

    public override string GetBreakEffectTexture(byte data) { return "coal_ore"; }
}
