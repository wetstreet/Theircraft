using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWool : NBTBlock
{
    public override string name => "Wool";
    public override string id => "minecraft:wool";

    public override float hardness => 0.8f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wool;
    public override SoundMaterial soundMaterial => SoundMaterial.Snow;

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "White Wool";
            case 1:
                return "Orange Wool";
            case 2:
                return "Magenta Wool";
            case 3:
                return "Light Blue Wool";
            case 4:
                return "Yellow Wool";
            case 5:
                return "Lime Wool";
            case 6:
                return "Pink Wool";
            case 7:
                return "Gray Wool";
            case 8:
                return "Light Gray Wool";
            case 9:
                return "Cyan Wool";
            case 10:
                return "Purple Wool";
            case 11:
                return "Blue Wool";
            case 12:
                return "Brown Wool";
            case 13:
                return "Green Wool";
            case 14:
                return "Red Wool";
            case 15:
                return "Black Wool";
        }
        throw new System.Exception("no name, data=" + data);
    }

    string GetTextureNameByData(int data)
    {
        switch (data)
        {
            case 0:
                return "wool_colored_white";
            case 1:
                return "wool_colored_orange";
            case 2:
                return "wool_colored_magenta";
            case 3:
                return "wool_colored_light_blue";
            case 4:
                return "wool_colored_yellow";
            case 5:
                return "wool_colored_lime";
            case 6:
                return "wool_colored_pink";
            case 7:
                return "wool_colored_gray";
            case 8:
                return "wool_colored_silver";
            case 9:
                return "wool_colored_cyan";
            case 10:
                return "wool_colored_purple";
            case 11:
                return "wool_colored_blue";
            case 12:
                return "wool_colored_brown";
            case 13:
                return "wool_colored_green";
            case 14:
                return "wool_colored_red";
            case 15:
                return "wool_colored_black";
        }
        return "wool_colored_white";
    }

    public override string GetTopTexName(NBTChunk chunk, int data) { return GetTextureNameByData(data); }
    public override string GetBottomTexName(NBTChunk chunk, int data) { return GetTextureNameByData(data); }
    public override string GetFrontTexName(NBTChunk chunk, int data) { return GetTextureNameByData(data); }
    public override string GetBackTexName(NBTChunk chunk, int data) { return GetTextureNameByData(data); }
    public override string GetLeftTexName(NBTChunk chunk, int data) { return GetTextureNameByData(data); }
    public override string GetRightTexName(NBTChunk chunk, int data) { return GetTextureNameByData(data); }

    public override string GetBreakEffectTexture(byte data) { return GetTextureNameByData(data); }
}
