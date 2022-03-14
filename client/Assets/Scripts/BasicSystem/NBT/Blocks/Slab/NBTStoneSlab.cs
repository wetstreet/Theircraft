using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStoneSlab : NBTSlab
{
    public override string name => "Wooden Slab";
    public override string id => "minecraft:stone_slab";

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Stone Slab";
            case 1:
                return "Sandstone Slab";
            case 3:
                return "Cobblestone Slab";
            case 4:
                return "Bricks Slab";
            case 5:
                return "Stone Bricks Slab";
            case 6:
                return "Quartz Slab";
            case 7:
                return "Nether Brick Slab";
        }
        return "Wood Slab";
    }

    public override float hardness => 2f;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    protected override string GetTexName(int data)
    {
        switch (data)
        {
            case 0:
                return "stone_slab_top";
            case 1:
                return "sandstone_top";
            case 3:
                return "cobblestone";
            case 4:
                return "brick";
            case 5:
                return "stonebrick";
            case 6:
                return "quartz_block_top";
            case 7:
                return "nether_brick";
        }
        return "planks_oak";
    }
}
