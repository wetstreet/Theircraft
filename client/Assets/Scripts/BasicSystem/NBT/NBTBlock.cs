﻿using UnityEngine;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct Vertex
{
    public Vector4 pos;
    public Vector3 normal;
    public Vector4 color;
    public Vector4 texcoord;
}

public enum Rotation
{
    Zero,
    Right,
}

public struct CubeAttributes
{
    public Vector3Int pos;
    public byte blockData;
}

public struct FaceAttributes
{
    public Vector3[] pos;
    public Vector2[] uv;
    public int faceIndex;
    public Color color;
    public float skyLight;
    public float blockLight;
    public Vector3 normal;
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

    public virtual bool isTileEntity { get { return false; } } // tile entity block has its own material

    public virtual float breakNeedTime {
        get {
            float damage = speedMultiplier / hardness / 100;
            if (damage > 1) return 0;
            int ticks = Mathf.CeilToInt(1 / damage);
            return ticks / 20f;
        }
    }

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

    public virtual bool isFence { get { return false; } }

    public virtual bool isCollidable { get { return true; } }

    public virtual string GetBreakEffectTexture(byte data) { return string.Empty; }

    public virtual string GetBreakEffectTexture(NBTChunk chunk, byte data) { return GetBreakEffectTexture(data); }

    public virtual string GetBreakEffectTexture(NBTChunk chunk, Vector3Int pos, byte data) { return GetBreakEffectTexture(chunk, data); }

    public virtual Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetBottomTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetFrontTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetBackTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetLeftTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetRightTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }

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

    protected static Vector3[] frontVertices = new Vector3[] { nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight };
    protected static Vector3[] backVertices = new Vector3[] { farBottomRight, farTopRight, farTopLeft, farBottomLeft };
    protected static Vector3[] topVertices = new Vector3[] { farTopRight, nearTopRight, nearTopLeft, farTopLeft };
    protected static Vector3[] bottomVertices = new Vector3[] { nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft };
    protected static Vector3[] leftVertices = new Vector3[] { farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft };
    protected static Vector3[] rightVertices = new Vector3[] { nearBottomRight, nearTopRight, farTopRight, farBottomRight };

    protected static Vector2[] uv_zero = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    protected static Vector2[] uv_right = new Vector2[4] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };

    // wireframe
    public virtual float topOffset { get { return 0.501f; } }
    public virtual float bottomOffset { get { return -0.501f; } }
    public virtual float leftOffset { get { return -0.501f; } }
    public virtual float rightOffset { get { return 0.501f; } }
    public virtual float frontOffset { get { return 0.501f; } }
    public virtual float backOffset { get { return -0.501f; } }
    public virtual float radius { get { return -0.501f; } }
    public virtual bool useRadius { get { return false; } }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = new CubeAttributes();
        //ca.pos = pos;
        ca.blockData = blockData;

        NBTMesh nbtMesh = new NBTMesh(256);

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
        fa.color = GetFrontTintColorByData(chunk, pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            AddFace(nbtMesh, fa, ca);

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            AddFace(nbtMesh, fa, ca);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetItemMesh error,generator="+GetType()+",message=\n" + e.Message);
        }

        nbtMesh.Refresh();

        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }

    protected virtual Rotation GetTopRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBottomRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetFrontRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBackRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetLeftRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetRightRotationByData(byte data) { return Rotation.Zero; }

    public virtual void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        UnityEngine.Profiling.Profiler.BeginSample("AddFaces");

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
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    protected void SetVertex(NBTMesh mesh, Vector3 pos, int faceIndex, Vector2 texcoord, float skyLight, float blockLight, Vector4 color, Vector3 normal)
    {
        Vertex vert = mesh.vertexArray[mesh.vertexCount];
        vert.pos.x = pos.x;
        vert.pos.y = pos.y;
        vert.pos.z = pos.z;
        vert.pos.w = faceIndex;
        vert.texcoord.x = texcoord.x;
        vert.texcoord.y = texcoord.y;
        vert.texcoord.z = skyLight;
        vert.texcoord.w = blockLight;
        vert.color = color;
        vert.normal = normal;
        mesh.vertexArray[mesh.vertexCount++] = vert;
    }

    protected void AddFace(NBTMesh mesh, FaceAttributes fa, CubeAttributes ca)
    {
        ushort startIndex = mesh.vertexCount;

        SetVertex(mesh, fa.pos[0] + ca.pos, fa.faceIndex, fa.uv[0], fa.skyLight, fa.blockLight, fa.color, fa.normal);
        SetVertex(mesh, fa.pos[1] + ca.pos, fa.faceIndex, fa.uv[1], fa.skyLight, fa.blockLight, fa.color, fa.normal);
        SetVertex(mesh, fa.pos[2] + ca.pos, fa.faceIndex, fa.uv[2], fa.skyLight, fa.blockLight, fa.color, fa.normal);
        SetVertex(mesh, fa.pos[3] + ca.pos, fa.faceIndex, fa.uv[3], fa.skyLight, fa.blockLight, fa.color, fa.normal);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    protected virtual FaceAttributes GetFrontFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z - 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = frontVertices;
        fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
        fa.color = GetFrontTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.forward;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetBackFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z + 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = backVertices;
        fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
        fa.color = GetBackTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.back;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetTopFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y + 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = topVertices;
        fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
        fa.color = GetTopTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.up;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetBottomFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y - 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa;
        fa.pos = bottomVertices;
        fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
        fa.color = GetBottomTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.down;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetLeftFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x - 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa;
        fa.pos = leftVertices;
        fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
        fa.color = GetLeftTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.left;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetRightFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x + 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa;
        fa.pos = rightVertices;
        fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
        fa.color = GetRightTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.right;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
}
