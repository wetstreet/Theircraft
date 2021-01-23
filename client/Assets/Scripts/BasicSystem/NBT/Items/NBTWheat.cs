using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWheat : NBTItem
{
    public override string name { get { return "Wheat"; } }
    public override string id { get { return "minecraft:wheat"; } }

    public override string GetIconPathByData(short data) { return "wheat"; }
}
