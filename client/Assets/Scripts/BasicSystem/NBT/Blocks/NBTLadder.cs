using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLadder : NBTBlock
{
    public override string name { get { return "Ladder"; } }
    public override string id { get { return "minecraft:ladder"; } }

    public override string GetIconPathByData(short data) { return "ladder"; }

    public override void Init()
    {
        UsedTextures = new string[] { "ladder" };
    }

    public override string GetBreakEffectTexture(byte data) { return "ladder"; }

    public override bool isTransparent { get { return true; } }

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;
    public override float hardness => 0.4f;

    protected static Vector3 nearBottomLeft_1 = new Vector3(-0.4375f, -0.5f, -0.4375f);
    protected static Vector3 nearBottomRight_1 = new Vector3(0.4375f, -0.5f, -0.4375f);
    protected static Vector3 nearTopLeft_1 = new Vector3(-0.4375f, 0.5f, -0.4375f);
    protected static Vector3 nearTopRight_1 = new Vector3(0.4375f, 0.5f, -0.4375f);
    protected static Vector3 farBottomLeft_1 = new Vector3(-0.4375f, -0.5f, 0.4375f);
    protected static Vector3 farBottomRight_1 = new Vector3(0.4375f, -0.5f, 0.4375f);
    protected static Vector3 farTopLeft_1 = new Vector3(-0.4375f, 0.5f, 0.4375f);
    protected static Vector3 farTopRight_1 = new Vector3(0.4375f, 0.5f, 0.4375f);

    protected static Vector3[] frontFace = new Vector3[] { farBottomLeft_1, farTopLeft_1, farTopRight_1, farBottomRight_1 };
    protected static Vector3[] backFace = new Vector3[] { nearBottomRight_1, nearTopRight_1, nearTopLeft_1, nearBottomLeft_1 };
    protected static Vector3[] leftFace = new Vector3[] { farBottomRight_1, farTopRight_1, nearTopRight_1, nearBottomRight_1 };
    protected static Vector3[] rightFace = new Vector3[] { nearBottomLeft_1, nearTopLeft_1, farTopLeft_1, farBottomLeft_1 };

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.faceIndex = TextureArrayManager.GetIndexByName("ladder");
        fa.color = Color.white;
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;
        fa.uv = uv_zero;

        try
        {
            if (ca.blockData == 2)
            {
                fa.pos = frontFace;
                AddFace(nbtGO.nbtMesh, fa, ca);
            }
            else if (ca.blockData == 3)
            {
                fa.pos = backFace;
                AddFace(nbtGO.nbtMesh, fa, ca);
            }
            else if (ca.blockData == 4)
            {
                fa.pos = leftFace;
                AddFace(nbtGO.nbtMesh, fa, ca);
            }
            else if (ca.blockData == 5)
            {
                fa.pos = rightFace;
                AddFace(nbtGO.nbtMesh, fa, ca);
            }
            else
            {
                throw new System.Exception();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString() + "\n" + "pos=" + pos + ",data=" + blockData);
        }
    }
}