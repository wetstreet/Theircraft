using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLargeFlowers : NBTPlant
{
    public override string name { get { return "Large Flowers"; } }

    protected override Color GetTintColorByData(NBTChunk chunk, byte data)
    {
        if (data == 3 || data == 2)
        {
            return TintManager.tintColor;
        }
        else if (data == 10)
        {
            byte bottomType = 0;
            byte bottomData = 0;
            chunk.GetBlockData(pos.x, pos.y - 1, pos.z, ref bottomType, ref bottomData);
            if (bottomData == 3 || bottomData == 2)
            {
                return TintManager.tintColor;
            }
        }
        return Color.white;
    }

    public override void Init()
    {
        UsedTextures = new string[]
        {
            "double_plant_grass_bottom",
            "double_plant_grass_top",
            "double_plant_fern_bottom",
            "double_plant_fern_top",
            "double_plant_syringa_bottom",
            "double_plant_syringa_top",
            "double_plant_rose_bottom",
            "double_plant_rose_top",
            "double_plant_paeonia_bottom",
            "double_plant_paeonia_top",
        };
    }

    public override int GetPlantIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 1:
                return TextureArrayManager.GetIndexByName("double_plant_syringa_bottom");
            case 2:
                return TextureArrayManager.GetIndexByName("double_plant_grass_bottom");
            case 3:
                return TextureArrayManager.GetIndexByName("double_plant_fern_bottom");
            case 4:
                return TextureArrayManager.GetIndexByName("double_plant_rose_bottom");
            case 5:
                return TextureArrayManager.GetIndexByName("double_plant_paeonia_bottom");
            case 10:
                byte bottomType = 0;
                byte bottomData = 0;
                chunk.GetBlockData(pos.x, pos.y - 1, pos.z, ref bottomType, ref bottomData);
                switch (bottomData)
                {
                    case 1:
                        return TextureArrayManager.GetIndexByName("double_plant_syringa_top");
                    case 2:
                        return TextureArrayManager.GetIndexByName("double_plant_grass_top");
                    case 3:
                        return TextureArrayManager.GetIndexByName("double_plant_fern_top");
                    case 4:
                        return TextureArrayManager.GetIndexByName("double_plant_rose_top");
                    case 5:
                        return TextureArrayManager.GetIndexByName("double_plant_paeonia_top");
                }
                break;
        }
        throw new System.Exception("no index");
    }
}