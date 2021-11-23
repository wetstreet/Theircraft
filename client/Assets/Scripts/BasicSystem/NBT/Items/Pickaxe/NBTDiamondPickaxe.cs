using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDiamondPickaxe : NBTPickaxe
{
    public override string name { get { return "Diamond Pickaxe"; } }
    public override string id { get { return "minecraft:diamond_pickaxe"; } }

    public override int attackDamage => 5;

    public override float toolSpeed => 8;

    public override int durability => 1561;

    public override string GetIconPathByData(short data) { return "diamond_pickaxe"; }

    public override bool IsMatch(BlockMaterial blockMaterial)
    {
        return blockMaterial == BlockMaterial.RockI ||
            blockMaterial == BlockMaterial.RockII ||
            blockMaterial == BlockMaterial.RockIII ||
            blockMaterial == BlockMaterial.RockIV;
    }
}
