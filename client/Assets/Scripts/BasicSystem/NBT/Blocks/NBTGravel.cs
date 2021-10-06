using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGravel : NBTBlock
{
    public override string name { get { return "Gravel"; } }
    public override string id { get { return "minecraft:gravel"; } }

    public override string topName { get { return "gravel"; } }
    public override string bottomName { get { return "gravel"; } }
    public override string frontName { get { return "gravel"; } }
    public override string backName { get { return "gravel"; } }
    public override string leftName { get { return "gravel"; } }
    public override string rightName { get { return "gravel"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Gravel; } }

    public override string GetBreakEffectTexture(byte data) { return "gravel"; }
}
