using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTChest : NBTBlock
{
    public override string name { get { return "Chest"; } }
    public override string id { get { return "minecraft:chest"; } }

    public override float hardness => 2.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override bool isTileEntity => true;

    public override bool isTransparent => true;

    Mesh GetMesh(byte blockData)
    {
        if (!itemMeshDict.ContainsKey(0))
        {
            itemMeshDict[0] = Resources.Load<Mesh>("Meshes/blocks/bed/bed_foot_south");
        }
        return itemMeshDict[0];
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
        UnityEngine.Profiling.Profiler.BeginSample(GetType().Name + " AddCube");

        GameObject chest_prefab = Resources.Load<GameObject>("Meshes/entity/chest/chest_prefab");
        GameObject chest = Object.Instantiate(chest_prefab);
        chest.transform.parent = chunk.special.transform;
        chest.transform.localPosition = pos;

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public override Mesh GetItemMesh(byte blockData)
    {
        return GetMesh(blockData);
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        return GetMesh(blockData);
    }

    public override Material GetItemMaterial(byte data)
    {
        return GetMaterial(data);
    }

    public override bool canInteract => true;
    public override void OnRightClick()
    {
        ChestUI.Show(WireFrameHelper.pos);
    }

    public override void RenderWireframe(byte blockData)
    {
        float top = 0.385f;
        float bottom = -0.501f;
        float left = -0.4475f;
        float right = 0.4475f;
        float front = 0.4475f;
        float back = -0.4475f;

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}
