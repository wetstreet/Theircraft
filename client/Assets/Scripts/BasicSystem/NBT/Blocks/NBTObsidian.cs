using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTObsidian : NBTBlock
{
    public override string name { get { return "Obsidian"; } }

    public override string topName { get { return "obsidian"; } }
    public override string bottomName { get { return "obsidian"; } }
    public override string frontName { get { return "obsidian"; } }
    public override string backName { get { return "obsidian"; } }
    public override string leftName { get { return "obsidian"; } }
    public override string rightName { get { return "obsidian"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "obsidian"; }
}
