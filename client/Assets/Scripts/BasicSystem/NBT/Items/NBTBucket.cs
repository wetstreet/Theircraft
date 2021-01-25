using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBucket : NBTItem
{
    public override string name { get { return "Bucket"; } }
    public override string id { get { return "minecraft:bucket"; } }

    public override string GetIconPathByData(short data) { return "bucket_empty"; }
}
