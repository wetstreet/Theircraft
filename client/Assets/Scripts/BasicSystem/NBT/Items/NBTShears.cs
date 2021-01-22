using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTShears : NBTItem
{
    public override string name { get { return "Shears"; } }
    public override string id { get { return "minecraft:shears"; } }

    public override byte maxStackCount { get { return 1; } }

    public override string GetIconPathByData(short data) { return "shears"; }
}
