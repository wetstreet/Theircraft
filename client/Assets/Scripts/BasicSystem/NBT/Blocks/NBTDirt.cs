﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDirt : NBTBlock
{
    public override string name { get { return "Dirt"; } }
    public override string id { get { return "minecraft:dirt"; } }

    public override string topName { get { return "dirt"; } }
    public override string bottomName { get { return "dirt"; } }
    public override string frontName { get { return "dirt"; } }
    public override string backName { get { return "dirt"; } }
    public override string leftName { get { return "dirt"; } }
    public override string rightName { get { return "dirt"; } }

    public override float hardness => 0.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Gravel; } }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }
}
