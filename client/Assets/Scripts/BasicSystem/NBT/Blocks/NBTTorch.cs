using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTTorch : NBTBlock
{
    public override string name { get { return "Torch"; } }
    public override string id { get { return "minecraft:torch"; } }

    public override float topOffset => 0.135f;
    public override float radius => 0.0725f;
    public override bool useRadius => true;

    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override bool isTileEntity => true;

    public override bool isTransparent => true;

    Mesh GetMesh(byte blockData)
    {
        if (!itemMeshDict.ContainsKey(0))
        {
            itemMeshDict[0] = Resources.Load<Mesh>("Meshes/items/torch/torch");
        }
        return itemMeshDict[0];
    }
    Material GetMaterial(byte blockData)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            itemMaterialDict[0] = Resources.Load<Material>("Materials/torch");
        }
        return itemMaterialDict[0];
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        GameObject torch_prefab = Resources.Load<GameObject>("Prefabs/Blocks/torch");
        GameObject torch = Object.Instantiate(torch_prefab);
        torch.transform.parent = chunk.special.transform;
        torch.transform.localPosition = pos;
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
