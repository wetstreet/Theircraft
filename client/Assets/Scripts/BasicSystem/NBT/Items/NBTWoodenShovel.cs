using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenShovel : NBTItem
{
    public override string name { get { return "Wooden Shovel"; } }
    public override string id { get { return "minecraft:wooden_shovel"; } }

    public override short burningTime => 200;

    public override string GetIconPathByData(short data) { return "wood_shovel"; }
}
