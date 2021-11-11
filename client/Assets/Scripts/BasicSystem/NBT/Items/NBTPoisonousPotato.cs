using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPoisonousPotato : NBTItem
{
    public override string name { get { return "Poisonous Potato"; } }
    public override string id { get { return "minecraft:poisonous_potato"; } }

    public override string GetIconPathByData(short data) { return "potato_poisonous"; }
}
