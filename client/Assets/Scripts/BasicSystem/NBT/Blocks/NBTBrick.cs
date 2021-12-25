using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBrick : NBTBlock
{
    public override string name { get { return "Brick"; } }
    public override string id { get { return "minecraft:brick"; } }

    public override string topName { get { return "brick"; } }
    public override string bottomName { get { return "brick"; } }
    public override string frontName { get { return "brick"; } }
    public override string backName { get { return "brick"; } }
    public override string leftName { get { return "brick"; } }
    public override string rightName { get { return "brick"; } }

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "brick"; }
}
