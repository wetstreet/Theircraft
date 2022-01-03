using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGlass : NBTBlock
{
    public override string name => "Glass";
    public override string id => "minecraft:glass";

    public override string allName => "glass";

    public override bool isTransparent => true;

    public override float hardness => 0.3f;

    public override BlockMaterial blockMaterial => BlockMaterial.Glass;
    public override SoundMaterial soundMaterial => SoundMaterial.Glass;

    public override string GetBreakEffectTexture(byte data) { return "glass"; }
}
