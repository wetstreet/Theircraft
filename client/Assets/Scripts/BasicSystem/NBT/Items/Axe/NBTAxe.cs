using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTAxe : NBTItem
{
    public override string name { get { return "Wooden Axe"; } }
    public override string id { get { return "minecraft:wooden_axe"; } }

    public override byte maxStackCount { get { return 1; } }

    public override string GetIconPathByData(short data) { return "wood_axe"; }

    public override bool IsMatch(BlockMaterial blockMaterial)
    {
        return blockMaterial == BlockMaterial.Wood;
    }
}

