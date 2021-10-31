using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCarrots : NBTPlant
{
    public override string name { get { return "Carrot"; } }
    public override string id { get { return "minecraft:carrots"; } }

    public override string pathPrefix { get { return "GUI/items/"; } }

    public override string GetIconPathByData(short data) { return "carrot"; }

    public override void Init()
    {
        UsedTextures = new string[] { "carrots_stage_3" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("carrots_stage_3");
    }

    public override string GetBreakEffectTexture(byte data) { return "carrot"; }

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

    static Vector3[] face1 = new Vector3[4] { nearBottomLeft_1, nearTopLeft_1, nearTopRight_1, nearBottomRight_1 };
    static Vector3[] face2 = new Vector3[4] { farBottomLeft_1, farTopLeft_1, farTopRight_1, farBottomRight_1 };
    static Vector3[] face3 = new Vector3[4] { farBottomLeft_2, farTopLeft_2, nearTopLeft_2, nearBottomLeft_2 };
    static Vector3[] face4 = new Vector3[4] { farBottomRight_2, farTopRight_2, nearTopRight_2, nearBottomRight_2 };

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa;
        fa.faceIndex = GetPlantIndexByData(chunk, blockData);
        fa.color = Color.white;
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.zero;
        fa.uv = uv_zero;

        try
        {
            fa.pos = face1;
            AddFace(nbtGO.nbtMesh, fa, ca);
            fa.pos = face2;
            AddFace(nbtGO.nbtMesh, fa, ca);

            fa.pos = face3;
            AddFace(nbtGO.nbtMesh, fa, ca);
            fa.pos = face4;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString() + "\n" + "pos=" + pos + ",data=" + blockData);
        }
    }
}
