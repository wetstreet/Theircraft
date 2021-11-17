using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGoldOre : NBTBlock
{
    public override string name { get { return "Gold Ore"; } }
    public override string id { get { return "minecraft:gold_ore"; } }

    public override string topName { get { return "gold_ore"; } }
    public override string bottomName { get { return "gold_ore"; } }
    public override string frontName { get { return "gold_ore"; } }
    public override string backName { get { return "gold_ore"; } }
    public override string leftName { get { return "gold_ore"; } }
    public override string rightName { get { return "gold_ore"; } }

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockIII;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "gold_ore"; }
}
