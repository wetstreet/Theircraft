using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGravel : NBTBlock
{
    public override string topName { get { return "gravel"; } }
    public override string bottomName { get { return "gravel"; } }
    public override string frontName { get { return "gravel"; } }
    public override string backName { get { return "gravel"; } }
    public override string leftName { get { return "gravel"; } }
    public override string rightName { get { return "gravel"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Gravel; } }

    public override string GetBreakEffectTexture(byte data) { return "gravel"; }

    List<int> triangles = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
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
        if (triangles.Count > 0)
        {
            trianglesList.Add(triangles);
            materialList.Add(Resources.Load<Material>("Materials/block/gravel"));
        }
    }

    public override void ClearData()
    {
        triangles.Clear();
    }
}
