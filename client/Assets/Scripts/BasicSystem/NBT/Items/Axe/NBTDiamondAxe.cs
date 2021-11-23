using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDiamondAxe : NBTAxe
{
    public override string name { get { return "Diamond Axe"; } }
    public override string id { get { return "minecraft:diamond_axe"; } }

    public override int attackDamage => 9;

    public override float toolSpeed => 8;

    public override int durability => 1561;

    public override string GetIconPathByData(short data) { return "diamond_axe"; }
}