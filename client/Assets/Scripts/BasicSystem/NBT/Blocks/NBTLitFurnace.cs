using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLitFurnace : NBTBlock
{
    public override string name => "Furnace";
    public override string id => "minecraft:lit_furnace";

    public override float hardness => 3.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string frontName => "furnace_front_on";
    public override string backName => "furnace_side";
    public override string leftName => "furnace_side";
    public override string rightName => "furnace_side";
    public override string topName => "furnace_top";
    public override string bottomName => "furnace_top";

    public override string GetBreakEffectTexture(byte data)
    {
        return "furnace_front_on";
    }

    public override bool canInteract => true;
    public override void OnRightClick()
    {
        FurnaceUI.Show(WireFrameHelper.pos);
    }
}