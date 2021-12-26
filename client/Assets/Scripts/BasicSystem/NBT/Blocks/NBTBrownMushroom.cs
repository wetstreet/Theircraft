using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBrownMushroom : NBTPlant
{
    public override string name => "Brown Mushroom";
    public override string id => "minecraft:brown_mushroom";

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("mushroom_brown");
    }

    public override string GetBreakEffectTexture(byte data) { return "mushroom_brown"; }
}