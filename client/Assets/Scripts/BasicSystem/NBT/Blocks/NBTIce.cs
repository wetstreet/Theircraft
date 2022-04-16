using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTIce : NBTBlock
{
    public override string name => "Ice";
    public override string id => "minecraft:ice";

    public override float hardness => 0.5f;

    public override SoundMaterial soundMaterial => SoundMaterial.Glass;
    public override BlockMaterial blockMaterial => BlockMaterial.Glass;

    public override string allName => "ice";

    public override bool isTransparent => true;

    public override string GetBreakEffectTexture(byte data) { return "ice"; }
}
