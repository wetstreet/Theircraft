using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTIronIngot : NBTItem
{
    public override string name { get { return "Iron Ingot"; } }
    public override string id { get { return "minecraft:iron_ingot"; } }

    public override string GetIconPathByData(short data) { return "iron_ingot"; }
}