using UnityEngine;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct Vertex
{
    public Vector4 pos;
    public Vector3 normal;
    public Vector4 color;
    public Vector3 texcoord;
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

    public virtual string GetBreakEffectTexture(NBTChunk chunk, Vector3Int pos, byte data) { return GetBreakEffectTexture(chunk, data); }

    protected Color topColor = Color.white;
    protected Color bottomColor = Color.white;
    protected Color frontColor = Color.white;
    protected Color backColor = Color.white;
    protected Color leftColor = Color.white;
    protected Color rightColor = Color.white;

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

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
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

        NBTMesh nbtMesh = new NBTMesh(256);

        float skyLight = NBTHelper.GetSkyLightByte(pos.x, pos.y, pos.z) / 15f;

        AddFrontFace(nbtMesh, Vector3Int.zero, blockData, skyLight);
        AddRightFace(nbtMesh, Vector3Int.zero, blockData, skyLight);
        AddLeftFace(nbtMesh, Vector3Int.zero, blockData, skyLight);
        AddBackFace(nbtMesh, Vector3Int.zero, blockData, skyLight);
        AddTopFace(nbtMesh, Vector3Int.zero, blockData, skyLight);
        AddBottomFace(nbtMesh, Vector3Int.zero, blockData, skyLight);

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

        UnityEngine.Profiling.Profiler.BeginSample("AddFaces");

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z - 1);
            AddFrontFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x + 1, pos.y, pos.z);
            AddRightFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x - 1, pos.y, pos.z);
            AddLeftFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z + 1);
            AddBackFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y + 1, pos.z);
            AddTopFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y - 1, pos.z);
            AddBottomFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }

        UnityEngine.Profiling.Profiler.EndSample();
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

    protected void SetVertex(NBTMesh mesh, Vector3 pos, int faceIndex, Vector2 texcoord, float skyLight, Vector4 color, Vector3 normal)
    {
        Vertex vert = mesh.vertexArray[mesh.vertexCount];
        vert.pos.x = pos.x;
        vert.pos.y = pos.y;
        vert.pos.z = pos.z;
        vert.pos.w = faceIndex;
        vert.texcoord.x = texcoord.x;
        vert.texcoord.y = texcoord.y;
        vert.texcoord.z = skyLight;
        vert.color = color;
        vert.normal = normal;
        mesh.vertexArray[mesh.vertexCount++] = vert;
    }

    protected void AddFace(NBTMesh mesh, Vector3Int pos,
        Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4,
        int faceIndex, Color color, float skyLight, Vector3 normal)
    {
        ushort startIndex = mesh.vertexCount;

        SetVertex(mesh, pos1 + pos, faceIndex, uv[0], skyLight, color, normal);
        SetVertex(mesh, pos2 + pos, faceIndex, uv[1], skyLight, color, normal);
        SetVertex(mesh, pos3 + pos, faceIndex, uv[2], skyLight, color, normal);
        SetVertex(mesh, pos4 + pos, faceIndex, uv[3], skyLight, color, normal);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    protected virtual void AddFrontFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        AddFrontFace(mesh, pos, blockData, 1);
    }
    protected virtual void AddFrontFace(NBTMesh mesh, Vector3Int pos, byte blockData, float skyLight)
    {
        rotation = GetFrontRotationByData(blockData);
        AddFace(mesh, pos, nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight, frontIndex, frontColor, skyLight, Vector3.forward);
    }

    protected virtual void AddBackFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        AddBackFace(mesh, pos, blockData, 1);
    }
    protected virtual void AddBackFace(NBTMesh mesh, Vector3Int pos, byte blockData, float skyLight)
    {
        rotation = GetBackRotationByData(blockData);
        AddFace(mesh, pos, farBottomRight, farTopRight, farTopLeft, farBottomLeft, backIndex, backColor, skyLight, Vector3.back);
    }

    protected virtual void AddTopFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        AddTopFace(mesh, pos, blockData, 1);
    }
    protected virtual void AddTopFace(NBTMesh mesh, Vector3Int pos, byte blockData, float skyLight)
    {
        rotation = GetTopRotationByData(blockData);
        AddFace(mesh, pos, farTopRight, nearTopRight, nearTopLeft, farTopLeft, topIndex, topColor, skyLight, Vector3.up);
    }

    protected virtual void AddBottomFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        AddBottomFace(mesh, pos, blockData, 1);
    }
    protected virtual void AddBottomFace(NBTMesh mesh, Vector3Int pos, byte blockData, float skyLight)
    {
        rotation = GetBottomRotationByData(blockData);
        AddFace(mesh, pos, nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft, bottomIndex, bottomColor, skyLight, Vector3.down);
    }

    protected virtual void AddLeftFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        AddLeftFace(mesh, pos, blockData, 1);
    }
    protected virtual void AddLeftFace(NBTMesh mesh, Vector3Int pos, byte blockData, float skyLight)
    {
        rotation = GetLeftRotationByData(blockData);
        AddFace(mesh, pos, farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft, leftIndex, leftColor, skyLight, Vector3.left);
    }

    protected virtual void AddRightFace(NBTMesh mesh, Vector3Int pos, byte blockData)
    {
        AddRightFace(mesh, pos, blockData, 1);
    }
    protected virtual void AddRightFace(NBTMesh mesh, Vector3Int pos, byte blockData, float skyLight)
    {
        rotation = GetRightRotationByData(blockData);
        AddFace(mesh, pos, nearBottomRight, nearTopRight, farTopRight, farBottomRight, rightIndex, rightColor, skyLight, Vector3.right);
    }
}
