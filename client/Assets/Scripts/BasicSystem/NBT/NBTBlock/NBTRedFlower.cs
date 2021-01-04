using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedFlower : NBTBlock
{
    public override string topName { get { return "flower_allium"; } }
    public override string bottomName { get { return "flower_allium"; } }
    public override string frontName { get { return "flower_allium"; } }
    public override string backName { get { return "flower_allium"; } }
    public override string leftName { get { return "flower_allium"; } }
    public override string rightName { get { return "flower_allium"; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    List<int> triangles_poppy = new List<int>();
    List<int> triangles_daisy = new List<int>();
    List<int> triangles_houstonia = new List<int>();
    List<int> triangles_tulip_red = new List<int>();
    List<int> triangles_tulip_white = new List<int>();
    List<int> triangles_tulip_orange = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {

        List<int> triangles = null;

        switch (blockData)
        {
            case 0:
                triangles = triangles_poppy;
                break;
            case 3:
                triangles = triangles_houstonia;
                break;
            case 4:
                triangles = triangles_tulip_red;
                break;
            case 5:
                triangles = triangles_tulip_orange;
                break;
            case 6:
                triangles = triangles_tulip_white;
                break;
            case 8:
                triangles = triangles_daisy;
                break;
        }

        Vector3Int worldPos = pos + new Vector3Int(chunk.x, 0, chunk.z) * 16;
        if (triangles == null)
        {
            Debug.Log("pos=" + worldPos + ",data=" + blockData);

        }

        AddDiagonalFace(vertices, uv, triangles, pos);
        AddAntiDiagonalFace(vertices, uv, triangles, pos);
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_poppy.Count > 0)
        {
            trianglesList.Add(triangles_poppy);
            materialList.Add(Resources.Load<Material>("Materials/block/flower_poppy"));
        }
        if (triangles_daisy.Count > 0)
        {
            trianglesList.Add(triangles_daisy);
            materialList.Add(Resources.Load<Material>("Materials/block/flower_oxeye_daisy"));
        }
        if (triangles_houstonia.Count > 0)
        {
            trianglesList.Add(triangles_houstonia);
            materialList.Add(Resources.Load<Material>("Materials/block/flower_houstonia"));
        }
        if (triangles_tulip_red.Count > 0)
        {
            trianglesList.Add(triangles_tulip_red);
            materialList.Add(Resources.Load<Material>("Materials/block/flower_tulip_red"));
        }
        if (triangles_tulip_white.Count > 0)
        {
            trianglesList.Add(triangles_tulip_white);
            materialList.Add(Resources.Load<Material>("Materials/block/flower_tulip_white"));
        }
        if (triangles_tulip_orange.Count > 0)
        {
            trianglesList.Add(triangles_tulip_orange);
            materialList.Add(Resources.Load<Material>("Materials/block/flower_tulip_orange"));
        }
    }

    public override void ClearData()
    {
        triangles_poppy.Clear();
        triangles_daisy.Clear();
        triangles_houstonia.Clear();
        triangles_tulip_red.Clear();
        triangles_tulip_white.Clear();
        triangles_tulip_orange.Clear();
    }
}