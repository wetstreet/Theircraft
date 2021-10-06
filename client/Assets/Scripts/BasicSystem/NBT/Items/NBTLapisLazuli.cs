using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLapisLazuli : NBTItem
{
    public override string name { get { return "Lapis Lazuli"; } }
    public override string id { get { return "minecraft:lapis_lazuli"; } }

    public override string GetIconPathByData(short data) { return "lapis"; }
}
