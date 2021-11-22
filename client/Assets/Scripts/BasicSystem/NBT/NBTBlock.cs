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

public enum BlockMaterial
{
    Default,
    Ground,
    Wood,
    Leaves,
    Web,
    RockI,
    RockII,
    RockIII,
    RockIV,
    Glass,
}

public struct BlockLightAttributes
{
    public byte exist;
    public byte skyLight;
    public byte blockLight;
}

public struct CubeAttributes
{
    public Vector3Int pos;
    public Vector3Int worldPos;
    public byte blockData;

    public bool isBreakingMesh;

    public BlockLightAttributes front;
    public BlockLightAttributes back;
    public BlockLightAttributes left;
    public BlockLightAttributes right;
    public BlockLightAttributes top;
    public BlockLightAttributes bottom;

    public BlockLightAttributes frontTop ;
    public BlockLightAttributes frontBottom ;
    public BlockLightAttributes frontLeft ;
    public BlockLightAttributes frontRight ;
    public BlockLightAttributes frontTopLeft ;
    public BlockLightAttributes frontTopRight ;
    public BlockLightAttributes frontBottomLeft ;
    public BlockLightAttributes frontBottomRight ;

    public BlockLightAttributes backTop ;
    public BlockLightAttributes backBottom ;
    public BlockLightAttributes backLeft ;
    public BlockLightAttributes backRight ;
    public BlockLightAttributes backTopLeft ;
    public BlockLightAttributes backTopRight ;
    public BlockLightAttributes backBottomLeft ;
    public BlockLightAttributes backBottomRight ;

    public BlockLightAttributes topLeft ;
    public BlockLightAttributes topRight ;
    public BlockLightAttributes bottomLeft ;
    public BlockLightAttributes bottomRight ;
}

public struct FaceAttributes
{
    public Vector3[] pos;
    public Vector2[] uv;
    public float[] ao;
    public int faceIndex;
    public Color color;
    public float[] skyLight;
    public float[] blockLight;
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

    public virtual bool isTileEntity { get { return false; } } // tile entity block has its own material

    public virtual void Init() { }

    public string[] UsedTextures;

    public virtual BlockMaterial blockMaterial { get { return BlockMaterial.Default; } }

    public virtual SoundMaterial soundMaterial { get; }

    public virtual bool isTransparent { get { return false; } }

    public virtual bool willReduceLight { get { return false; } }

    // right click interact
    public virtual bool canInteract { get { return false; } }
    public virtual void OnRightClick(Vector3Int pos) { }

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

