using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGunpowder : NBTItem
{
    public override string name { get { return "Gunpowder"; } }
    public override string id { get { return "minecraft:gunpowder"; } }

    public override string GetIconPathByData(short data) { return "gunpowder"; }
}
