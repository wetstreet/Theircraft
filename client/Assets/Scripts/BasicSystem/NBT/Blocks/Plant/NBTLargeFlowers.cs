using System.Collections;
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

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        AddDiagonalFace(chunk, nbtGO.nbtMesh, ca);
        AddAntiDiagonalFace(chunk, nbtGO.nbtMesh, ca);

        if (blockData == 10)
        {
            chunk.GetBlockData(pos.x, pos.y - 1, pos.z, out byte bottomType, out byte bottomData);
            if (bottomData == 0)
            {
                AddSunflowerFace(chunk, nbtGO.nbtMesh, ca);
            }
        }
    }

    FaceAttributes sunflowerFA = new FaceAttributes()
    {
        skyLight = new float[4],
        blockLight = new float[4],
    };

    Vector3[] sunflowerFace = new Vector3[] {   new Vector3(-0.5f, -0.5f, -0.5f),
                                                new Vector3(-0.5f, 0.5f, 0f),
                                                new Vector3(0.5f, 0.5f, 0f),
                                                new Vector3(0.5f, -0.5f, -0.5f) };
    protected void AddSunflowerFace(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        sunflowerFA.pos = sunflowerFace;
        sunflowerFA.color = Color.white;
        sunflowerFA.skyLight[0] = skyLight;
        sunflowerFA.skyLight[1] = skyLight;
        sunflowerFA.skyLight[2] = skyLight;
        sunflowerFA.skyLight[3] = skyLight;
        sunflowerFA.blockLight[0] = blockLight;
        sunflowerFA.blockLight[1] = blockLight;
        sunflowerFA.blockLight[2] = blockLight;
        sunflowerFA.blockLight[3] = blockLight;
        sunflowerFA.normal = Vector3.zero;
        sunflowerFA.uv = TextureArrayManager.GetUVByName("double_plant_sunflower_front");

        AddFace(mesh, sunflowerFA, ca);
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