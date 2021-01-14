using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSandStone : NBTBlock
{
    public override string name { get { return "SandStone"; } }

    public override string topName { get { return "sandstone_top"; } }
    public override string bottomName { get { return "sandstone_bottom"; } }
    public override string frontName { get { return "sandstone_normal"; } }
    public override string backName { get { return "sandstone_normal"; } }
    public override string leftName { get { return "sandstone_normal"; } }
    public override string rightName { get { return "sandstone_normal"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "sandstone_normal"; }
}
