using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBeetrootSeeds : NBTItem
{
    public override string name { get { return "Beetroot Seeds"; } }
    public override string id { get { return "minecraft:beetroot_seeds"; } }

    public override string GetIconPathByData(short data) { return "beetroot_seeds"; }
}
