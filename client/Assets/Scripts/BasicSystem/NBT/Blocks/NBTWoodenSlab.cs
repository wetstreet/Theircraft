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
        topIndex = GetTopIndexByData(chunk, blockData);
        bottomIndex = GetBottomIndexByData(chunk, blockData);
        frontIndex = GetFrontIndexByData(chunk, blockData);
        backIndex = GetBackIndexByData(chunk, blockData);
        leftIndex = GetLeftIndexByData(chunk, blockData);
        rightIndex = GetRightIndexByData(chunk, blockData);

        topColor = GetTopTintColorByData(chunk, pos, blockData);
        bottomColor = GetBottomTintColorByData(chunk, pos, blockData);
        frontColor = GetFrontTintColorByData(chunk, pos, blockData);
        backColor = GetBackTintColorByData(chunk, pos, blockData);
        leftColor = GetLeftTintColorByData(chunk, pos, blockData);
        rightColor = GetRightTintColorByData(chunk, pos, blockData);

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(nbtGO.nbtMesh, pos, blockData);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(nbtGO.nbtMesh, pos, blockData);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(nbtGO.nbtMesh, pos, blockData);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(nbtGO.nbtMesh, pos, blockData);
        }
        if (blockData < 8 || !chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(nbtGO.nbtMesh, pos, blockData);
        }
        if (blockData >= 8 || !chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(nbtGO.nbtMesh, pos, blockData);
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

        SetVertex(mesh, ToVector4(pos1 + pos, faceIndex), uv[0], color);
        SetVertex(mesh, ToVector4(pos2 + pos, faceIndex), uv[1], color);
        SetVertex(mesh, ToVector4(pos3 + pos, faceIndex), uv[2], color);
        SetVertex(mesh, ToVector4(pos4 + pos, faceIndex), uv[3], color);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    protected override void AddFrontFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetFrontRotationByData(blockData);
        if (blockData >= 8)
        {
            AddFace(mesh, pos, nearMiddleLeft, nearTopLeft, nearTopRight, nearMiddleRight, uv_top, frontIndex, frontColor);
        }
        else
        {
            AddFace(mesh, pos, nearBottomLeft, nearMiddleLeft, nearMiddleRight, nearBottomRight, uv_bot, frontIndex, frontColor);
        }
    }

    protected override void AddBackFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetBackRotationByData(blockData);
        if (blockData >= 8)
            AddFace(mesh, pos, farMiddleRight, farTopRight, farTopLeft, farMiddleLeft, uv_top, backIndex, backColor);
        else
            AddFace(mesh, pos, farBottomRight, farMiddleRight, farMiddleLeft, farBottomLeft, uv_bot, backIndex, backColor);
    }

    protected override void AddTopFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetTopRotationByData(blockData);
        if (blockData >= 8)
            AddFace(mesh, pos, farTopRight, nearTopRight, nearTopLeft, farTopLeft, uv_full, topIndex, topColor);
        else
            AddFace(mesh, pos, farMiddleRight, nearMiddleRight, nearMiddleLeft, farMiddleLeft, uv_full, topIndex, topColor);
    }

    protected override void AddBottomFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetBottomRotationByData(blockData);
        if (blockData >= 8)
            AddFace(mesh, pos, nearMiddleRight, farMiddleRight, farMiddleLeft, nearMiddleLeft, uv_full, bottomIndex, bottomColor);
        else
            AddFace(mesh, pos, nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft, uv_full, bottomIndex, bottomColor);
    }

    protected override void AddLeftFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetLeftRotationByData(blockData);
        if (blockData >= 8)
            AddFace(mesh, pos, farMiddleLeft, farTopLeft, nearTopLeft, nearMiddleLeft, uv_top, leftIndex, leftColor);
        else
            AddFace(mesh, pos, farBottomLeft, farMiddleLeft, nearMiddleLeft, nearBottomLeft, uv_bot, leftIndex, leftColor);
    }

    protected override void AddRightFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetRightRotationByData(blockData);
        if (blockData >= 8)
            AddFace(mesh, pos, nearMiddleRight, nearTopRight, farTopRight, farMiddleRight, uv_top, rightIndex, rightColor);
        else
            AddFace(mesh, pos, nearBottomRight, nearMiddleRight, farMiddleRight, farBottomRight, uv_bot, rightIndex, rightColor);
    }
}
