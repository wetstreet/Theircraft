using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedstone : NBTItem
{
    public override string name { get { return "Redstone"; } }
    public override string id { get { return "minecraft:redstone"; } }

    public override string GetIconPathByData(short data) { return "redstone_dust"; }
}
