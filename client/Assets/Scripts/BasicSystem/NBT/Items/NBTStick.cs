using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStick : NBTItem
{
    public override string name { get { return "Stick"; } }
    public override string id { get { return "minecraft:stick"; } }

    public override short burningTime => 100;

    public override string GetIconPathByData(short data) { return "stick"; }
}
