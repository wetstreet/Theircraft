using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSand : NBTBlock
{
    public override string id => "minecraft:sand";
    public override string name { get { return "Sand"; } }

    public override string topName { get { return "sand"; } }
    public override string bottomName { get { return "sand"; } }
    public override string frontName { get { return "sand"; } }
    public override string backName { get { return "sand"; } }
    public override string leftName { get { return "sand"; } }
    public override string rightName { get { return "sand"; } }

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Sand; } }

    public override string GetBreakEffectTexture(byte data) { return "sand"; }
}
