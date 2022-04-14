using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGoldSword : NBTItem
{
    public override string name { get { return "Gold Sword"; } }
    public override string id { get { return "minecraft:gold_sword"; } }

    public override int attackDamage => 4;

    public override int durability => 32;

    public override string GetIconPathByData(short data) { return "gold_sword"; }
}