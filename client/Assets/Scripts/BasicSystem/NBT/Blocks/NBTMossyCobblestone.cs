using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTMossyCobblestone : NBTBlock
{
    public override string name { get { return "Mossy Cobblestone"; } }
    public override string id { get { return "minecraft:mossy_cobblestone"; } }

    public override string topName { get { return "cobblestone_mossy"; } }
    public override string bottomName { get { return "cobblestone_mossy"; } }
    public override string frontName { get { return "cobblestone_mossy"; } }
    public override string backName { get { return "cobblestone_mossy"; } }
    public override string leftName { get { return "cobblestone_mossy"; } }
    public override string rightName { get { return "cobblestone_mossy"; } }

    public override float hardness { get { return 2; } }

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "cobblestone_mossy"; }
}
