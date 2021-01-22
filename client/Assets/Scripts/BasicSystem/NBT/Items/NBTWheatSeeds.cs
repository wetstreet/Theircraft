using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWheatSeeds : NBTItem
{
    public override string name { get { return "Wheat Seeds"; } }
    public override string id { get { return "minecraft:wheat_seeds"; } }

    public override string GetIconPathByData(short data) { return "seeds_wheat"; }
}
