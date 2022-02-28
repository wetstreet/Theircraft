using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCrops : NBTPlant
{

    public override string pathPrefix { get { return "GUI/items/"; } }

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
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.color = Color.white;
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;
        fa.uv = TextureArrayManager.GetUVByName(GetTexName(chunk, pos, blockData));

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
