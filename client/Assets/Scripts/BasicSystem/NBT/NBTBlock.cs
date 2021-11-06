using UnityEngine;

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
    public bool front;
    public bool back;
    public bool left;
    public bool right;
    public bool top;
    public bool bottom;

    public int frontTop ;
    public int frontBottom ;
    public int frontLeft ;
    public int frontRight ;
    public int frontTopLeft ;
    public int frontTopRight ;
    public int frontBottomLeft ;
    public int frontBottomRight ;

    public int backTop ;
    public int backBottom ;
    public int backLeft ;
    public int backRight ;
    public int backTopLeft ;
    public int backTopRight ;
    public int backBottomLeft ;
    public int backBottomRight ;

    public int topLeft ;
    public int topRight ;
    public int bottomLeft ;
    public int bottomRight ;
}

public struct FaceAttributes
{
    public Vector3[] pos;
    public Vector2[] uv;
    public float[] ao;
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

    public virtual void Init() { }

    public string[] UsedTextures;

    public virtual SoundMaterial soundMaterial { get; }

    public virtual bool isTransparent { get { return false; } }

    public virtual bool willReduceLight { get { return false; } }

    public virtual bool isFence { get { return false; } }

    public virtual bool isCollidable { get { return true; } }

    public virtual string GetBreakEffectTexture(byte data) { return string.Empty; }

    public virtual string GetBreakEffectTexture(NBTChunk chunk, byte data) { return GetBreakEffectTexture(data); }

    public virtual string GetBreakEffectTexture(NBTChunk chunk, Vector3Int pos, byte data) { return GetBreakEffectTexture(chunk, data); }

