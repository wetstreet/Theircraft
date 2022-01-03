using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTEmeraldOre : NBTBlock
{
    public override string name => "Emerald Ore";
    public override string id => "minecraft:emerald_ore";

    public override string allName => "emerald_ore";

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockIII;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "emerald_ore"; }
}
