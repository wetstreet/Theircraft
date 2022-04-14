using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStoneSword : NBTItem
{
    public override string name { get { return "Stone Sword"; } }
    public override string id { get { return "minecraft:stone_sword"; } }

    public override int attackDamage => 5;

    public override int durability => 131;

    public override string GetIconPathByData(short data) { return "stone_sword"; }
}