using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTClay : NBTBlock
{
    public override string name { get { return "Clay"; } }

    public override string topName { get { return "clay"; } }
    public override string bottomName { get { return "clay"; } }
    public override string frontName { get { return "clay"; } }
    public override string backName { get { return "clay"; } }
    public override string leftName { get { return "clay"; } }
    public override string rightName { get { return "clay"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Gravel; } }
}
