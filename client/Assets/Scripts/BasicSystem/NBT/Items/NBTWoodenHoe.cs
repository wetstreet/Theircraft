using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenHoe : NBTItem
{
    public override string name { get { return "Wooden Hoe"; } }
    public override string id { get { return "minecraft:wooden_hoe"; } }

    public override short burningTime => 200;

    public override string GetIconPathByData(short data) { return "wood_hoe"; }
}
