using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedMushroom : NBTPlant
{
    public override string name => "Red Mushroom";
    public override string id => "minecraft:red_mushroom";

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("mushroom_red");
    }

    public override string GetBreakEffectTexture(byte data) { return "mushroom_red"; }
}