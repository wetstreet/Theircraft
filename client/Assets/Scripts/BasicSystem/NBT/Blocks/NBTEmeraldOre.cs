using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTEmeraldOre : NBTBlock
{
    public override string name { get { return "Emerald Ore"; } }
    public override string id { get { return "minecraft:emerald_ore"; } }

    public override string topName { get { return "emerald_ore"; } }
    public override string bottomName { get { return "emerald_ore"; } }
    public override string frontName { get { return "emerald_ore"; } }
    public override string backName { get { return "emerald_ore"; } }
    public override string leftName { get { return "emerald_ore"; } }
    public override string rightName { get { return "emerald_ore"; } }

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockIII;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "emerald_ore"; }
}
