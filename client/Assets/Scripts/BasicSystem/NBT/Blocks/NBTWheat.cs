using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWheat : NBTPlant
{
    public override string name { get { return "Wheat"; } }
    public override string id { get { return "minecraft:wheat"; } }

    public override string pathPrefix { get { return "GUI/items/"; } }

    public override string GetIconPathByData(short data) { return "wheat"; }

    public override void Init()
    {
        UsedTextures = new string[] { "wheat_stage_7" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("wheat_stage_7");
    }

    protected override string itemMeshPath { get { return "wheat"; } }

    public override string GetBreakEffectTexture(byte data) { return "wheat"; }

    protected static Vector3 nearBottomLeft_1 = new Vector3(-0.5f, -0.5f, -0.25f);
    protected static Vector3 nearBottomRight_1 = new Vector3(0.5f, -0.5f, -0.25f);
    protected static Vector3 nearTopLeft_1 = new Vector3(-0.5f, 0.5f, -0.25f);
    protected static Vector3 nearTopRight_1 = new Vector3(0.5f, 0.5f, -0.25f);
    protected static Vector3 farBottomLeft_1 = new Vector3(-0.5f, -0.5f, 0.25f);
    protected static Vector3 farBottomRight_1 = new Vector3(0.5f, -0.5f, 0.25f);
    protected static Vector3 farTopLeft_1 = new Vector3(-0.5f, 0.5f, 0.25f);
    protected static Vector3 farTopRight_1 = new Vector3(0.5f, 0.5f, 0.25f);

    protected static Vector3 nearBottomLeft_2 = new Vector3(-0.25f, -0.5f, -0.5f);
    protected static Vector3 nearBottomRight_2 = new Vector3(0.25f, -0.5f, -0.5f);
    protected static Vector3 nearTopLeft_2 = new Vector3(-0.25f, 0.5f, -0.5f);
    protected static Vector3 nearTopRight_2 = new Vector3(0.25f, 0.5f, -0.5f);
    protected static Vector3 farBottomLeft_2 = new Vector3(-0.25f, -0.5f, 0.5f);
    protected static Vector3 farBottomRight_2 = new Vector3(0.25f, -0.5f, 0.5f);
    protected static Vector3 farTopLeft_2 = new Vector3(-0.25f, 0.5f, 0.5f);
    protected static Vector3 farTopRight_2 = new Vector3(0.25f, 0.5f, 0.5f);

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        plantIndex = GetPlantIndexByData(chunk, blockData);
        tintColor = GetTintColorByData(chunk, blockData);

        float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z);

        try
        {
            AddFace(nbtGO.nbtMesh, pos, nearBottomLeft_1, nearTopLeft_1, nearTopRight_1, nearBottomRight_1, plantIndex, Color.white, skyLight, Vector3.zero);
            AddFace(nbtGO.nbtMesh, pos, farBottomLeft_1, farTopLeft_1, farTopRight_1, farBottomRight_1, plantIndex, Color.white, skyLight, Vector3.zero);

            AddFace(nbtGO.nbtMesh, pos, farBottomLeft_2, farTopLeft_2, nearTopLeft_2, nearBottomLeft_2, plantIndex, Color.white, skyLight, Vector3.zero);
            AddFace(nbtGO.nbtMesh, pos, farBottomRight_2, farTopRight_2, nearTopRight_2, nearBottomRight_2, plantIndex, Color.white, skyLight, Vector3.zero);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString() + "\n" + "pos=" + pos + ",data=" + blockData);
        }
    }
}
