using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSpruceDoor : NBTOakDoor
{
    public override string name => "Spruce Door";
    public override string id => "minecraft:spruce_door";
    public override string GetIconPathByData(short data) { return "door_spruce"; }

    public override string GetBreakEffectTexture(byte data) { return "door_spruce_lower"; }

    public override string upperName => "door_spruce_upper";
    public override string lowerName => "door_spruce_lower";
}
