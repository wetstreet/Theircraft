using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCraftingTable : NBTBlock
{
    public override string name => "Crafting Table";
    public override string id => "minecraft:crafting_table";

    public override float hardness => 2.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override short burningTime => 300;

    public override string frontName => "crafting_table_front";
    public override string backName => "crafting_table_side";
    public override string leftName => "crafting_table_side";
    public override string rightName => "crafting_table_front";
    public override string topName => "crafting_table_top";
    public override string bottomName => "crafting_table_top";

    public override string GetBreakEffectTexture(byte data)
    {
        return "crafting_table_front";
    }

    public override bool canInteract => true;
    public override void OnRightClick()
    {
        CraftingTableUI.Show();
    }
}
