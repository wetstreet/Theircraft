using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLapisOre : NBTBlock
{
    public override string name { get { return "Lapis Ore"; } }
    public override string id { get { return "minecraft:lapis_ore"; } }

    public override string GetDropItemByData(byte data) { return "minecraft:lapis_lazuli"; }

    public override string topName { get { return "lapis_ore"; } }
    public override string bottomName { get { return "lapis_ore"; } }
    public override string frontName { get { return "lapis_ore"; } }
    public override string backName { get { return "lapis_ore"; } }
    public override string leftName { get { return "lapis_ore"; } }
    public override string rightName { get { return "lapis_ore"; } }

    public override BlockMaterial blockMaterial => BlockMaterial.RockII;

    public override float hardness => 3;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "lapis_ore"; }
}
