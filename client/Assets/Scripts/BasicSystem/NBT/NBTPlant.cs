using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPlant : NBTBlock
{
    public override void ClearData()
    {

    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        this.pos = pos;
        vertices = nbtGO.vertexList;
        triangles = nbtGO.triangles;

        AddDiagonalFace();
        AddAntiDiagonalFace();
    }

    protected virtual Color tintColor { get { return Color.white; } }

    void AddDiagonalFace()
    {
        AddFace(farBottomLeft, farTopLeft, nearTopRight, nearBottomRight, plantIndex, tintColor);
        //AddFace(farBottomLeft, farTopLeft, nearTopRight, nearBottomRight, plantIndex);
    }

    void AddAntiDiagonalFace()
    {
        AddFace(nearBottomLeft, nearTopLeft, farTopRight, farBottomRight, plantIndex, tintColor);
        //AddFace(nearBottomLeft, nearTopLeft, farTopRight, farBottomRight, plantIndex);
    }
}
