using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGoldPickaxe : NBTPickaxe
{
    public override string name { get { return "Gold Pickaxe"; } }
    public override string id { get { return "minecraft:gold_pickaxe"; } }

    public override int attackDamage => 2;

    public override float toolSpeed => 12;

    public override int durability => 32;

    public override string GetIconPathByData(short data) { return "gold_pickaxe"; }

    public override bool IsMatch(BlockMaterial blockMaterial)
    {
        return blockMaterial == BlockMaterial.RockI ||
            blockMaterial == BlockMaterial.RockII ||
            blockMaterial == BlockMaterial.RockIII;
    }
}
