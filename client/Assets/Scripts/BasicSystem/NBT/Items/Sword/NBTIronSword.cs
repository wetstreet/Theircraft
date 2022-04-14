using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTIronSword : NBTItem
{
    public override string name { get { return "Iron Sword"; } }
    public override string id { get { return "minecraft:iron_sword"; } }

    public override int attackDamage => 6;

    public override int durability => 250;

    public override string GetIconPathByData(short data) { return "iron_sword"; }
}