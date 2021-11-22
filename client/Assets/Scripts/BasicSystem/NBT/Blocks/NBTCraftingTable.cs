using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCraftingTable : NBTBlock
{
    public override string name { get { return "Crafting Table"; } }
    public override string id { get { return "minecraft:crafting_table"; } }

    public override float hardness { get { return 2.5f; } }

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    public override string frontName => "crafting_table_front";
    public override string backName => "crafting_table_side";
    public override string leftName => "crafting_table_side";
    public override string rightName => "crafting_table_front";
    public override string topName => "crafting_table_top";
    public override string bottomName => "crafting_table_top";

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
    {
        return "crafting_table_front";
    }

    public override bool canInteract => true;
    public override void OnRightClick(Vector3Int pos)
    {
        CraftingTableUI.Show();
    }
}
