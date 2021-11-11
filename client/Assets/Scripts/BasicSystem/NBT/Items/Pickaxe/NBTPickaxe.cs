using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPickaxe : NBTItem
{
    public override string name { get { return "Wooden Pickaxe"; } }
    public override string id { get { return "minecraft:wooden_pickaxe"; } }

    public override byte maxStackCount { get { return 1; } }

    public override string GetIconPathByData(short data) { return "wood_pickaxe"; }
}
