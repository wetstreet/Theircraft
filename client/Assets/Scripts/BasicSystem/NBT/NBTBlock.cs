using protocol.cs_theircraft;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct Vertex
{
    public Vector4 pos;
    public Vector4 color;
    public Vector2 texcoord;
}

public enum Rotation
{
    Zero,
    Right,
}

public abstract class NBTBlock : NBTObject
{

    public virtual string topName { get; }
    public virtual string bottomName { get; }
    public virtual string frontName { get; }
    public virtual string backName { get; }
    public virtual string leftName { get; }
    public virtual string rightName { get; }

    public virtual float hardness { get { return 1; } }

    public virtual float speedMultiplier { get { return 3; } }

    public virtual float breakNeedTime {
        get {
            float damage = speedMultiplier / hardness / 100;
            if (damage > 1) return 0;
            int ticks = Mathf.CeilToInt(1 / damage);
            return ticks / 20f;
        }
    }

    public int topIndex;
    public int bottomIndex;
    public int frontIndex;
    public int backIndex;
    public int leftIndex;
    public int rightIndex;

    public virtual int GetTopIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(topName); }
    public virtual int GetBottomIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(bottomName); }
    public virtual int GetFrontIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(frontName); }
    public virtual int GetBackIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(backName); }
    public virtual int GetLeftIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(leftName); }
    public virtual int GetRightIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(rightName); }

    public virtual void Init() { }

    public string[] UsedTextures;

    public virtual SoundMaterial soundMaterial { get; }

    public virtual bool isTransparent { get { return false; } }

    public virtual bool isCollidable { get { return true; } }

    public virtual string GetBreakEffectTexture(byte data) { return string.Empty; }

    public virtual string GetBreakEffectTexture(NBTChunk chunk, byte data) { return GetBreakEffectTexture(data); }

    protected Color topColor;
    protected Color bottomColor;
    protected Color frontColor;
    protected Color backColor;
    protected Color leftColor;
    protected Color rightColor;

    public virtual Color GetTopTintColorByData(NBTChunk chunk, byte data) { return Color.white; }
    public virtual Color GetBottomTintColorByData(NBTChunk chunk, byte data) { return Color.white; }
    public virtual Color GetFrontTintColorByData(NBTChunk chunk, byte data) { return Color.white; }
    public virtual Color GetBackTintColorByData(NBTChunk chunk, byte data) { return Color.white; }
    public virtual Color GetLeftTintColorByData(NBTChunk chunk, byte data) { return Color.white; }
    public virtual Color GetRightTintColorByData(NBTChunk chunk, byte data) { return Color.white; }


    public virtual bool hasDropItem { get { return true; } }

    public virtual string GetDropItemByData(byte data) { return id; }

    public virtual byte GetDropItemData(byte data) { return data; }

    protected static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    protected static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    protected static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    protected static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    protected static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    protected static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    protected static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    protected static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    public override Mesh GetItemMesh(NBTChunk chunk, byte blockData)
    {
        if (!itemMeshDict.ContainsKey(blockData))
        {
            Vector3Int pos = Vector3Int.zero;

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

            NBTMesh nbtMesh = new NBTMesh(256);

            AddFrontFace(nbtMesh, pos, blockData);
            AddRightFace(nbtMesh, pos, blockData);
            AddLeftFace(nbtMesh, pos, blockData);
            AddBackFace(nbtMesh, pos, blockData);
            AddTopFace(nbtMesh, pos, blockData);
            AddBottomFace(nbtMesh, pos, blockData);

            nbtMesh.Refresh();

            itemMeshDict.Add(blockData, nbtMesh.mesh);

            nbtMesh.Dispose();
        }

        return itemMeshDict[blockData];
    }

    protected virtual Rotation GetTopRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBottomRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetFrontRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBackRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetLeftRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetRightRotationByData(byte data) { return Rotation.Zero; }

    public virtual void AddCube(NBTChunk chunk, byte blockData, byte skyLight, Vector3Int pos, NBTGameObject nbtGO)
    {
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

        UnityEngine.Profiling.Profiler.BeginSample("AddFaces");

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
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(nbtGO.nbtMesh, pos, blockData);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(nbtGO.nbtMesh, pos, blockData);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    protected Vector4 ToVector4(Vector3 v3, float w)
    {
        return new Vector4(v3.x, v3.y, v3.z, w);
    }

    protected void AddFace(NBTMesh mesh, Vector3Int pos, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, int faceIndex)
    {
        AddFace(mesh, pos, pos1, pos2, pos3, pos4, faceIndex, Color.white);
    }

    protected Rotation rotation = Rotation.Zero;
    Vector2[] uv_zero = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    Vector2[] uv_right = new Vector2[4] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };
    Vector2[] uv {
        get {
            if (rotation == Rotation.Zero) {
                return uv_zero;
            } else if (rotation == Rotation.Right) {
                return uv_right;
            }
            return uv_zero;
        }
    }

    void SetVertex(NBTMesh mesh, Vector4 pos, Vector2 texcoord, Vector4 color)
    {
        Vertex vert = mesh.vertexArray[mesh.vertexCount];
        vert.pos = pos;
        vert.texcoord = texcoord;
        vert.color = color;
        mesh.vertexArray[mesh.vertexCount++] = vert;
    }

    protected void AddFace(NBTMesh mesh, Vector3Int pos, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, int faceIndex, Color color)
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
    
    protected virtual void AddFrontFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetFrontRotationByData(blockData);
        AddFace(mesh, pos, nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight, frontIndex, frontColor);
    }

    protected virtual void AddBackFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetBackRotationByData(blockData);
        AddFace(mesh, pos, farBottomRight, farTopRight, farTopLeft, farBottomLeft, backIndex, backColor);
    }

    protected virtual void AddTopFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetTopRotationByData(blockData);
        AddFace(mesh, pos, farTopRight, nearTopRight, nearTopLeft, farTopLeft, topIndex, topColor);
    }

    protected virtual void AddBottomFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetBottomRotationByData(blockData);
        AddFace(mesh, pos, nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft, bottomIndex, bottomColor);
    }

    protected virtual void AddLeftFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetLeftRotationByData(blockData);
        AddFace(mesh, pos, farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft, leftIndex, leftColor);
    }

    protected virtual void AddRightFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        rotation = GetRightRotationByData(blockData);
        AddFace(mesh, pos, nearBottomRight, nearTopRight, farTopRight, farBottomRight, rightIndex, rightColor);
    }
}
