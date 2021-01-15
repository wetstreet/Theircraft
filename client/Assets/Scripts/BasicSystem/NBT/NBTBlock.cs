using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

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

public abstract class NBTBlock
{
    public virtual string name { get; }

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

    protected static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    protected static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    protected static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    protected static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    protected static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    protected static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    protected static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    protected static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    protected void CopyFromMesh(Mesh mesh, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        int length = vertices.Count;
        foreach (Vector3 vertex in mesh.vertices)
        {
            vertices.Add(vertex + pos);
        }
        uv.AddRange(mesh.uv);

        foreach (int index in mesh.triangles)
        {
            triangles.Add(index + length);
        }
    }
    
    protected Vector3Int pos;
    protected byte blockData;
    protected List<Vertex> vertices;
    protected List<int> triangles;

    protected virtual Rotation GetTopRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBottomRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetFrontRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBackRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetLeftRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetRightRotationByData(byte data) { return Rotation.Zero; }

    public virtual void AddCube(NBTChunk chunk, byte blockData, byte skyLight, Vector3Int pos, NBTGameObject nbtGO)
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
            AddFrontFace();
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace();
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace();
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace();
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace();
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace();
        }
    }

    Vector4 ToVector4(Vector3 v3, float w)
    {
        return new Vector4(v3.x, v3.y, v3.z, w);
    }

    protected void AddFace(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, int faceIndex)
    {
        AddFace(pos1, pos2, pos3, pos4, faceIndex, Color.white);
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

    protected void AddFace(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, int faceIndex, Color color)
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
    
    void AddFrontFace()
    {
        rotation = GetFrontRotationByData(blockData);
        AddFace(nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight, frontIndex, frontColor);
    }

    void AddBackFace()
    {
        rotation = GetBackRotationByData(blockData);
        AddFace(farBottomRight, farTopRight, farTopLeft, farBottomLeft, backIndex, backColor);
    }

    void AddTopFace()
    {
        rotation = GetTopRotationByData(blockData);
        AddFace(farTopRight, nearTopRight, nearTopLeft, farTopLeft, topIndex, topColor);
    }

    void AddBottomFace()
    {
        rotation = GetBottomRotationByData(blockData);
        AddFace(nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft, bottomIndex, bottomColor);
    }

    void AddLeftFace()
    {
        rotation = GetLeftRotationByData(blockData);
        AddFace(farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft, leftIndex, leftColor);
    }

    void AddRightFace()
    {
        rotation = GetRightRotationByData(blockData);
        AddFace(nearBottomRight, nearTopRight, farTopRight, farBottomRight, rightIndex, rightColor);
    }
}
