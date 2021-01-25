using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBirchStairs : NBTBlock
{
    public override string name { get { return "Birch Stairs"; } }
    public override string id { get { return "minecraft:birch_stairs"; } }

    public override string GetIconPathByData(short data){ return "BirchWoodStairs"; }

    public override string topName => "planks_birch";
    public override string bottomName => "planks_birch";
    public override string leftName => "planks_birch";
    public override string rightName => "planks_birch";
    public override string frontName => "planks_birch";
    public override string backName => "planks_birch";

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_birch"; }
}
