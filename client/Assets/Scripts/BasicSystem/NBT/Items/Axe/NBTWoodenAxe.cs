using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenAxe : NBTAxe
{
    public override string name { get { return "Wooden Axe"; } }
    public override string id { get { return "minecraft:wooden_axe"; } }

    public override int attackDamage => 7;

    public override float toolSpeed => 2;

    public override int durability => 60;

    public override short burningTime => 200;

    public override string GetIconPathByData(short data) { return "wood_axe"; }
}

