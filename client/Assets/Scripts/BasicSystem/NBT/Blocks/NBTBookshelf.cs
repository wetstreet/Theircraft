using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBookshelf : NBTBlock
{
    public override string name => "Bookshelf";
    public override string id => "minecraft:bookshelf";

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override float hardness => 2.5f;

    public override string frontName => "bookshelf";
    public override string backName => "bookshelf";
    public override string leftName => "bookshelf";
    public override string rightName => "bookshelf";
    public override string topName => "planks_oak";
    public override string bottomName => "planks_oak";

    public override string GetBreakEffectTexture(byte data)
    {
        return "bookshelf";
    }
}