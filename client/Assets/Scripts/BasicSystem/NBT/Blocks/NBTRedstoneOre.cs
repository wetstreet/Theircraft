using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedstoneOre : NBTBlock
{
    public override string name => "Redstone Ore";
    public override string id => "minecraft:redstone_ore";

    public override string GetDropItemByData(byte data) { return "minecraft:redstone"; }

    public override string allName => "redstone_ore";

    public override BlockMaterial blockMaterial => BlockMaterial.RockIII;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override float hardness => 3;

    public override string GetBreakEffectTexture(byte data) { return "redstone_ore"; }
}
