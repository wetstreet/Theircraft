using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGlassPane : NBTBlock
{
    public override string name { get { return "Glass Panel"; } }
    public override string id { get { return "minecraft:glass_pane"; } }

    public override string topName { get { return "glass"; } }
    public override string bottomName { get { return "glass"; } }
    public override string frontName { get { return "glass"; } }
    public override string backName { get { return "glass"; } }
    public override string leftName { get { return "glass"; } }
    public override string rightName { get { return "glass"; } }

    public override bool isTransparent => true;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Glass; } }

    public override string GetBreakEffectTexture(byte data) { return "glass"; }
}