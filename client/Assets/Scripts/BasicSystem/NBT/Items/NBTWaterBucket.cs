using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWaterBucket : NBTItem
{
    public override string name { get { return "Water Bucket"; } }
    public override string id { get { return "minecraft:water_bucket"; } }

    public override string GetIconPathByData(short data) { return "bucket_water"; }
}
