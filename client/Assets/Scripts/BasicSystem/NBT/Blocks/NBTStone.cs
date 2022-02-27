using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStone : NBTBlock
{
    public override string name => "Stone";
    public override string id => "minecraft:stone";

    public override float hardness => 1.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string GetDropItemByData(byte data)
    {
        if (data == 0)
            return "minecraft:cobblestone";
        else
            return "minecraft:stone";
    }

    public override string allName => "stone";

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Stone";
            case 1:
                return "Granite";
            case 2:
                return "Polished Granite";
            case 3:
                return "Diorite";
            case 4:
                return "Polished Diorite";
            case 5:
                return "Andesite";
            case 6:
                return "Polished Andesite";
        }
        throw new System.Exception("no tex for data!");
    }

    string GetTexByData(int data)
    {
        switch (data)
        {
            case 0:
                return "stone";
            case 1:
                return "stone_granite";
            case 2:
                return "stone_granite_smooth";
            case 3:
                return "stone_diorite";
            case 4:
                return "stone_diorite_smooth";
            case 5:
                return "stone_andesite";
            case 6:
                return "stone_andesite_smooth";
        }
        throw new System.Exception("no tex for data!");
    }

    public override string GetFrontTexName(NBTChunk chunk, int data) { return GetTexByData(data); }
    public override string GetBackTexName(NBTChunk chunk, int data) { return GetTexByData(data); }
    public override string GetLeftTexName(NBTChunk chunk, int data) { return GetTexByData(data); }
    public override string GetRightTexName(NBTChunk chunk, int data) { return GetTexByData(data); }
    public override string GetTopTexName(NBTChunk chunk, int data) { return GetTexByData(data); }
    public override string GetBottomTexName(NBTChunk chunk, int data) { return GetTexByData(data); }

    public override string GetBreakEffectTexture(byte data) { return GetTexByData(data); }
}
