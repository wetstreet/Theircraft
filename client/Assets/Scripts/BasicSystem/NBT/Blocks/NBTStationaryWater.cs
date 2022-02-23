using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStationaryWater : NBTBlock
{
    public override bool isTransparent => true;

    public override bool willReduceLight => true;

    static byte TYPE_WATER = 9;

    protected static Vector3 nearTopLeft_still = new Vector3(-0.5f, 0.4f, -0.5f);
    protected static Vector3 nearTopRight_still = new Vector3(0.5f, 0.4f, -0.5f);
    protected static Vector3 farTopLeft_still = new Vector3(-0.5f, 0.4f, 0.5f);
    protected static Vector3 farTopRight_still = new Vector3(0.5f, 0.4f, 0.5f);

    bool IsVerticalWater(byte data, byte type = 9)
    {
        return type == TYPE_WATER && data >= 9 && data <= 15;
    }

    protected FaceAttributes fa = new FaceAttributes()
    {
        pos = new Vector3[4]
    };
    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        chunk.GetBlockData(pos.x, pos.y, pos.z - 1, out byte frontType, out byte frontData);
        chunk.GetBlockData(pos.x, pos.y, pos.z + 1, out byte backType, out byte backData);
        chunk.GetBlockData(pos.x - 1, pos.y, pos.z, out byte leftType, out byte leftData);
        chunk.GetBlockData(pos.x + 1, pos.y, pos.z, out byte rightType, out byte rightData);
        chunk.GetBlockData(pos.x, pos.y - 1, pos.z, out byte bottomType, out byte bottomData);
        chunk.GetBlockData(pos.x, pos.y + 1, pos.z, out byte topType, out byte topData);

        bool selfIsVerticalWater = IsVerticalWater(blockData);

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        fa.color = Color.white;
        fa.skyLight = skylight_default;
        fa.blockLight = blocklight_default;
        fa.uv = uv_zero;

        if (NBTGeneratorManager.IsTransparent(frontType) && frontType != TYPE_WATER)
        {
            if (selfIsVerticalWater)
            {
                fa.pos[0] = nearBottomLeft;
                fa.pos[1] = nearTopLeft;
                fa.pos[2] = nearTopRight;
                fa.pos[3] = nearBottomRight;
            }
            else
            {
                fa.pos[0] = nearBottomLeft;
                fa.pos[1] = nearTopLeft_still;
                fa.pos[2] = nearTopRight_still;
                fa.pos[3] = nearBottomRight;
            }
            fa.normal = Vector3.forward;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (NBTGeneratorManager.IsTransparent(backType) && backType != TYPE_WATER)
        {
            if (selfIsVerticalWater)
            {
                fa.pos[0] = farBottomRight;
                fa.pos[1] = farTopRight;
                fa.pos[2] = farTopLeft;
                fa.pos[3] = farBottomLeft;
            }
            else
            {
                fa.pos[0] = farBottomRight;
                fa.pos[1] = farTopRight_still;
                fa.pos[2] = farTopLeft_still;
                fa.pos[3] = farBottomLeft;
            }
            fa.normal = Vector3.back;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (NBTGeneratorManager.IsTransparent(leftType) && leftType != TYPE_WATER)
        {
            if (selfIsVerticalWater)
            {
                fa.pos[0] = farBottomLeft;
                fa.pos[1] = farTopLeft;
                fa.pos[2] = nearTopLeft;
                fa.pos[3] = nearBottomLeft;
            }
            else
            {
                fa.pos[0] = farBottomLeft;
                fa.pos[1] = farTopLeft_still ;
                fa.pos[2] = nearTopLeft_still;
                fa.pos[3] = nearBottomLeft;
            }
            fa.normal = Vector3.left;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (NBTGeneratorManager.IsTransparent(rightType) && rightType != TYPE_WATER)
        {
            if (selfIsVerticalWater)
            {
                fa.pos[0] = nearBottomRight;
                fa.pos[1] = nearTopRight;
                fa.pos[2] = farTopRight;
                fa.pos[3] = farBottomRight;
            }
            else
            {
                fa.pos[0] = nearBottomRight;
                fa.pos[1] = nearTopRight_still;
                fa.pos[2] = farTopRight_still;
                fa.pos[3] = farBottomRight;
            }
            fa.normal = Vector3.right;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (NBTGeneratorManager.IsTransparent(topType) && topType != TYPE_WATER)
        {
            if (selfIsVerticalWater)
            {
                fa.pos[0] = farTopRight;
                fa.pos[1] = nearTopRight;
                fa.pos[2] = nearTopLeft;
                fa.pos[3] = farTopLeft;
            }
            else
            {
                bool leftIsVerticalWater = IsVerticalWater(leftData, leftType);
                bool rightIsVerticalWater = IsVerticalWater(rightData, rightType);
                bool frontIsVerticalWater = IsVerticalWater(frontData, frontType);
                bool backIsVerticalWater = IsVerticalWater(backData, backType);

                fa.pos[0] = backIsVerticalWater || rightIsVerticalWater ? farTopRight : farTopRight_still;
                fa.pos[1] = frontIsVerticalWater || rightIsVerticalWater ? nearTopRight : nearTopRight_still;
                fa.pos[2] = frontIsVerticalWater || leftIsVerticalWater ? nearTopLeft : nearTopLeft_still;
                fa.pos[3] = backIsVerticalWater || leftIsVerticalWater ? farTopLeft : farTopLeft_still;
            }
            fa.normal = Vector3.up;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (NBTGeneratorManager.IsTransparent(bottomType) && bottomType != TYPE_WATER)
        {
            fa.pos[0] = nearBottomRight;
            fa.pos[1] = farBottomRight;
            fa.pos[2] = farBottomLeft;
            fa.pos[3] = nearBottomLeft;
            fa.normal = Vector3.down;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }
}
