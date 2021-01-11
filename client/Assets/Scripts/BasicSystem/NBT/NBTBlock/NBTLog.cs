using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLog : NBTBlock
{
    public override string name { get { return "Log"; } }

    public override string topName { get { return "log_oak_top"; } }
    public override string bottomName { get { return "log_oak_top"; } }
    public override string frontName { get { return "log_oak"; } }
    public override string backName { get { return "log_oak"; } }
    public override string leftName { get { return "log_oak"; } }
    public override string rightName { get { return "log_oak"; } }

    protected override Rotation GetTopRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetBottomRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetFrontRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetBackRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetLeftRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Zero;
        }
        else
        {
            return Rotation.Right;
        }
    }
    protected override Rotation GetRightRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Zero;
        }
        else
        {
            return Rotation.Right;
        }
    }

    public override void Init()
    {
        UsedTextures = new string[] { "log_oak_top", "log_oak", "log_spruce_top", "log_spruce", "log_birch_top", "log_birch", "log_jungle_top", "log_jungle" };
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 1:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 2:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 3:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
            case 4:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 5:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 6:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 7:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
        }
        return TextureArrayManager.GetIndexByName("log_oak_top");
    }

    public override int GetBottomIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 1:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 2:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 3:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
            case 4:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 5:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 6:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 7:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
        }
        return TextureArrayManager.GetIndexByName("log_oak_top");
    }
    public override int GetFrontIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }

    public override int GetBackIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }
    public override int GetLeftIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }

    public override int GetRightIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
    {
        string texture = "";
        switch (data % 4)
        {
            case 0:
                texture = "log_oak";
                break;
            case 1:
                texture = "log_spruce";
                break;
            case 2:
                texture = "log_birch";
                break;
            case 3:
                texture = "log_jungle";
                break;
        }
        return texture;
    }

    List<int> triangles_oak_top = new List<int>();
    List<int> triangles_oak_side = new List<int>();
    List<int> triangles_spruce_top = new List<int>();
    List<int> triangles_spruce_side = new List<int>();
    List<int> triangles_birch_top = new List<int>();
    List<int> triangles_birch_side = new List<int>();
    List<int> triangles_jungle_top = new List<int>();
    List<int> triangles_jungle_side = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        Mesh top = null;
        Mesh side = null;

        List<int> triangles_top = null;
        List<int> triangles_side = null;

        switch (blockData % 4)
        {
            case 0:
                triangles_top = triangles_oak_top;
                triangles_side = triangles_oak_side;
                break;
            case 1:
                triangles_top = triangles_spruce_top;
                triangles_side = triangles_spruce_side;
                break;
            case 2:
                triangles_top = triangles_birch_top;
                triangles_side = triangles_birch_side;
                break;
            case 3:
                triangles_top = triangles_jungle_top;
                triangles_side = triangles_jungle_side;
                break;
        }

        if (blockData >= 0 && blockData < 4)
        {
            top = Resources.Load<Mesh>("Meshes/blocks/log/log_y_top");
            side = Resources.Load<Mesh>("Meshes/blocks/log/log_y_side");
        }
        if (blockData >= 4 && blockData < 8)
        {
            top = Resources.Load<Mesh>("Meshes/blocks/log/log_x_top");
            side = Resources.Load<Mesh>("Meshes/blocks/log/log_x_side");
        }
        else if (blockData >= 8 && blockData < 12)
        {
            top = Resources.Load<Mesh>("Meshes/blocks/log/log_z_top");
            side = Resources.Load<Mesh>("Meshes/blocks/log/log_z_side");
        }

        CopyFromMesh(top, pos, vertices, uv, triangles_top);
        CopyFromMesh(side, pos, vertices, uv, triangles_side);
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_oak_top.Count > 0)
        {
            trianglesList.Add(triangles_oak_top);
            materialList.Add(Resources.Load<Material>("Materials/block/log_oak_top"));
        }
        if (triangles_oak_side.Count > 0)
        {
            trianglesList.Add(triangles_oak_side);
            materialList.Add(Resources.Load<Material>("Materials/block/log_oak_side"));
        }
        if (triangles_spruce_top.Count > 0)
        {
            trianglesList.Add(triangles_spruce_top);
            materialList.Add(Resources.Load<Material>("Materials/block/log_spruce_top"));
        }
        if (triangles_spruce_side.Count > 0)
        {
            trianglesList.Add(triangles_spruce_side);
            materialList.Add(Resources.Load<Material>("Materials/block/log_spruce_side"));
        }
        if (triangles_birch_top.Count > 0)
        {
            trianglesList.Add(triangles_birch_top);
            materialList.Add(Resources.Load<Material>("Materials/block/log_birch_top"));
        }
        if (triangles_birch_side.Count > 0)
        {
            trianglesList.Add(triangles_birch_side);
            materialList.Add(Resources.Load<Material>("Materials/block/log_birch_side"));
        }
        if (triangles_jungle_top.Count > 0)
        {
            trianglesList.Add(triangles_jungle_top);
            materialList.Add(Resources.Load<Material>("Materials/block/log_jungle_top"));
        }
        if (triangles_jungle_side.Count > 0)
        {
            trianglesList.Add(triangles_jungle_side);
            materialList.Add(Resources.Load<Material>("Materials/block/log_jungle_side"));
        }
    }

    public override void ClearData()
    {
        triangles_oak_top.Clear();
        triangles_oak_side.Clear();
        triangles_spruce_top.Clear();
        triangles_spruce_side.Clear();
        triangles_birch_top.Clear();
        triangles_birch_side.Clear();
        triangles_jungle_top.Clear();
        triangles_jungle_side.Clear();
    }
}
