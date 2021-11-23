using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGoldAxe : NBTAxe
{
    public override string name { get { return "Gold Axe"; } }
    public override string id { get { return "minecraft:gold_axe"; } }

    public override int attackDamage => 7;

    public override float toolSpeed => 12;

    public override int durability => 32;

    public override string GetIconPathByData(short data) { return "gold_axe"; }
}