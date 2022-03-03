﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLargeFlowers : NBTPlant
{
    public override string name => "Large Flowers";
    public override string id => "minecraft:double_plant";

    protected override Color GetTintColorByData(NBTChunk chunk, Vector3Int pos, byte data)
    {
        if (data == 3 || data == 2)
        {
            return TintManager.tintColor;
        }
        else if (data == 10)
        {
            chunk.GetBlockData(pos.x, pos.y - 1, pos.z, out byte bottomType, out byte bottomData);
            if (bottomData == 3 || bottomData == 2)
            {
                return TintManager.tintColor;
            }
        }
        return Color.white;
    }

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        switch (data)
        {
            case 0:
                return "double_plant_sunflower_bottom";
            case 1:
                return "double_plant_syringa_bottom";
            case 2:
                return "double_plant_grass_bottom";
            case 3:
                return "double_plant_fern_bottom";
            case 4:
                return "double_plant_rose_bottom";
            case 5:
                return "double_plant_paeonia_bottom";
            case 10:
                chunk.GetBlockData(pos.x, pos.y - 1, pos.z, out byte bottomType, out byte bottomData);
                switch (bottomData)
                {
                    case 0:
                        return "double_plant_sunflower_top";
                    case 1:
                        return "double_plant_syringa_top";
                    case 2:
                        return "double_plant_grass_top";
                    case 3:
                        return "double_plant_fern_top";
                    case 4:
                        return "double_plant_rose_top";
                    case 5:
                        return "double_plant_paeonia_top";
                }
                throw new System.Exception("bottomData=" + bottomData);
        }
        throw new System.Exception("data=" + data);
    }

    public override string GetBreakEffectTexture(NBTChunk chunk, Vector3Int pos, byte data)
    {
        return GetTexName(chunk, pos, data);
    }

    public override string GetIconPathByData(short data)
    {
        switch (data)
        {
            case 1:
                return "double_plant_syringa_bottom";
            case 2:
                return "double_plant_grass_bottom";
            case 3:
                return "double_plant_fern_bottom";
            case 4:
                return "double_plant_rose_bottom";
            case 5:
                return "double_plant_paeonia_bottom";
        }
        throw new System.Exception("no icon");
    }
}