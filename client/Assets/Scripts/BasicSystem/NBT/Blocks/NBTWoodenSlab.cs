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
    
    public override void AddCube(NBTChunk chunk, byte blockData, byte skyLight, Vector3Int pos, NBTGameObject nbtGO)
    {
        this.pos = pos;
        this.blockData = blockData;
        vertices = nbtGO.vertexList;
        triangles = nbtGO.triangles;

        topIndex = GetTopIndexByData(chunk, blockData);
        bottomIndex = GetBottomIndexByData(chunk, blockData);
        frontIndex = GetFrontIndexByData(chunk, blockData);
        backIndex = GetBackIndexByData(chunk, blockData);
        leftIndex = GetLeftIndexByData(chunk, blockData);
        rightIndex = GetRightIndexByData(chunk, blockData);

        topColor = GetTopTintColorByData(chunk, blockData);
        bottomColor = GetBottomTintColorByData(chunk, blockData);
        frontColor = GetFrontTintColorByData(chunk, blockData);
        backColor = GetBackTintColorByData(chunk, blockData);
        leftColor = GetLeftTintColorByData(chunk, blockData);
        rightColor = GetRightTintColorByData(chunk, blockData);

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(blockData);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(blockData);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(blockData);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(blockData);
        }
        if (blockData < 8 || !chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(blockData);
        }
        if (blockData >= 8 || !chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(blockData);
        }
    }

    static Vector2 leftMid = new Vector2(0, 0.5f);
    static Vector2 rightMid = new Vector2(1, 0.5f);

    Vector2[] uv_full = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    Vector2[] uv_top = new Vector2[4] { leftMid, Vector2.up, Vector2.one, rightMid };
    Vector2[] uv_bot = new Vector2[4] { Vector2.zero, leftMid, rightMid, Vector2.right };

    protected void AddFace(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, Vector2[] uv, int faceIndex, Color color)
    {
        vertices.Add(new Vertex { pos = ToVector4(pos1 + pos, faceIndex), texcoord = uv[0], color = color });
        vertices.Add(new Vertex { pos = ToVector4(pos2 + pos, faceIndex), texcoord = uv[1], color = color });
        vertices.Add(new Vertex { pos = ToVector4(pos3 + pos, faceIndex), texcoord = uv[2], color = color });
        vertices.Add(new Vertex { pos = ToVector4(pos4 + pos, faceIndex), texcoord = uv[3], color = color });

        int startIndex = vertices.Count - 4;
        triangles.AddRange(new int[] {
            startIndex, startIndex + 1, startIndex + 2,
            startIndex, startIndex + 2, startIndex + 3
        });
    }

    protected override void AddFrontFace(byte data)
    {
        rotation = GetFrontRotationByData(blockData);
        if (data >= 8)
        {
            AddFace(nearMiddleLeft, nearTopLeft, nearTopRight, nearMiddleRight, uv_top, frontIndex, frontColor);
        }
        else
        {
            AddFace(nearBottomLeft, nearMiddleLeft, nearMiddleRight, nearBottomRight, uv_bot, frontIndex, frontColor);
        }
    }

    protected override void AddBackFace(byte data)
    {
        rotation = GetBackRotationByData(blockData);
        if (data >= 8)
            AddFace(farMiddleRight, farTopRight, farTopLeft, farMiddleLeft, uv_top, backIndex, backColor);
        else
            AddFace(farBottomRight, farMiddleRight, farMiddleLeft, farBottomLeft, uv_bot, backIndex, backColor);
    }

    protected override void AddTopFace(byte data)
    {
        rotation = GetTopRotationByData(blockData);
        if (data >= 8)
            AddFace(farTopRight, nearTopRight, nearTopLeft, farTopLeft, uv_full, topIndex, topColor);
        else
            AddFace(farMiddleRight, nearMiddleRight, nearMiddleLeft, farMiddleLeft, uv_full, topIndex, topColor);
    }

    protected override void AddBottomFace(byte data)
    {
        rotation = GetBottomRotationByData(blockData);
        if (data >= 8)
            AddFace(nearMiddleRight, farMiddleRight, farMiddleLeft, nearMiddleLeft, uv_full, bottomIndex, bottomColor);
        else
            AddFace(nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft, uv_full, bottomIndex, bottomColor);
    }

    protected override void AddLeftFace(byte data)
    {
        rotation = GetLeftRotationByData(blockData);
        if (data >= 8)
            AddFace(farMiddleLeft, farTopLeft, nearTopLeft, nearMiddleLeft, uv_top, leftIndex, leftColor);
        else
            AddFace(farBottomLeft, farMiddleLeft, nearMiddleLeft, nearBottomLeft, uv_bot, leftIndex, leftColor);
    }

    protected override void AddRightFace(byte data)
    {
        rotation = GetRightRotationByData(blockData);
        if (data >= 8)
            AddFace(nearMiddleRight, nearTopRight, farTopRight, farMiddleRight, uv_top, rightIndex, rightColor);
        else
            AddFace(nearBottomRight, nearMiddleRight, farMiddleRight, farBottomRight, uv_bot, rightIndex, rightColor);
    }
}
