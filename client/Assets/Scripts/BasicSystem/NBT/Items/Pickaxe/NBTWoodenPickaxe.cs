using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenPickaxe : NBTPickaxe
{
    public override string name { get { return "Wooden Pickaxe"; } }
    public override string id { get { return "minecraft:wooden_pickaxe"; } }

    public override int attackDamage => 2;

    public override float toolSpeed => 2;

    public override int durability => 60;

    public override short burningTime => 200;

    public override string GetIconPathByData(short data) { return "wood_pickaxe"; }

    public override bool IsMatch(BlockMaterial blockMaterial)
    {
        return blockMaterial == BlockMaterial.RockI;
    }
}
