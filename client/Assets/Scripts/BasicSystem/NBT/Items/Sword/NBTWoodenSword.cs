using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenSword : NBTItem
{
    public override string name { get { return "Wooden Sword"; } }
    public override string id { get { return "minecraft:wooden_sword"; } }

    public override int attackDamage => 4;

    public override int durability => 59;

    public override short burningTime => 200;

    public override string GetIconPathByData(short data) { return "wood_sword"; }
}