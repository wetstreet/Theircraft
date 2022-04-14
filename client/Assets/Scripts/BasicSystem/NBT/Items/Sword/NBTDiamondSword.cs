using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDiamondSword : NBTItem
{
    public override string name { get { return "Diamond Sword"; } }
    public override string id { get { return "minecraft:diamond_sword"; } }

    public override int attackDamage => 7;

    public override int durability => 1561;

    public override string GetIconPathByData(short data) { return "diamond_sword"; }
}