using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTYellowFlower : NBTBlock
{
    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    List<int> triangles_dandelion = new List<int>();
    List<int> triangles_daisy = new List<int>();
    List<int> triangles_houstonia = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {

        List<int> triangles = null;

        switch (blockData)
        {
            case 0:
                triangles = triangles_dandelion;
                break;
            //case 3:
            //    triangles = triangles_houstonia;
            //    break;
            //case 8:
            //    triangles = triangles_daisy;
            //    break;
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
        if (triangles_dandelion.Count > 0)
        {
            trianglesList.Add(triangles_dandelion);
            materialList.Add(Resources.Load<Material>("Materials/block/flower_dandelion"));
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
    }

    public override void ClearData()
    {
        triangles_dandelion.Clear();
        triangles_daisy.Clear();
        triangles_houstonia.Clear();
    }
}