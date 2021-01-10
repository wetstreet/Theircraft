using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPlant : NBTBlock
{
    protected int plantIndex;

    public virtual int GetPlantIndexByData(NBTChunk chunk, int data) { return GetPlantIndexByData(data); }

    public virtual int GetPlantIndexByData(int data) { return 0; }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    public override void ClearData()
    {

    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        this.pos = pos;
        vertices = nbtGO.vertexList;
        triangles = nbtGO.triangles;

        plantIndex = GetPlantIndexByData(chunk, blockData);
        tintColor = GetTintColorByData(chunk, blockData);

        try
        {
            AddDiagonalFace();
            AddAntiDiagonalFace();

        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString() + "\n" + "pos=" + pos + ",data=" + blockData);
        }
    }

    protected Color tintColor;

    protected virtual Color GetTintColorByData(NBTChunk chunk, byte data) { return Color.white; }

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
