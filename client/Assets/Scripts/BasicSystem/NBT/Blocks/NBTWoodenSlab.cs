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
        CubeAttributes ca = InitCubeAttributes(chunk, blockData, pos);

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(nbtGO.nbtMesh, ca, 1);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(nbtGO.nbtMesh, ca, 1);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(nbtGO.nbtMesh, ca, 1);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(nbtGO.nbtMesh, ca, 1);
        }
        if (blockData < 8 || !chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(nbtGO.nbtMesh, ca, 1);
        }
        if (blockData >= 8 || !chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(nbtGO.nbtMesh, ca, 1);
        }
    }

    static Vector2 leftMid = new Vector2(0, 0.5f);
    static Vector2 rightMid = new Vector2(1, 0.5f);

    Vector2[] uv_full = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    Vector2[] uv_top = new Vector2[4] { leftMid, Vector2.up, Vector2.one, rightMid };
    Vector2[] uv_bot = new Vector2[4] { Vector2.zero, leftMid, rightMid, Vector2.right };

    protected void AddFace(NBTMesh mesh, Vector3Int pos, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, Vector2[] uv, int faceIndex, Color color)
    {
        ushort startIndex = mesh.vertexCount;

        SetVertex(mesh, pos1 + pos, faceIndex, uv[0], 1, color, Vector3.zero);
        SetVertex(mesh, pos2 + pos, faceIndex, uv[1], 1, color, Vector3.zero);
        SetVertex(mesh, pos3 + pos, faceIndex, uv[2], 1, color, Vector3.zero);
        SetVertex(mesh, pos4 + pos, faceIndex, uv[3], 1, color, Vector3.zero);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    protected override void AddFrontFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetFrontRotationByData(ca.blockData);
        if (ca.blockData >= 8)
        {
            AddFace(mesh, ca.pos, nearMiddleLeft, nearTopLeft, nearTopRight, nearMiddleRight, uv_top, ca.frontIndex, ca.frontColor);
        }
        else
        {
            AddFace(mesh, ca.pos, nearBottomLeft, nearMiddleLeft, nearMiddleRight, nearBottomRight, uv_bot, ca.frontIndex, ca.frontColor);
        }
    }

    protected override void AddBackFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetBackRotationByData(ca.blockData);
        if (ca.blockData >= 8)
            AddFace(mesh, ca.pos, farMiddleRight, farTopRight, farTopLeft, farMiddleLeft, uv_top, ca.backIndex, ca.backColor);
        else
            AddFace(mesh, ca.pos, farBottomRight, farMiddleRight, farMiddleLeft, farBottomLeft, uv_bot, ca.backIndex, ca.backColor);
    }

    protected override void AddTopFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetTopRotationByData(ca.blockData);
        if (ca.blockData >= 8)
            AddFace(mesh, ca.pos, farTopRight, nearTopRight, nearTopLeft, farTopLeft, uv_full, ca.topIndex, ca.topColor);
        else
            AddFace(mesh, ca.pos, farMiddleRight, nearMiddleRight, nearMiddleLeft, farMiddleLeft, uv_full, ca.topIndex, ca.topColor);
    }

    protected override void AddBottomFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetBottomRotationByData(ca.blockData);
        if (ca.blockData >= 8)
            AddFace(mesh, ca.pos, nearMiddleRight, farMiddleRight, farMiddleLeft, nearMiddleLeft, uv_full, ca.bottomIndex, ca.bottomColor);
        else
            AddFace(mesh, ca.pos, nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft, uv_full, ca.bottomIndex, ca.bottomColor);
    }

    protected override void AddLeftFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetLeftRotationByData(ca.blockData);
        if (ca.blockData >= 8)
            AddFace(mesh, ca.pos, farMiddleLeft, farTopLeft, nearTopLeft, nearMiddleLeft, uv_top, ca.leftIndex, ca.leftColor);
        else
            AddFace(mesh, ca.pos, farBottomLeft, farMiddleLeft, nearMiddleLeft, nearBottomLeft, uv_bot, ca.leftIndex, ca.leftColor);
    }

    protected override void AddRightFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetRightRotationByData(ca.blockData);
        if (ca.blockData >= 8)
            AddFace(mesh, ca.pos, nearMiddleRight, nearTopRight, farTopRight, farMiddleRight, uv_top, ca.rightIndex, ca.rightColor);
        else
            AddFace(mesh, ca.pos, nearBottomRight, nearMiddleRight, farMiddleRight, farBottomRight, uv_bot, ca.rightIndex, ca.rightColor);
    }
}
