using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTIronOre : NBTBlock
{
    public override string name { get { return "Iron Ore"; } }
    public override string id { get { return "minecraft:iron_ore"; } }

    public override string topName { get { return "iron_ore"; } }
    public override string bottomName { get { return "iron_ore"; } }
    public override string frontName { get { return "iron_ore"; } }
    public override string backName { get { return "iron_ore"; } }
    public override string leftName { get { return "iron_ore"; } }
    public override string rightName { get { return "iron_ore"; } }

    public override float hardness => 3;

    public override BlockMaterial blockMaterial => BlockMaterial.RockII;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "iron_ore"; }
}
