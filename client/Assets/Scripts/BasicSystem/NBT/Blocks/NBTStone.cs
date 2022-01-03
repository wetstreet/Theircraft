using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStone : NBTBlock
{
    public override string name => "Stone";
    public override string id => "minecraft:stone";

    public override float hardness => 1.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetDropItemByData(byte data) { return "minecraft:cobblestone"; }

    public override string allName => "stone";

    public override string GetBreakEffectTexture(byte data) { return "stone"; }
}
