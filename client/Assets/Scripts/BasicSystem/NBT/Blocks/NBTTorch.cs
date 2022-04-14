using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTTorch : NBTBlock
{
    public override string name => "Torch";
    public override string id => "minecraft:torch";

    public override float hardness => 0;

    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override bool isTileEntity => true;

    public override bool isTransparent => true;

    public override Vector3 itemSize => Vector3.one;

    public override string GetIconPathByData(short data = 0)
    {
        return "torch_on";
    }

    public override byte GetDropItemData(byte data) { return 0; }

    public override string pathPrefix => "GUI/block/";

    public override string GetBreakEffectTexture(byte data)
    {
        return "torch_on";
    }

    public override bool CanAddBlock(Vector3Int pos)
    {
        byte type = NBTHelper.GetBlockByte(pos);
        return type == 0;
    }

    public override void OnAddBlock(RaycastHit hit)
    {
        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte type = NBTGeneratorManager.id2type[id];
        byte data = (byte)InventorySystem.items[ItemSelectPanel.curIndex].damage;
        if (hit.normal == Vector3.back)
        {
            data = 4;
        }
        else if (hit.normal == Vector3.forward)
        {
            data = 3;
        }
        else if (hit.normal == Vector3.left)
        {
            data = 2;
        }
        else if (hit.normal == Vector3.right)
        {
            data = 1;
        }

        NBTChunk chunk = NBTHelper.GetChunk(pos);
        pos.x -= chunk.x * 16;
        pos.z -= chunk.z * 16;
        chunk.SetBlockData(pos.x, pos.y, pos.z, type, data);
        chunk.AddTileEntityObj(pos, this, data);
    }

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

    public override void RenderWireframe(byte blockData)
    {
        if (blockData == 0)
        {
            float top = 0.135f;
            float bottom = -0.501f;
            float left = -0.1f;
            float right = 0.1f;
            float front = 0.1f;
            float back = -0.1f;

            RenderWireframeByVertex(top, bottom, left, right, front, back);
        }
        else if (blockData == 1)
        {
            float top = 0.3125f;
            float bottom = -0.3125f;
            float left = -0.5f;
            float right = -0.185f;
            float front = 0.15f;
            float back = -0.15f;

            RenderWireframeByVertex(top, bottom, left, right, front, back);
        }
        else if (blockData == 2)
        {
            float top = 0.3125f;
            float bottom = -0.3125f;
            float left = 0.185f;
            float right = 0.5f;
            float front = 0.15f;
            float back = -0.15f;

            RenderWireframeByVertex(top, bottom, left, right, front, back);
        }
        else if (blockData == 3)
        {
            float top = 0.3125f;
            float bottom = -0.3125f;
            float left = -0.15f;
            float right = 0.15f;
            float front = -0.185f;
            float back = -0.5f;

            RenderWireframeByVertex(top, bottom, left, right, front, back);
        }
        else if (blockData == 4)
        {
            float top = 0.3125f;
            float bottom = -0.3125f;
            float left = -0.15f;
            float right = 0.15f;
            float front = 0.5f;
            float back = 0.185f;

            RenderWireframeByVertex(top, bottom, left, right, front, back);
        }
    }
}
