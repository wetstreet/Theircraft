using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTIronOre : NBTBlock
{
    public override string name => "Iron Ore";
    public override string id => "minecraft:iron_ore";

    public override string allName => "iron_ore";

    public override string smeltResult => "minecraft:iron_ingot";

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockII;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "iron_ore"; }
}
