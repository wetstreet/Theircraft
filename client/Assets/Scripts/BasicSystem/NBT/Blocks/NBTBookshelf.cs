using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBookshelf : NBTBlock
{
    public override string name { get { return "Bookshelf"; } }
    public override string id { get { return "minecraft:bookshelf"; } }

    public override string frontName => "bookshelf";
    public override string backName => "bookshelf";
    public override string leftName => "bookshelf";
    public override string rightName => "bookshelf";
    public override string topName => "planks_oak";
    public override string bottomName => "planks_oak";

    public override float hardness { get { return 2.5f; } }

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
    {
        return "bookshelf";
    }
}