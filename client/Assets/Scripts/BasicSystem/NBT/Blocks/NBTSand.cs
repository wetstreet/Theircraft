using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSand : NBTBlock
{
    public override string id => "minecraft:sand";
    public override string name => "Sand";

    public override string allName => "sand";

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;
    public override SoundMaterial soundMaterial => SoundMaterial.Sand;

    public override string GetBreakEffectTexture(byte data) { return "sand"; }
}
