using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCoalOre : NBTBlock
{
    public override string name { get { return "CoalOre"; } }
    public override string id { get { return "minecraft:coal_ore"; } }

    public override string GetDropItemByData(byte data) { return "minecraft:coal"; }

    public override string topName { get { return "coal_ore"; } }
    public override string bottomName { get { return "coal_ore"; } }
    public override string frontName { get { return "coal_ore"; } }
    public override string backName { get { return "coal_ore"; } }
    public override string leftName { get { return "coal_ore"; } }
    public override string rightName { get { return "coal_ore"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "coal_ore"; }
}
