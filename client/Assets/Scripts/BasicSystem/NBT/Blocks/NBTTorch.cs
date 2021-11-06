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

    public override float breakNeedTime => 0;

    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override bool isTileEntity => true;

    public override bool isTransparent => true;

    public override string GetIconPathByData(short data = 0)
    {
        return "torch_on";
    }

    public override byte GetDropItemData(byte data) { return 0; }

    public override string pathPrefix => "GUI/block/";

    Material GetMaterial(byte blockData)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            itemMaterialDict[0] = Resources.Load<Material>("Materials/torch");
        }
        return itemMaterialDict[0];
    }

    // 1 east
    // 2 west
    // 3 south
    // 4 north
    // 5 up

    public override GameObject GetTileEntityGameObject(NBTChunk chunk, byte blockData, Vector3Int pos)
    {
        GameObject torch_prefab;
        switch (blockData)
        {
            case 1:
                torch_prefab = Resources.Load<GameObject>("Prefabs/Blocks/torch_+x");
                break;
            case 2:
                torch_prefab = Resources.Load<GameObject>("Prefabs/Blocks/torch_-x");
                break;
            case 3:
                torch_prefab = Resources.Load<GameObject>("Prefabs/Blocks/torch_+z");
                break;
            case 4:
                torch_prefab = Resources.Load<GameObject>("Prefabs/Blocks/torch_-z");
                break;
            default:
                torch_prefab = Resources.Load<GameObject>("Prefabs/Blocks/torch");
                break;
        }
        GameObject torch = Object.Instantiate(torch_prefab);
        torch.transform.parent = chunk.special.transform;
        torch.transform.localPosition = pos;

        return torch;
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte data)
    {
        if (!itemMeshDict.ContainsKey(data))
        {
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(data));
            Mesh mesh = ItemMeshGenerator.instance.Generate(tex);
            itemMeshDict.Add(data, mesh);
        }
        return itemMeshDict[data];
    }

    public override Material GetItemMaterial(byte data)
    {
        return GetMaterial(data);
    }
}
