using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDiamondOre : NBTBlock
{
    public override string name => "Diamond Ore";
    public override string id => "minecraft:diamond_ore";

    public override string allName => "diamond_ore";

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockIII;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetDropItemByData(byte data)
    {
        return "minecraft:diamond";
    }

    public override string GetBreakEffectTexture(byte data) { return "diamond_ore"; }
}
