using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLeaves : NBTBlock
{
    public override string name { get { return "Leaves"; } }

    public override string topName { get { return "leaves_oak"; } }
    public override string bottomName { get { return "leaves_oak"; } }
    public override string frontName { get { return "leaves_oak"; } }
    public override string backName { get { return "leaves_oak"; } }
    public override string leftName { get { return "leaves_oak"; } }
    public override string rightName { get { return "leaves_oak"; } }

    protected override Color topColor => TintManager.tintColor;
    protected override Color bottomColor => TintManager.tintColor;
    protected override Color frontColor => TintManager.tintColor;
    protected override Color backColor => TintManager.tintColor;
    protected override Color leftColor => TintManager.tintColor;
    protected override Color rightColor => TintManager.tintColor;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override string GetBreakEffectTexture(byte data)
    {
        string texture = "";
        switch (data % 4)
        {
            case 0:
                texture = "leaves_oak";
                break;
            case 1:
                texture = "leaves_spruce";
                break;
            case 2:
                texture = "leaves_birch";
                break;
            case 3:
                texture = "leaves_jungle";
                break;
        }
        return texture;
    }

    List<int> triangles_oak = new List<int>();
    List<int> triangles_spruce = new List<int>();
    List<int> triangles_birch = new List<int>();
    List<int> triangles_jungle = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        List<int> triangles = null;
        Vector3Int worldPos = new Vector3Int(chunk.x * 16 + pos.x, pos.y, chunk.z * 16 + pos.z);
        switch (blockData % 4)
        {
            case 0:
                triangles = triangles_oak;
                break;
            case 1:
                triangles = triangles_spruce;
                break;
            case 2:
                triangles = triangles_birch;
                break;
            case 3:
                triangles = triangles_jungle;
                break;
        }

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(vertices, uv, triangles, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(vertices, uv, triangles, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(vertices, uv, triangles, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(vertices, uv, triangles, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(vertices, uv, triangles, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(vertices, uv, triangles, pos);
        }
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_oak.Count > 0)
        {
            trianglesList.Add(triangles_oak);
            materialList.Add(Resources.Load<Material>("Materials/block/leaves_oak"));
        }
        if (triangles_spruce.Count > 0)
        {
            trianglesList.Add(triangles_spruce);
            materialList.Add(Resources.Load<Material>("Materials/block/leaves_spruce"));
        }
        if (triangles_birch.Count > 0)
        {
            trianglesList.Add(triangles_birch);
            materialList.Add(Resources.Load<Material>("Materials/block/leaves_birch"));
        }
        if (triangles_jungle.Count > 0)
        {
            trianglesList.Add(triangles_jungle);
            materialList.Add(Resources.Load<Material>("Materials/block/leaves_jungle"));
        }
    }

    public override void ClearData()
    {
        triangles_oak.Clear();
        triangles_spruce.Clear();
        triangles_birch.Clear();
        triangles_jungle.Clear();
    }
}
