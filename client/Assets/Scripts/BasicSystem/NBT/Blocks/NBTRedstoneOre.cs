using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedstoneOre : NBTBlock
{
    public override string name { get { return "RedstoneOre"; } }
    public override string id { get { return "minecraft:redstone_ore"; } }

    public override string GetDropItemByData(byte data) { return "minecraft:redstone"; }

    public override string topName { get { return "redstone_ore"; } }
    public override string bottomName { get { return "redstone_ore"; } }
    public override string frontName { get { return "redstone_ore"; } }
    public override string backName { get { return "redstone_ore"; } }
    public override string leftName { get { return "redstone_ore"; } }
    public override string rightName { get { return "redstone_ore"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "redstone_ore"; }
}
