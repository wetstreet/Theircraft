using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGrassPath : NBTBlock
{
    public override string name => "Grass Path";
    public override string id => "minecraft:grass_path";

    public override string GetDropItemByData(byte data) { return "minecraft:dirt"; }

    public override bool isTransparent => true;

    public override string topName => "grass_path_top";
    public override string bottomName => "dirt";
    public override string frontName => "grass_path_side";
    public override string backName => "grass_path_side";
    public override string leftName => "grass_path_side";
    public override string rightName => "grass_path_side";

    public override float hardness => 0.65f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;
    public override SoundMaterial soundMaterial => SoundMaterial.Grass;

    public override Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }

    protected static Vector3 nearTopLeft = new Vector3(-0.5f, 0.4375f, -0.5f);
    protected static Vector3 nearTopRight = new Vector3(0.5f, 0.4375f, -0.5f);
    protected static Vector3 farTopLeft = new Vector3(-0.5f, 0.4375f, 0.5f);
    protected static Vector3 farTopRight = new Vector3(0.5f, 0.4375f, 0.5f);

    protected static Vector3[] frontVertices = new Vector3[] { nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight };
    protected static Vector3[] backVertices = new Vector3[] { farBottomRight, farTopRight, farTopLeft, farBottomLeft };
    protected static Vector3[] topVertices = new Vector3[] { farTopRight, nearTopRight, nearTopLeft, farTopLeft };
    protected static Vector3[] leftVertices = new Vector3[] { farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft };
    protected static Vector3[] rightVertices = new Vector3[] { nearBottomRight, nearTopRight, farTopRight, farBottomRight };

    static Vector2[] _uv_side;
    protected static Vector2[] uv_side
    {
        get
        {
            if (_uv_side == null)
            {
                _uv_side = new Vector2[4] { Vector2.zero, new Vector2(0, 0.9375f), new Vector2(1, 0.9375f), Vector2.right };
            }
            return _uv_side;
        }
    }


    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        InitBlockAttributes(chunk, ref ca);

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            fa.pos = frontVertices;
            fa.uv = uv_side;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            fa.pos = rightVertices;
            fa.uv = uv_side;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            fa.pos = leftVertices;
            fa.uv = uv_side;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            fa.pos = backVertices;
            fa.uv = uv_side;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            fa.pos = topVertices;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }

    public override Mesh GetItemMesh(byte data = 0)
    {
        CubeAttributes ca = new CubeAttributes();

        NBTMesh nbtMesh = new NBTMesh(256);

        FaceAttributes fa = new FaceAttributes();
        //fa.skyLight = skylight_default;
        //fa.blockLight = blocklight_default;
        fa.color = Color.white;
        fa.uv = uv_zero;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            fa.uv = TextureArrayManager.GetUVByName(frontName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.uv = TextureArrayManager.GetUVByName(backName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.uv = TextureArrayManager.GetUVByName(topName);
            fa.color = TintManager.tintColor;
            AddFace(nbtMesh, fa, ca);
            fa.color = Color.white;

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.uv = TextureArrayManager.GetUVByName(bottomName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.uv = TextureArrayManager.GetUVByName(leftName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.uv = TextureArrayManager.GetUVByName(rightName);
            AddFace(nbtMesh, fa, ca);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetItemMesh error,generator=" + GetType() + ",message=\n" + e.Message);
        }

        nbtMesh.Refresh();

        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }
}