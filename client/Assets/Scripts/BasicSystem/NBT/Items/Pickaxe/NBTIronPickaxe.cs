using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTIronPickaxe : NBTPickaxe
{
    public override string name { get { return "Iron Pickaxe"; } }
    public override string id { get { return "minecraft:iron_pickaxe"; } }

    public override int attackDamage => 4;

    public override float toolSpeed => 6;

    public override int durability => 250;

    public override string GetIconPathByData(short data) { return "iron_pickaxe"; }

    public override bool IsMatch(BlockMaterial blockMaterial)
    {
        return blockMaterial == BlockMaterial.RockI ||
            blockMaterial == BlockMaterial.RockII ||
            blockMaterial == BlockMaterial.RockIII;
    }
}
