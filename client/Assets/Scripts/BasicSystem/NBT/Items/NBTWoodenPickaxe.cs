using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenPickaxe : NBTItem
{
    public override string name { get { return "Wooden Pickaxe"; } }
    public override string id { get { return "minecraft:wooden_pickaxe"; } }

    public override string GetIconPathByData(short data) { return "wood_pickaxe"; }
}
