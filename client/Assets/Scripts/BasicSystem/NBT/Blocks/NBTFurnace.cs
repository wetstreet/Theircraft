using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceItem
{
    public byte count;
    public short damage;
    public string id;
    public byte slot;

    public FurnaceItem(string _id)
    {
        count = 1;
        damage = 0;
        id = _id;
        slot = 2;
    }

    public FurnaceItem(TagNodeCompound item)
    {
        count = item["Count"].ToTagByte();
        damage = item["Damage"].ToTagShort();
        id = item["id"].ToTagString();
        slot = item["Slot"].ToTagByte();
    }
}

public class FurnaceData
{
    public FurnaceItem source; // slot 0
    public FurnaceItem fuel; // slot 1
    public FurnaceItem result; // slot 2
    public short burnTime;
    public short cookTime;
    public short cookTimeTotal;

    public FurnaceData(TagNodeCompound node)
    {
        burnTime = node["BurnTime"].ToTagShort();
        cookTime = node["CookTime"].ToTagShort();
        cookTimeTotal = node["CookTimeTotal"].ToTagShort();
        var items = node["Items"].ToTagList();
        foreach (TagNodeCompound item in items)
        {
            var slot = item["Slot"] as TagNodeByte;
            if (slot == 0)
                source = new FurnaceItem(item);
            else if (slot == 1)
                fuel = new FurnaceItem(item);
            else if (slot == 2)
                result = new FurnaceItem(item);
        }
    }

    public void Serialize(TagNodeCompound node)
    {

    }
}

public class NBTFurnace : NBTBlock
{
    public override string name => "Furnace";
    public override string id => "minecraft:furnace";

    public override float hardness => 3.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string frontName => "furnace_front_off";
    public override string backName => "furnace_side";
    public override string leftName => "furnace_side";
    public override string rightName => "furnace_side";
    public override string topName => "furnace_top";
    public override string bottomName => "furnace_top";

    public override string GetBreakEffectTexture(byte data)
    {
        return "furnace_front_off";
    }

    public override bool canInteract => true;
    public override void OnRightClick()
    {
        FurnaceUI.Show(WireFrameHelper.pos);
    }
}