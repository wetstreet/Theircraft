using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSandStone : NBTBlock
{
    public override string name => "Sandstone";
    public override string id => "minecraft:sandstone";

    public override string topName => "sandstone_top";
    public override string bottomName => "sandstone_bottom";
    public override string frontName => "sandstone_normal";
    public override string backName => "sandstone_normal";
    public override string leftName => "sandstone_normal";
    public override string rightName => "sandstone_normal";

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetBreakEffectTexture(byte data) { return "sandstone_normal"; }
}
