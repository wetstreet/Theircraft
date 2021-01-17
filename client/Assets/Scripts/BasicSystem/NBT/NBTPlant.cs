using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTPlant : NBTBlock
{
    protected int plantIndex;

    public virtual int GetPlantIndexByData(NBTChunk chunk, int data) { return GetPlantIndexByData(data); }

    public virtual int GetPlantIndexByData(int data) { return 0; }

    public override float breakNeedTime { get { return 0; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    public override bool hasDropItem { get { return false; } }

    public override void AddCube(NBTChunk chunk, byte blockData, byte skyLight, Vector3Int pos, NBTGameObject nbtGO)
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

    public override Color GetFrontTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(chunk, data); }

    void AddDiagonalFace()
    {
        AddFace(farBottomLeft, farTopLeft, nearTopRight, nearBottomRight, plantIndex, tintColor);
    }

    void AddAntiDiagonalFace()
    {
        AddFace(nearBottomLeft, nearTopLeft, farTopRight, farBottomRight, plantIndex, tintColor);
    }

    protected virtual string itemMeshPath { get { return null; } }

    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    {
        if (!string.IsNullOrEmpty(itemMeshPath))
        {
            return Resources.Load<Mesh>("Meshes/items/" + itemMeshPath + "/" + itemMeshPath);
        }
        return null;
    }
}
