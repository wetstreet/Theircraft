using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTClay : NBTBlock
{
    public override string name { get { return "Clay"; } }
    public override string id { get { return "minecraft:clay"; } }

    public override string topName { get { return "clay"; } }
    public override string bottomName { get { return "clay"; } }
    public override string frontName { get { return "clay"; } }
    public override string backName { get { return "clay"; } }
    public override string leftName { get { return "clay"; } }
    public override string rightName { get { return "clay"; } }

    public override float hardness => 0.6f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Gravel; } }

    public override string GetBreakEffectTexture(byte data) { return "clay"; }
}
