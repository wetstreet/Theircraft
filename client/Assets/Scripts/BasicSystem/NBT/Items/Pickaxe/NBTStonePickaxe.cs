using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStonePickaxe : NBTPickaxe
{
    public override string name { get { return "Stone Pickaxe"; } }
    public override string id { get { return "minecraft:stone_pickaxe"; } }

    public override int attackDamage => 3;

    public override float toolSpeed => 4;

    public override int durability => 131;

    public override string GetIconPathByData(short data) { return "stone_pickaxe"; }

    public override bool IsMatch(BlockMaterial blockMaterial)
    {
        return blockMaterial == BlockMaterial.RockI ||
            blockMaterial == BlockMaterial.RockII;
    }
}
