using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRottonFlesh : NBTItem
{
    public override string name { get { return "Rotten Flesh"; } }
    public override string id { get { return "minecraft:rotten_flesh"; } }

    public override string GetIconPathByData(short data) { return "rotten_flesh"; }
}
