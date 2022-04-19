using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCoal : NBTItem
{
    public override string name { get { return "Coal"; } }
    public override string id { get { return "minecraft:coal"; } }

    public override short burningTime => 1600;

    public override string GetIconPathByData(short data) { return "coal"; }
}
