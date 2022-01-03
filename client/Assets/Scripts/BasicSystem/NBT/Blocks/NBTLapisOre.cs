using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLapisOre : NBTBlock
{
    public override string name => "Lapis Ore";
    public override string id => "minecraft:lapis_ore";

    public override string GetDropItemByData(byte data) { return "minecraft:lapis_lazuli"; }

    public override string allName => "lapis_ore";

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockII;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "lapis_ore"; }
}
