using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenSlab : NBTSlab
{
    public override string name => "Wooden Slab";
    public override string id => "minecraft:wooden_slab";

    public override short burningTime => 150;

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Oak Wood Slab";
            case 1:
                return "Spruce Wood Slab";
            case 2:
                return "Birch Wood Slab";
            case 3:
                return "Jungle Wood Slab";
            case 4:
                return "Acacia Wood Slab";
            case 5:
                return "Dark Oak Wood Slab";
        }
        return "Wood Slab";
    }

    public override float hardness => 2f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    protected override string GetTexName(int data)
    {
        switch (data)
        {
            case 0:
                return "planks_oak";
            case 1:
                return "planks_spruce";
            case 2:
                return "planks_birch";
            case 3:
                return "planks_jungle";
            case 4:
                return "planks_acacia";
            case 5:
                return "planks_big_oak";
        }
        return "planks_oak";
    }
}
