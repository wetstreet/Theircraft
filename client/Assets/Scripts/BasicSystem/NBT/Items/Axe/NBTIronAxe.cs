using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTIronAxe : NBTAxe
{
    public override string name { get { return "Iron Axe"; } }
    public override string id { get { return "minecraft:iron_axe"; } }

    public override int attackDamage => 9;

    public override float toolSpeed => 6;

    public override int durability => 251;

    public override string GetIconPathByData(short data) { return "iron_axe"; }
}