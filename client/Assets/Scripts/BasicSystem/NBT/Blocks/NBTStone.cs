using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStone : NBTBlock
{
    public override string name { get { return "Stone"; } }
    public override string id { get { return "minecraft:stone"; } }

    public override float hardness { get { return 1.5f; } }

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;

    public override string GetDropItemByData(byte data) { return "minecraft:cobblestone"; }

    public override string topName { get { return "stone"; } }
    public override string bottomName { get { return "stone"; } }
    public override string frontName { get { return "stone"; } }
    public override string backName { get { return "stone"; } }
    public override string leftName { get { return "stone"; } }
    public override string rightName { get { return "stone"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "stone"; }
}
