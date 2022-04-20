using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGoldOre : NBTBlock
{
    public override string name => "Gold Ore";
    public override string id => "minecraft:gold_ore";

    public override string allName => "gold_ore";

    public override string smeltResult => "minecraft:gold_ingot";

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockIII;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "gold_ore"; }
}
