using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLitFurnace : NBTFurnace
{
    public override string name => "Furnace";
    public override string id => "minecraft:lit_furnace";

    public override string frontName => "furnace_front_on";

    public override string GetBreakEffectTexture(byte data)
    {
        return "furnace_front_on";
    }

    public override string GetDropItemByData(byte data)
    {
        return "minecraft:furnace";
    }
}