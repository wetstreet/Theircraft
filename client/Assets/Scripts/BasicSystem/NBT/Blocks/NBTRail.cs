using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRail : NBTBlock
{
    public override string name => "Rail";
    public override string id => "minecraft:rail";

    public override bool isTransparent => true;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;
    public override float hardness => 0.7f;

    public override string pathPrefix => "GUI/block/";
    public override string GetIconPathByData(short data) { return "rail_normal"; }

    public override byte GetDropItemData(byte data) { return 0; }

    public override string GetBreakEffectTexture(byte data) { return "rail_normal"; }

    protected static Vector3 nearTopLeft_1 = new Vector3(-0.5f, -0.375f, -0.5f);
    protected static Vector3 nearTopRight_1 = new Vector3(0.5f, -0.375f, -0.5f);
    protected static Vector3 farTopLeft_1 = new Vector3(-0.5f, -0.375f, 0.5f);
    protected static Vector3 farTopRight_1 = new Vector3(0.5f, -0.375f, 0.5f);

    protected static Vector3[] topFace = new Vector3[] { farTopRight_1, nearTopRight_1, nearTopLeft_1, farTopLeft_1 };

    void FillMesh(NBTChunk chunk, CubeAttributes ca, NBTMesh nbtMesh)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.color = Color.white;
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;
        fa.uv = TextureArrayManager.GetUVByName("ladder");

        fa.pos = topFace;
        AddFace(nbtMesh, fa, ca);
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        FillMesh(chunk, ca, nbtGO.nbtMesh);
    }

    public override Mesh GetBreakingEffectMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = chunk.ca;
        ca.worldPos = pos;
        ca.blockData = blockData;
        ca.isBreakingMesh = true;

        NBTMesh nbtMesh = new NBTMesh(256);

        FillMesh(chunk, ca, nbtMesh);

        nbtMesh.Refresh();
        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }

    public override void RenderWireframe(byte blockData)
    {

        float top, bottom, left, right, front, back;
        top = 0.501f;
        bottom = -0.501f;
        if (blockData == 2) // front
        {
            left = -0.501f;
            right = 0.501f;
            front = 0.501f;
            back = 0.3115f;
        }
        else if (blockData == 3) // back
        {
            left = -0.501f;
            right = 0.501f;
            front = -0.3115f;
            back = -0.501f;
        }
        else if (blockData == 4) // left
        {
            left = 0.3115f;
            right = 0.501f;
            front = 0.501f;
            back = -0.501f;
        }
        else if (blockData == 5) // right
        {
            left = -0.501f;
            right = -0.3115f;
            front = 0.501f;
            back = -0.501f;
        }
        else
        {
            left = -0.501f;
            right = 0.501f;
            front = 0.501f;
            back = -0.501f;
        }

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}
