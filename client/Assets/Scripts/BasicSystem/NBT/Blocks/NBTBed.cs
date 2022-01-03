using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBed : NBTBlock
{
    public override string name => "Bed";
    public override string id => "minecraft:bed";

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override float hardness => 0.2f;

    public override bool isTileEntity => true;

    public override bool isTransparent => true;

    // 0 = south foot
    // 1 = west foot
    // 2 = north foot
    // 3 = east foot
    // 8 = south head
    // 9 = west head
    // 10 = north head
    // 11 = east head
    Mesh GetMesh(byte blockData)
    {
        if (!itemMeshDict.ContainsKey(blockData))
        {
            switch (blockData)
            {
                case 11:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_east");
                    break;
                case 3:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_east");
                    break;
                case 8:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_north");
                    break;
                case 0:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_north");
                    break;
                case 9:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_west");
                    break;
                case 1:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_west");
                    break;
                case 10:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_head_south");
                    break;
                case 2:
                    itemMeshDict[blockData] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_south");
                    break;
            }
        }
        return itemMeshDict[blockData];
    }
    Material GetMaterial(byte blockData)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            itemMaterialDict[0] = Resources.Load<Material>("Materials/bed");
        }
        return itemMaterialDict[0];
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        Debug.Log("bed addcube");
        Mesh mesh = GetMesh(blockData);
        GameObject bed = new GameObject("bed");
        bed.transform.parent = chunk.special.transform;
        bed.transform.localPosition = pos;
        bed.AddComponent<MeshFilter>().sharedMesh = mesh;
        bed.AddComponent<MeshRenderer>().sharedMaterial = GetMaterial(blockData);
        bed.AddComponent<MeshCollider>().sharedMesh = mesh;
        bed.layer = LayerMask.NameToLayer("Chunk");
    }

    Mesh combinedMesh;
    public override Mesh GetItemMesh(byte blockData)
    {
        if (combinedMesh == null)
        {
            Mesh head = GetMesh(8);
            Mesh foot = GetMesh(0);

            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = head;
            combine[0].transform = Matrix4x4.Translate(Vector3.forward);
            combine[1].mesh = foot;
            combine[1].transform = Matrix4x4.identity;

            combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
        }
        return combinedMesh;
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        return GetItemMesh(blockData);
    }

    public override Material GetItemMaterial(byte data)
    {
        return GetMaterial(data);
    }

    public override void RenderWireframe(byte blockData)
    {
        float top = 0.0635f;
        float bottom = -0.501f;
        float left = -0.501f;
        float right = 0.501f;
        float front = 0.501f;
        float back = -0.501f;

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}
