using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLadder : NBTBlock
{
    public override string name => "Ladder";
    public override string id => "minecraft:ladder";

    public override bool isTransparent => true;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;
    public override float hardness => 0.4f;

    public override string pathPrefix => "GUI/block/";
    public override string GetIconPathByData(short data) { return "ladder"; }

    public override byte GetDropItemData(byte data) { return 0; }

    public override string GetBreakEffectTexture(byte data) { return "ladder"; }

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

    void FillMesh(NBTChunk chunk, CubeAttributes ca, NBTMesh nbtMesh)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.color = Color.white;
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;
        fa.uv = TextureArrayManager.GetUVByName("ladder");

        if (ca.blockData == 2)
        {
            fa.pos = frontFace;
            AddFace(nbtMesh, fa, ca);
        }
        else if (ca.blockData == 3)
        {
            fa.pos = backFace;
            AddFace(nbtMesh, fa, ca);
        }
        else if (ca.blockData == 4)
        {
            fa.pos = leftFace;
            AddFace(nbtMesh, fa, ca);
        }
        else if (ca.blockData == 5)
        {
            fa.pos = rightFace;
            AddFace(nbtMesh, fa, ca);
        }
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

    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(data))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(data));
            mat.mainTexture = tex;
            itemMaterialDict.Add(data, mat);
        }
        return itemMaterialDict[data];
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        if (!itemMeshDict.ContainsKey(0))
        {
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(0));
            Mesh mesh = ItemMeshGenerator.instance.Generate(tex);
            itemMeshDict.Add(0, mesh);
        }
        return itemMeshDict[0];
    }

    public override void OnAddBlock(RaycastHit hit)
    {
        if (hit.normal.y != 0)
        {
            return;
        }

        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte type = NBTGeneratorManager.id2type[id];
        byte data = 0;

        if (hit.normal == Vector3.back)
        {
            data = 2;
        }
        else if (hit.normal == Vector3.forward)
        {
            data = 3;
        }
        else if (hit.normal == Vector3.left)
        {
            data = 4;
        }
        else if (hit.normal == Vector3.right)
        {
            data = 5;
        }
        NBTHelper.SetBlockData(pos, type, data);
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