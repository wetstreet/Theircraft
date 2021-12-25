using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBedrock : NBTBlock
{
    public override string name { get { return "Bedrock"; } }
    public override string id { get { return "minecraft:bedrock"; } }

    public override string topName { get { return "bedrock"; } }
    public override string bottomName { get { return "bedrock"; } }
    public override string frontName { get { return "bedrock"; } }
    public override string backName { get { return "bedrock"; } }
    public override string leftName { get { return "bedrock"; } }
    public override string rightName { get { return "bedrock"; } }

    public override float hardness => -1;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "bedrock"; }
}
