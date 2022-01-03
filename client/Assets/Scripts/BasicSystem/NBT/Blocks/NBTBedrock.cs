using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBedrock : NBTBlock
{
    public override string name => "Bedrock";
    public override string id => "minecraft:bedrock";

    public override BlockMaterial blockMaterial => BlockMaterial.Other;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override float hardness => -1;

    public override string allName => "bedrock";

    public override string GetBreakEffectTexture(byte data) { return "bedrock"; }
}
