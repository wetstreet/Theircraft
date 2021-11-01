using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenSlab : NBTBlock
{
    public override string name { get { return "Wooden Slab"; } }
    public override string id { get { return "minecraft:wooden_slab"; } }

    public override bool isTransparent { get { return true; } }

    public override string GetIconPathByData(short data)
    {
        if (data == 0) return "OakSlab";
        else if (data == 1) return "SpruceSlab";
        else if (data == 2) return "BirchSlab";
        else if (data == 3) return "JungleSlab";
        else return null;
    }

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Oak Wood Slab";
            case 1:
                return "Spruce Wood Slab";
            case 2:
                return "Birch Wood Slab";
            case 3:
                return "Jungle Wood Slab";
        }
        return "Wood Slab";
    }

    public override float hardness { get { return 2f; } }

    public override void Init()
    {
        UsedTextures = new string[] { "planks_oak", "planks_spruce", "planks_birch", "planks_jungle" };
    }

    int GetIndexByData(int data)
    {
        switch (data % 4)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("planks_oak");
            case 1:
                return TextureArrayManager.GetIndexByName("planks_spruce");
            case 2:
                return TextureArrayManager.GetIndexByName("planks_birch");
            case 3:
                return TextureArrayManager.GetIndexByName("planks_jungle");
        }
        return TextureArrayManager.GetIndexByName("planks_oak");
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetBottomIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetFrontIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetBackIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetLeftIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetRightIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
    {
        switch (data % 4)
        {
            case 0:
                return "planks_oak";
            case 1:
                return "planks_spruce";
            case 2:
                return "planks_birch";
            case 3:
                return "planks_jungle";
        }
        return null;
    }

    protected static Vector3 nearMiddleLeft = new Vector3(-0.5f, 0, -0.5f);
    protected static Vector3 farMiddleLeft = new Vector3(-0.5f, 0, 0.5f);
    protected static Vector3 nearMiddleRight = new Vector3(0.5f, 0, -0.5f);
    protected static Vector3 farMiddleRight = new Vector3(0.5f, 0, 0.5f);
    
    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (blockData < 8 || !chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (blockData >= 8 || !chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }

    static Vector2 leftMid = new Vector2(0, 0.5f);
    static Vector2 rightMid = new Vector2(1, 0.5f);

    Vector2[] uv_full = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    Vector2[] uv_top = new Vector2[4] { leftMid, Vector2.up, Vector2.one, rightMid };
    Vector2[] uv_bot = new Vector2[4] { Vector2.zero, leftMid, rightMid, Vector2.right };

    protected static Vector3[] frontVertices_top = new Vector3[] { nearMiddleLeft, nearTopLeft, nearTopRight, nearMiddleRight };
    protected static Vector3[] frontVertices_bottom = new Vector3[] { nearBottomLeft, nearMiddleLeft, nearMiddleRight, nearBottomRight };

    protected static Vector3[] backVertices_top = new Vector3[] { farMiddleRight, farTopRight, farTopLeft, farBottomLeft };
    protected static Vector3[] backVertices_bottom = new Vector3[] { farBottomRight, farMiddleRight, farMiddleLeft, farMiddleLeft };

    protected static Vector3[] topVertices_top = new Vector3[] { farTopRight, nearTopRight, nearTopLeft, farTopLeft };
    protected static Vector3[] topVertices_bottom = new Vector3[] { farMiddleRight, nearMiddleRight, nearMiddleLeft, farMiddleLeft };

    protected static Vector3[] bottomVertices_top = new Vector3[] { nearMiddleRight, farMiddleRight, farMiddleLeft, nearMiddleLeft };
    protected static Vector3[] bottomVertices_bottom = new Vector3[] { nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft };

    protected static Vector3[] leftVertices_top = new Vector3[] { farMiddleLeft, farTopLeft, nearTopLeft, nearMiddleLeft };
    protected static Vector3[] leftVertices_bottom = new Vector3[] { farBottomLeft, farMiddleLeft, nearMiddleLeft, nearBottomLeft };

    protected static Vector3[] rightVertices_top = new Vector3[] { nearMiddleRight, nearTopRight, farTopRight, farMiddleRight };
    protected static Vector3[] rightVertices_bottom = new Vector3[] { nearBottomRight, nearMiddleRight, farMiddleRight, farBottomRight };


    protected override FaceAttributes GetFrontFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z - 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = frontVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = frontVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
        fa.color = GetFrontTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.forward;

        return fa;
    }
    protected override FaceAttributes GetBackFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z + 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = backVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = backVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
        fa.color = GetBackTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.back;

        return fa;
    }
    protected override FaceAttributes GetTopFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y + 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
            fa.pos = topVertices_top;
        else
            fa.pos = topVertices_bottom;
        fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
        fa.color = GetTopTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.up;
        fa.uv = uv_full;

        return fa;
    }

    protected override FaceAttributes GetBottomFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y - 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
            fa.pos = bottomVertices_top;
        else
            fa.pos = bottomVertices_bottom;
        fa.pos = bottomVertices;
        fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
        fa.color = GetBottomTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.down;
        fa.uv = uv_full;

        return fa;
    }
    protected override FaceAttributes GetLeftFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x - 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = leftVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = leftVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
        fa.color = GetLeftTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.left;

        return fa;
    }
    protected override FaceAttributes GetRightFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x + 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        if (ca.blockData >= 8)
        {
            fa.pos = rightVertices_top;
            fa.uv = uv_top;
        }
        else
        {
            fa.pos = rightVertices_bottom;
            fa.uv = uv_bot;
        }
        fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
        fa.color = GetRightTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.right;

        return fa;
    }
}
