using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCobblestone : NBTBlock
{
    public override string name { get { return "Cobblestone"; } }

    public override string topName { get { return "cobblestone"; } }
    public override string bottomName { get { return "cobblestone"; } }
    public override string frontName { get { return "cobblestone"; } }
    public override string backName { get { return "cobblestone"; } }
    public override string leftName { get { return "cobblestone"; } }
    public override string rightName { get { return "cobblestone"; } }

    public override float hardness { get { return 2; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "cobblestone"; }
}