    public virtual Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetTopTintColorByData(data); }
    public virtual Color GetBottomTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetBottomTintColorByData(data); }
    public virtual Color GetFrontTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetFrontTintColorByData(data); }
    public virtual Color GetBackTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetBackTintColorByData(data); }
    public virtual Color GetLeftTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetLeftTintColorByData(data); }
    public virtual Color GetRightTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return GetRightTintColorByData(data); }
    public virtual Color GetTopTintColorByData(byte data) { return Color.white; }
    public virtual Color GetBottomTintColorByData(byte data) { return Color.white; }
    public virtual Color GetFrontTintColorByData(byte data) { return Color.white; }
    public virtual Color GetBackTintColorByData(byte data) { return Color.white; }
    public virtual Color GetLeftTintColorByData(byte data) { return Color.white; }
    public virtual Color GetRightTintColorByData(byte data) { return Color.white; }

    protected virtual Rotation GetTopRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBottomRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetFrontRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetBackRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetLeftRotationByData(byte data) { return Rotation.Zero; }
    protected virtual Rotation GetRightRotationByData(byte data) { return Rotation.Zero; }

    public virtual bool hasDropItem { get { return true; } }
    public override Vector3 itemSize => itemSize_half;

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

    public virtual void OnDestroyBlock(Vector3Int globalPos, byte blockData)
    {

    }

    protected static float[] skylight_default = new float[] { 1, 1, 1, 1 };
    protected static float[] blocklight_default = new float[] { 1, 1, 1, 1 };
    public virtual Mesh GetItemMesh(byte data = 0)
    {
        CubeAttributes ca = new CubeAttributes();

        NBTMesh nbtMesh = new NBTMesh(256);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = skylight_default;
        fa.blockLight = blocklight_default;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            fa.faceIndex = GetFrontIndexByData(null, data);
            fa.uv = GetFrontRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetFrontTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.faceIndex = GetBackIndexByData(null, data);
            fa.uv = GetBackRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetBackTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.faceIndex = GetTopIndexByData(null, data);
            fa.uv = GetTopRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetTopTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.faceIndex = GetBottomIndexByData(null, data);
            fa.uv = GetBottomRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetBottomTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.faceIndex = GetLeftIndexByData(null, data);
            fa.uv = GetLeftRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetLeftTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.faceIndex = GetRightIndexByData(null, data);
            fa.uv = GetRightRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetRightTintColorByData(data);
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

        chunk.GetLights(pos.x - chunk.x * 16, pos.y, pos.z - chunk.z * 16, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };

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

    static byte HAS_BLOCK = 1;
    static byte NO_BLOCK = 0;

    BlockLightAttributes InitBlockLightAttributes(NBTChunk chunk, int xInChunk, int worldY, int zInChunk)
    {
        BlockLightAttributes bla = new BlockLightAttributes();
        bla.exist = chunk.HasOpaqueBlock(xInChunk, worldY, zInChunk) ? HAS_BLOCK : NO_BLOCK;
        if (bla.exist == 0)
        {
            chunk.GetLightsByte(xInChunk, worldY, zInChunk, out bla.skyLight, out bla.blockLight, true);
        }
        else
        {
            bla.skyLight = 0;
            bla.blockLight = 0;
        }
        return bla;
    }

    protected void InitBlockAttributes(NBTChunk chunk, ref CubeAttributes ca)
    {
        ca.front = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y, ca.pos.z - 1);
        ca.front = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y, ca.pos.z - 1);
        ca.back = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y, ca.pos.z + 1);
        ca.left = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y, ca.pos.z);
        ca.right = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y, ca.pos.z);
        ca.top = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y + 1, ca.pos.z);
        ca.bottom = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y - 1, ca.pos.z);

        if (ca.front.exist == 0 || ca.back.exist == 0 || ca.left.exist == 0 || ca.right.exist == 0 || ca.top.exist == 0 || ca.bottom.exist == 0)
        {
            ca.frontTop = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y + 1, ca.pos.z - 1);
            ca.frontBottom = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y - 1, ca.pos.z - 1);
            ca.frontLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y, ca.pos.z - 1);
            ca.frontRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y, ca.pos.z - 1);
            ca.frontTopLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y + 1, ca.pos.z - 1);
            ca.frontTopRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y + 1, ca.pos.z - 1);
            ca.frontBottomLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y - 1, ca.pos.z - 1);
            ca.frontBottomRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y - 1, ca.pos.z - 1);

            ca.backTop = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y + 1, ca.pos.z + 1);
            ca.backBottom = InitBlockLightAttributes(chunk, ca.pos.x, ca.pos.y - 1, ca.pos.z + 1);
            ca.backLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y, ca.pos.z + 1);
            ca.backRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y, ca.pos.z + 1);
            ca.backTopLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y + 1, ca.pos.z + 1);
            ca.backTopRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y + 1, ca.pos.z + 1);
            ca.backBottomLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y - 1, ca.pos.z + 1);
            ca.backBottomRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y - 1, ca.pos.z + 1);

            ca.topLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y + 1, ca.pos.z);
            ca.topRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y + 1, ca.pos.z);

            ca.bottomLeft = InitBlockLightAttributes(chunk, ca.pos.x - 1, ca.pos.y - 1, ca.pos.z);
            ca.bottomRight = InitBlockLightAttributes(chunk, ca.pos.x + 1, ca.pos.y - 1, ca.pos.z);
        }
    }

    public virtual void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.worldPos = new Vector3Int(pos.x + chunk.x * 16, pos.y, pos.z + chunk.z * 16);
        ca.blockData = blockData;

        InitBlockAttributes(chunk, ref ca);

        UnityEngine.Profiling.Profiler.BeginSample("AddFaces");

        if (ca.front.exist == 0)
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.right.exist == 0)
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.left.exist == 0)
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.back.exist == 0)
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.top.exist == 0)
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.bottom.exist == 0)
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    protected void SetVertex(NBTMesh mesh, Vector3 pos, int faceIndex, Vector2 texcoord,
        float skyLight, float blockLight, Vector4 color, Vector3 normal)
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

        if (fa.skyLight == null)
        {
            fa.skyLight = skylight_default;
        }
        if (fa.blockLight == null)
        {
            fa.blockLight = blocklight_default;
        }

        SetVertex(mesh, fa.pos[0] + ca.pos, fa.faceIndex, fa.uv[0], fa.skyLight[0], fa.blockLight[0], fa.color, fa.normal);
        SetVertex(mesh, fa.pos[1] + ca.pos, fa.faceIndex, fa.uv[1], fa.skyLight[1], fa.blockLight[1], fa.color, fa.normal);
        SetVertex(mesh, fa.pos[2] + ca.pos, fa.faceIndex, fa.uv[2], fa.skyLight[2], fa.blockLight[2], fa.color, fa.normal);
        SetVertex(mesh, fa.pos[3] + ca.pos, fa.faceIndex, fa.uv[3], fa.skyLight[3], fa.blockLight[3], fa.color, fa.normal);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    protected virtual FaceAttributes GetFrontFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        FaceAttributes fa = new FaceAttributes();
        fa.pos = frontVertices;
        fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
        fa.color = GetFrontTintColorByData(chunk, ca.pos, ca.blockData);
        fa.normal = Vector3.forward;

        //nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight
        fa.skyLight = new float[4];
        fa.skyLight[0] = (ca.front.skyLight + ca.frontBottom.skyLight + ca.frontLeft.skyLight + ca.frontBottomLeft.skyLight) / 60.0f;
        fa.skyLight[1] = (ca.front.skyLight + ca.frontTop.skyLight + ca.frontLeft.skyLight + ca.frontTopLeft.skyLight) / 60.0f;
        fa.skyLight[2] = (ca.front.skyLight + ca.frontTop.skyLight + ca.frontRight.skyLight + ca.frontTopRight.skyLight) / 60.0f;
        fa.skyLight[3] = (ca.front.skyLight + ca.frontBottom.skyLight + ca.frontRight.skyLight + ca.frontBottomRight.skyLight) / 60.0f;
        fa.blockLight = new float[4];
        fa.blockLight[0] = (ca.front.blockLight + ca.frontBottom.blockLight + ca.frontLeft.blockLight + ca.frontBottomLeft.blockLight) / 60.0f;
        fa.blockLight[1] = (ca.front.blockLight + ca.frontTop.blockLight + ca.frontLeft.blockLight + ca.frontTopLeft.blockLight) / 60.0f;
        fa.blockLight[2] = (ca.front.blockLight + ca.frontTop.blockLight + ca.frontRight.blockLight + ca.frontTopRight.blockLight) / 60.0f;
        fa.blockLight[3] = (ca.front.blockLight + ca.frontBottom.blockLight + ca.frontRight.blockLight + ca.frontBottomRight.blockLight) / 60.0f;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetBackFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        FaceAttributes fa = new FaceAttributes();
        fa.pos = backVertices;
        fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
        fa.color = GetBackTintColorByData(chunk, ca.pos, ca.blockData);
        fa.normal = Vector3.back;

        //farBottomRight, farTopRight, farTopLeft, farBottomLeft
        fa.skyLight = new float[4];
        fa.skyLight[0] = (ca.back.skyLight + ca.backBottom.skyLight + ca.backRight.skyLight + ca.backBottomRight.skyLight) / 60.0f;
        fa.skyLight[1] = (ca.back.skyLight + ca.backTop.skyLight + ca.backRight.skyLight + ca.backTopRight.skyLight) / 60.0f;
        fa.skyLight[2] = (ca.back.skyLight + ca.backTop.skyLight + ca.backLeft.skyLight + ca.backTopLeft.skyLight) / 60.0f;
        fa.skyLight[3] = (ca.back.skyLight + ca.backBottom.skyLight + ca.backLeft.skyLight + ca.backBottomLeft.skyLight) / 60.0f;
        fa.blockLight = new float[4];
        fa.blockLight[0] = (ca.back.blockLight + ca.backBottom.blockLight + ca.backRight.blockLight + ca.backBottomRight.blockLight) / 60.0f;
        fa.blockLight[1] = (ca.back.blockLight + ca.backTop.blockLight + ca.backRight.blockLight + ca.backTopRight.blockLight) / 60.0f;
        fa.blockLight[2] = (ca.back.blockLight + ca.backTop.blockLight + ca.backLeft.blockLight + ca.backTopLeft.blockLight) / 60.0f;
        fa.blockLight[3] = (ca.back.blockLight + ca.backBottom.blockLight + ca.backLeft.blockLight + ca.backBottomLeft.blockLight) / 60.0f;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetTopFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        FaceAttributes fa = new FaceAttributes();
        fa.pos = topVertices;
        fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
        fa.color = GetTopTintColorByData(chunk, ca.pos, ca.blockData);
        fa.normal = Vector3.up;

        //farTopRight, nearTopRight, nearTopLeft, farTopLeft
        fa.skyLight = new float[4];
        fa.skyLight[0] = (ca.top.skyLight + ca.backTop.skyLight + ca.topRight.skyLight + ca.backTopRight.skyLight) / 60.0f;
        fa.skyLight[1] = (ca.top.skyLight + ca.frontTop.skyLight + ca.topRight.skyLight + ca.frontTopRight.skyLight) / 60.0f;
        fa.skyLight[2] = (ca.top.skyLight + ca.frontTop.skyLight + ca.topLeft.skyLight + ca.frontTopLeft.skyLight) / 60.0f;
        fa.skyLight[3] = (ca.top.skyLight + ca.backTop.skyLight + ca.topLeft.skyLight + ca.backTopLeft.skyLight) / 60.0f;
        fa.blockLight = new float[4];
        fa.blockLight[0] = (ca.top.blockLight + ca.backTop.blockLight + ca.topRight.blockLight + ca.backTopRight.blockLight) / 60.0f;
        fa.blockLight[1] = (ca.top.blockLight + ca.frontTop.blockLight + ca.topRight.blockLight + ca.frontTopRight.blockLight) / 60.0f;
        fa.blockLight[2] = (ca.top.blockLight + ca.frontTop.blockLight + ca.topLeft.blockLight + ca.frontTopLeft.blockLight) / 60.0f;
        fa.blockLight[3] = (ca.top.blockLight + ca.backTop.blockLight + ca.topLeft.blockLight + ca.backTopLeft.blockLight) / 60.0f;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetBottomFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        FaceAttributes fa = new FaceAttributes();
        fa.pos = bottomVertices;
        fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
        fa.color = GetBottomTintColorByData(chunk, ca.pos, ca.blockData);
        fa.normal = Vector3.down;

        //nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft
        fa.skyLight = new float[4];
        fa.skyLight[0] = (ca.bottom.skyLight + ca.frontBottom.skyLight + ca.bottomRight.skyLight + ca.frontBottomRight.skyLight) / 60.0f;
        fa.skyLight[1] = (ca.bottom.skyLight + ca.backBottom.skyLight + ca.bottomRight.skyLight + ca.backBottomRight.skyLight) / 60.0f;
        fa.skyLight[2] = (ca.bottom.skyLight + ca.backBottom.skyLight + ca.bottomLeft.skyLight + ca.backBottomLeft.skyLight) / 60.0f;
        fa.skyLight[3] = (ca.bottom.skyLight + ca.frontBottom.skyLight + ca.bottomLeft.skyLight + ca.frontBottomLeft.skyLight) / 60.0f;
        fa.blockLight = new float[4];
        fa.blockLight[0] = (ca.bottom.blockLight + ca.frontBottom.blockLight + ca.bottomRight.blockLight + ca.frontBottomRight.blockLight) / 60.0f;
        fa.blockLight[1] = (ca.bottom.blockLight + ca.backBottom.blockLight + ca.bottomRight.blockLight + ca.backBottomRight.blockLight) / 60.0f;
        fa.blockLight[2] = (ca.bottom.blockLight + ca.backBottom.blockLight + ca.bottomLeft.blockLight + ca.backBottomLeft.blockLight) / 60.0f;
        fa.blockLight[3] = (ca.bottom.blockLight + ca.frontBottom.blockLight + ca.bottomLeft.blockLight + ca.frontBottomLeft.blockLight) / 60.0f;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetLeftFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        FaceAttributes fa = new FaceAttributes();
        fa.pos = leftVertices;
        fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
        fa.color = GetLeftTintColorByData(chunk, ca.pos, ca.blockData);
        fa.normal = Vector3.left;

        //farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft
        fa.skyLight = new float[4];
        fa.skyLight[0] = (ca.left.skyLight + ca.backLeft.skyLight + ca.bottomLeft.skyLight + ca.backBottomLeft.skyLight) / 60.0f;
        fa.skyLight[1] = (ca.left.skyLight + ca.backLeft.skyLight + ca.topLeft.skyLight + ca.backTopLeft.skyLight) / 60.0f;
        fa.skyLight[2] = (ca.left.skyLight + ca.frontLeft.skyLight + ca.topLeft.skyLight + ca.frontTopLeft.skyLight) / 60.0f;
        fa.skyLight[3] = (ca.left.skyLight + ca.frontLeft.skyLight + ca.bottomLeft.skyLight + ca.frontBottomLeft.skyLight) / 60.0f;
        fa.blockLight = new float[4];
        fa.blockLight[0] = (ca.left.blockLight + ca.backLeft.blockLight + ca.bottomLeft.blockLight + ca.backBottomLeft.blockLight) / 60.0f;
        fa.blockLight[1] = (ca.left.blockLight + ca.backLeft.blockLight + ca.topLeft.blockLight + ca.backTopLeft.blockLight) / 60.0f;
        fa.blockLight[2] = (ca.left.blockLight + ca.frontLeft.blockLight + ca.topLeft.blockLight + ca.frontTopLeft.blockLight) / 60.0f;
        fa.blockLight[3] = (ca.left.blockLight + ca.frontLeft.blockLight + ca.bottomLeft.blockLight + ca.frontBottomLeft.blockLight) / 60.0f;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }
    protected virtual FaceAttributes GetRightFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        FaceAttributes fa = new FaceAttributes();
        fa.pos = rightVertices;
        fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
        fa.color = GetRightTintColorByData(chunk, ca.pos, ca.blockData);
        fa.normal = Vector3.right;

        //nearBottomRight, nearTopRight, farTopRight, farBottomRight
        fa.skyLight = new float[4];
        fa.skyLight[0] = (ca.right.skyLight + ca.frontRight.skyLight + ca.bottomRight.skyLight + ca.frontBottomRight.skyLight) / 60.0f;
        fa.skyLight[1] = (ca.right.skyLight + ca.frontRight.skyLight + ca.topRight.skyLight + ca.frontTopRight.skyLight) / 60.0f;
        fa.skyLight[2] = (ca.right.skyLight + ca.backRight.skyLight + ca.topRight.skyLight + ca.backTopRight.skyLight) / 60.0f;
        fa.skyLight[3] = (ca.right.skyLight + ca.backRight.skyLight + ca.bottomRight.skyLight + ca.backBottomRight.skyLight) / 60.0f;
        fa.blockLight = new float[4];
        fa.blockLight[0] = (ca.right.blockLight + ca.frontRight.blockLight + ca.bottomRight.blockLight + ca.frontBottomRight.blockLight) / 60.0f;
        fa.blockLight[1] = (ca.right.blockLight + ca.frontRight.blockLight + ca.topRight.blockLight + ca.frontTopRight.blockLight) / 60.0f;
        fa.blockLight[2] = (ca.right.blockLight + ca.backRight.blockLight + ca.topRight.blockLight + ca.backTopRight.blockLight) / 60.0f;
        fa.blockLight[3] = (ca.right.blockLight + ca.backRight.blockLight + ca.bottomRight.blockLight + ca.backBottomRight.blockLight) / 60.0f;

        Rotation rotation = GetFrontRotationByData(ca.blockData);
        if (rotation == Rotation.Right)
            fa.uv = uv_right;
        else
            fa.uv = uv_zero;

        return fa;
    }

    public void OnAddBlock(RaycastHit hit)
    {
        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        if (CanAddBlock(pos))
        {
            PlayerController.instance.PlayHandAnimation();

            byte type = NBTGeneratorManager.id2type[id];
            byte data = (byte)InventorySystem.items[ItemSelectPanel.curIndex].damage;
            NBTHelper.SetBlockData(pos, type, data);

            InventorySystem.DecrementCurrent();
            ItemSelectPanel.instance.RefreshUI();
        }
    }

    public bool CanAddBlock(Vector3Int pos)
    {
        byte type = NBTHelper.GetBlockByte(pos);
        NBTBlock targetGenerator = NBTGeneratorManager.GetMeshGenerator(type);

        if (this is NBTPlant)
        {
            if (this == targetGenerator) { return false; }

            byte belowType = NBTHelper.GetBlockByte(pos + Vector3Int.down);

            //如果手上拿的是植物，则判断下方是否是否是实体
            NBTBlock targetBelowGenerator = NBTGeneratorManager.GetMeshGenerator(belowType);
            return targetBelowGenerator != null && !(targetBelowGenerator is NBTPlant);
        }
        else
        {
            //如果手上拿的不是植物，则判断碰撞盒是否与玩家相交
            return !PlayerController.instance.cc.bounds.Intersects(new Bounds(pos, Vector3.one));
        }
    }

    public virtual GameObject GetTileEntityGameObject(NBTChunk chunk, byte blockData, Vector3Int pos)
    {
        return null;
    }

    protected void RenderWireframeByVertex(float top, float bottom, float left, float right, float front, float back)
    {
        // bottom lines
        GL.Vertex3(left, bottom, front);
        GL.Vertex3(right, bottom, front);
        GL.Vertex3(left, bottom, back);
        GL.Vertex3(right, bottom, back);
        GL.Vertex3(left, bottom, front);
        GL.Vertex3(left, bottom, back);
        GL.Vertex3(right, bottom, front);
        GL.Vertex3(right, bottom, back);

        // top lines
        GL.Vertex3(left, top, front);
        GL.Vertex3(right, top, front);
        GL.Vertex3(left, top, back);
        GL.Vertex3(right, top, back);
        GL.Vertex3(left, top, front);
        GL.Vertex3(left, top, back);
        GL.Vertex3(right, top, front);
        GL.Vertex3(right, top, back);

        // vertical lines
        GL.Vertex3(right, top, back);
        GL.Vertex3(right, bottom, back);
        GL.Vertex3(right, top, front);
        GL.Vertex3(right, bottom, front);
        GL.Vertex3(left, top, back);
        GL.Vertex3(left, bottom, back);
        GL.Vertex3(left, top, front);
        GL.Vertex3(left, bottom, front);
    }

    public virtual void RenderWireframe(byte blockData)
    {
        float top = 0.501f;
        float bottom = -0.501f;
        float left = -0.501f;
        float right = 0.501f;
        float front = 0.501f;
        float back = -0.501f;

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}
