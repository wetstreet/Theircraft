using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTChest : NBTBlock
{
    public override string name { get { return "Chest"; } }
    public override string id { get { return "minecraft:chest"; } }

    public override float topOffset => 0.385f;
    public override float radius => 0.4475f;
    public override bool useRadius => true;

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
        GameObject chest_prefab = Resources.Load<GameObject>("Meshes/entity/chest/chest_prefab");
        GameObject chest = Object.Instantiate(chest_prefab);
        chest.transform.parent = chunk.special.transform;
        chest.transform.localPosition = pos;
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
}