    public virtual int GetTopIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(topName); }
    public virtual int GetBottomIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(bottomName); }
    public virtual int GetFrontIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(frontName); }
    public virtual int GetBackIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(backName); }
    public virtual int GetLeftIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(leftName); }
    public virtual int GetRightIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(rightName); }

    public virtual Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetBottomTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetFrontTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetBackTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetLeftTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }
    public virtual Color GetRightTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }

    protected virtual Rotation GetTopRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBottomRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetFrontRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBackRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetLeftRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetRightRotationByData(byte data) { return Rotation.Zero; }

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

    public virtual Mesh GetItemMesh(byte data = 0)
    {
        CubeAttributes ca = new CubeAttributes();

        NBTMesh nbtMesh = new NBTMesh(256);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = 1;
        fa.blockLight = 1;
        fa.color = Color.white;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            fa.faceIndex = GetFrontIndexByData(null, data);
            fa.uv = GetFrontRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.faceIndex = GetBackIndexByData(null, data);
            fa.uv = GetBackRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.faceIndex = GetTopIndexByData(null, data);
            fa.uv = GetTopRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            AddFace(nbtMesh, fa, ca);

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.faceIndex = GetBottomIndexByData(null, data);
            fa.uv = GetBottomRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.faceIndex = GetLeftIndexByData(null, data);
            fa.uv = GetLeftRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.faceIndex = GetRightIndexByData(null, data);
            fa.uv = GetRightRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
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

    // for break effect & drop item (will be affected by light)
    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = new CubeAttributes();
        //ca.pos = pos;
        ca.blockData = blockData;

        NBTMesh nbtMesh = new NBTMesh(256);

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
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
            fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
            fa.color = GetFrontTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
            fa.color = GetBackTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
            fa.color = GetTopTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
            fa.color = GetBottomTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
            fa.color = GetLeftTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
            fa.color = GetRightTintColorByData(chunk, pos, ca.blockData);
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

    protected void InitBlockAttributes(NBTChunk chunk, ref CubeAttributes ca)
    {
        ca.front = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y, ca.pos.z - 1);
        ca.back = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y, ca.pos.z + 1);
        ca.left = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y, ca.pos.z);
        ca.right = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y, ca.pos.z);
        ca.top = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y + 1, ca.pos.z);
        ca.bottom = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y - 1, ca.pos.z);

        if (ca.front || ca.back || ca.left || ca.right || ca.top || ca.bottom)
        {
            ca.frontTop = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y + 1, ca.pos.z - 1) ? 1 : 0;
            ca.frontBottom = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y - 1, ca.pos.z - 1) ? 1 : 0;
            ca.frontLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y, ca.pos.z - 1) ? 1 : 0;
            ca.frontRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y, ca.pos.z - 1) ? 1 : 0;
            ca.frontTopLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y + 1, ca.pos.z - 1) ? 1 : 0;
            ca.frontTopRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y + 1, ca.pos.z - 1) ? 1 : 0;
            ca.frontBottomLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y - 1, ca.pos.z - 1) ? 1 : 0;
            ca.frontBottomRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y - 1, ca.pos.z - 1) ? 1 : 0;

            ca.backTop = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y + 1, ca.pos.z + 1) ? 1 : 0;
            ca.backBottom = chunk.HasOpaqueBlock(ca.pos.x, ca.pos.y - 1, ca.pos.z + 1) ? 1 : 0;
            ca.backLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y, ca.pos.z + 1) ? 1 : 0;
            ca.backRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y, ca.pos.z + 1) ? 1 : 0;
            ca.backTopLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y + 1, ca.pos.z + 1) ? 1 : 0;
            ca.backTopRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y + 1, ca.pos.z + 1) ? 1 : 0;
            ca.backBottomLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y - 1, ca.pos.z + 1) ? 1 : 0;
            ca.backBottomRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y - 1, ca.pos.z + 1) ? 1 : 0;

            ca.topLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y + 1, ca.pos.z) ? 1 : 0;
            ca.topRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y + 1, ca.pos.z) ? 1 : 0;

            ca.bottomLeft = chunk.HasOpaqueBlock(ca.pos.x - 1, ca.pos.y - 1, ca.pos.z) ? 1 : 0;
            ca.bottomRight = chunk.HasOpaqueBlock(ca.pos.x + 1, ca.pos.y - 1, ca.pos.z) ? 1 : 0;
        }
    }

    public virtual void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        InitBlockAttributes(chunk, ref ca);

        UnityEngine.Profiling.Profiler.BeginSample("AddFaces");

        if (!ca.front)
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!ca.right)
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!ca.left)
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!ca.back)
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!ca.top)
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!ca.bottom)
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    protected void SetVertex(NBTMesh mesh, Vector3 pos, int faceIndex, Vector2 texcoord,
        float skyLight, float blockLight, float ao, Vector4 color, Vector3 normal)
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
        vert.color.x = color.x;
        vert.color.y = color.y;
        vert.color.z = color.z;
        vert.color.w = ao;
        vert.normal = normal;
        mesh.vertexArray[mesh.vertexCount++] = vert;
    }

    protected void AddFace(NBTMesh mesh, FaceAttributes fa, CubeAttributes ca)
    {
        ushort startIndex = mesh.vertexCount;

        if (fa.ao == null)
        {
            fa.ao = ao_default;
        }

        SetVertex(mesh, fa.pos[0] + ca.pos, fa.faceIndex, fa.uv[0], fa.skyLight, fa.blockLight, fa.ao[0], fa.color, fa.normal);
        SetVertex(mesh, fa.pos[1] + ca.pos, fa.faceIndex, fa.uv[1], fa.skyLight, fa.blockLight, fa.ao[1], fa.color, fa.normal);
        SetVertex(mesh, fa.pos[2] + ca.pos, fa.faceIndex, fa.uv[2], fa.skyLight, fa.blockLight, fa.ao[2], fa.color, fa.normal);
        SetVertex(mesh, fa.pos[3] + ca.pos, fa.faceIndex, fa.uv[3], fa.skyLight, fa.blockLight, fa.ao[3], fa.color, fa.normal);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    static float[] ao = new float[] { 1, 0.3f, 0.2f, 0.1f };
    protected static float[] ao_default = new float[] { 1, 1, 1, 1 };
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

        //nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight
        fa.ao = new float[4];
        fa.ao[0] = ao[ca.frontBottom + ca.frontLeft + ca.frontBottomLeft];
        fa.ao[1] = ao[ca.frontTop + ca.frontLeft + ca.frontTopLeft];
        fa.ao[2] = ao[ca.frontTop + ca.frontRight + ca.frontTopRight];
        fa.ao[3] = ao[ca.frontBottom + ca.frontRight + ca.frontBottomRight];

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

        //farBottomRight, farTopRight, farTopLeft, farBottomLeft
        fa.ao = new float[4];
        fa.ao[0] = ao[ca.backBottom + ca.backRight + ca.backBottomRight];
        fa.ao[1] = ao[ca.backTop + ca.backRight + ca.backTopRight];
        fa.ao[2] = ao[ca.backTop + ca.backLeft + ca.backTopLeft];
        fa.ao[3] = ao[ca.backBottom + ca.backLeft + ca.backBottomLeft];

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

        //farTopRight, nearTopRight, nearTopLeft, farTopLeft
        fa.ao = new float[4];
        fa.ao[0] = ao[ca.backTop + ca.topRight + ca.backTopRight];
        fa.ao[1] = ao[ca.frontTop + ca.topRight + ca.frontTopRight];
        fa.ao[2] = ao[ca.frontTop + ca.topLeft + ca.frontTopLeft];
        fa.ao[3] = ao[ca.backTop + ca.topLeft + ca.backTopLeft];

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

        FaceAttributes fa = new FaceAttributes();
        fa.pos = bottomVertices;
        fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
        fa.color = GetBottomTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.down;

        //nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft
        fa.ao = new float[4];
        fa.ao[0] = ao[ca.frontBottom + ca.bottomRight + ca.frontBottomRight];
        fa.ao[1] = ao[ca.backBottom + ca.bottomRight + ca.backBottomRight];
        fa.ao[2] = ao[ca.backBottom + ca.bottomLeft + ca.backBottomLeft];
        fa.ao[3] = ao[ca.frontBottom + ca.bottomLeft + ca.frontBottomLeft];

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

        FaceAttributes fa = new FaceAttributes();
        fa.pos = leftVertices;
        fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
        fa.color = GetLeftTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.left;

        //farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft
        fa.ao = new float[4];
        fa.ao[0] = ao[ca.backLeft + ca.bottomLeft + ca.backBottomLeft];
        fa.ao[1] = ao[ca.backLeft + ca.topLeft + ca.backTopLeft];
        fa.ao[2] = ao[ca.frontLeft + ca.topLeft + ca.frontTopLeft];
        fa.ao[3] = ao[ca.frontLeft + ca.bottomLeft + ca.frontBottomLeft];

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

        FaceAttributes fa = new FaceAttributes();
        fa.pos = rightVertices;
        fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
        fa.color = GetRightTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.right;

        //nearBottomRight, nearTopRight, farTopRight, farBottomRight
        fa.ao = new float[4];
        fa.ao[0] = ao[ca.frontRight + ca.bottomRight + ca.frontBottomRight];
        fa.ao[1] = ao[ca.frontRight + ca.topRight + ca.frontTopRight];
        fa.ao[2] = ao[ca.backRight + ca.topRight + ca.backTopRight];
        fa.ao[3] = ao[ca.backRight + ca.bottomRight + ca.backBottomRight];

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }

    public virtual GameObject GetTileEntityGameObject(NBTChunk chunk, byte blockData, Vector3Int pos)
    {
        return null;
    }
}
