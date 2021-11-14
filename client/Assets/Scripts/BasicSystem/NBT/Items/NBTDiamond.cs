using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDiamond : NBTItem
{
    public override string name { get { return "Diamond"; } }
    public override string id { get { return "minecraft:diamond"; } }

    public override string GetIconPathByData(short data) { return "diamond"; }
}