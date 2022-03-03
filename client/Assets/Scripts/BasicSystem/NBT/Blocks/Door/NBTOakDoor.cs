using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTOakDoor : NBTDoor
{
    public override string name => "Oak Door";
    public override string id => "minecraft:wooden_door";

    public override string GetIconPathByData(short data) { return "door_wood"; }

    public override string GetBreakEffectTexture(byte data) { return "door_wood_lower"; }

    public override string upperName => "door_wood_upper";
    public override string lowerName => "door_wood_lower";
}