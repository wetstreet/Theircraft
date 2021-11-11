using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDiamondOre : NBTBlock
{
    public override string name { get { return "DiamondOre"; } }
    public override string id { get { return "minecraft:diamond_ore"; } }

    public override string topName { get { return "diamond_ore"; } }
    public override string bottomName { get { return "diamond_ore"; } }
    public override string frontName { get { return "diamond_ore"; } }
    public override string backName { get { return "diamond_ore"; } }
    public override string leftName { get { return "diamond_ore"; } }
    public override string rightName { get { return "diamond_ore"; } }

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockIII;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "diamond_ore"; }
}
