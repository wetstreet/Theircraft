using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGrassBlock : NBTBlock
{
    public override string topName { get { return "grass_top"; } }
    public override string bottomName { get { return "dirt"; } }
    public override string frontName { get { return "grass_side"; } }
    public override string backName { get { return "grass_side"; } }
    public override string leftName { get { return "grass_side"; } }
    public override string rightName { get { return "grass_side"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }

    List<int> triangles_top = new List<int>();
    List<int> triangles_side = new List<int>();
    List<int> triangles_bot = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(nbtGO.vertices, nbtGO.uv1, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(nbtGO.vertices, nbtGO.uv1, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(nbtGO.vertices, nbtGO.uv1, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(nbtGO.vertices, nbtGO.uv1, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(nbtGO.vertices, nbtGO.uv1, triangles_top, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(nbtGO.vertices, nbtGO.uv1, triangles_bot, pos);
        }
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_top.Count > 0)
        {
            trianglesList.Add(triangles_top);
            materialList.Add(Resources.Load<Material>("Materials/block/grass_top"));
        }
        if (triangles_side.Count > 0)
        {
            trianglesList.Add(triangles_side);
            materialList.Add(Resources.Load<Material>("Materials/block/grass_side"));
        }
        if (triangles_bot.Count > 0)
        {
            trianglesList.Add(triangles_bot);
            materialList.Add(Resources.Load<Material>("Materials/block/grass_bottom"));
        }
    }

    public override void ClearData()
    {
        triangles_top.Clear();
        triangles_side.Clear();
        triangles_bot.Clear();
    }
}
