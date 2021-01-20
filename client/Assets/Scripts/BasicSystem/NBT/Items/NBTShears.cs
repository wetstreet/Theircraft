using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTShears : NBTItem
{
    public override string name { get { return "Shears"; } }
    public override string id { get { return "minecraft:shears"; } }

    public override byte maxStackCount { get { return 1; } }

    public override string GetNameByData(short data) { return name; }

    public override string GetIconPathByData(short data) { return "shears"; }

    protected override string itemMeshPath { get { return null; } }
}
