using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGoldIngot : NBTItem
{
    public override string name { get { return "Gold Ingot"; } }
    public override string id { get { return "minecraft:gold_ingot"; } }

    public override string GetIconPathByData(short data) { return "gold_ingot"; }
}