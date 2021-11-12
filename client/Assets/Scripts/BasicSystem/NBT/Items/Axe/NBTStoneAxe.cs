using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStoneAxe : NBTAxe
{
    public override string name { get { return "Stone Axe"; } }
    public override string id { get { return "minecraft:stone_axe"; } }

    public override int attackDamage => 9;

    public override float toolSpeed => 4;

    public override int durability => 132;

    public override string GetIconPathByData(short data) { return "stone_axe"; }
}