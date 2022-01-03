using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTObsidian : NBTBlock
{
    public override string name => "Obsidian";
    public override string id => "minecraft:obsidian";

    public override string allName => "obsidian";

    public override BlockMaterial blockMaterial => BlockMaterial.RockIV;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "obsidian"; }
}
