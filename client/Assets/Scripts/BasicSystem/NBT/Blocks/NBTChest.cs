using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTChest : NBTBlock
{
    public override string name => "Chest";
    public override string id => "minecraft:chest";

    public override float hardness => 2.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override short burningTime => 300;

    public override bool isTileEntity => true;

    public override bool isTransparent => true;

    GameObject chest_prefab;

    public override string GetBreakEffectTexture(byte data)
    {
        return "planks_oak";
    }

    public override GameObject GetTileEntityGameObject(NBTChunk chunk, byte blockData, Vector3Int pos)
    {
        if (chest_prefab == null)
        {
            chest_prefab = Resources.Load<GameObject>("Meshes/entity/chest/chest_prefab");
        }
        GameObject chest = Object.Instantiate(chest_prefab);
        chest.transform.parent = chunk.special.transform;
        chest.transform.localPosition = pos;
        int y = 0;
        switch (blockData)
        {
            case 2:
                y = 0;
                break;
            case 3:
                y = 180;
                break;
            case 4:
                y = 90;
                break;
            case 5:
                y = -90;
                break;
        }
        chest.transform.localEulerAngles = new Vector3(0, y, 0);

        return chest;
    }

    public override void OnAddBlock(RaycastHit hit)
    {
        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte type = NBTGeneratorManager.id2type[id];
        byte data = CalcBlockDirection(pos, 3, 4, 2, 5);

        NBTChunk chunk = NBTHelper.GetChunk(pos);

        Vector3Int localPos = new Vector3Int(pos.x - chunk.x * 16, pos.y, pos.z - chunk.z * 16);
        chunk.SetBlockData(localPos.x, localPos.y, localPos.z, type, data);
        chunk.AddTileEntityObj(localPos, this, data);

        TagNodeCompound node = CreateEmptyChestNode(pos);
        chunk.AddTileEntity(pos, node);
    }

    public override void OnDestroyBlock(Vector3Int globalPos, byte blockData)
    {
        NBTChunk chunk = NBTHelper.GetChunk(globalPos);

        if (chunk != null && chunk.tileEntityDict.ContainsKey(globalPos))
        {
            TagNodeList Items = (TagNodeList)chunk.tileEntityDict[globalPos]["Items"];
            foreach (TagNodeCompound item in Items)
            {
                string id = item["id"] as TagNodeString;
                short damage = item["Damage"] as TagNodeShort;
                byte count = item["Count"] as TagNodeByte;
                Item.CreateBlockDropItem(id, (byte)damage, globalPos, count);
            }
        }

        chunk.RemoveTileEntity(globalPos);
    }

    TagNodeCompound CreateEmptyChestNode(Vector3Int pos)
    {
        TagNodeCompound node = new TagNodeCompound();
        TagNodeList items = new TagNodeList(TagType.TAG_COMPOUND);
        node.Add("Items", items);
        node.Add("id", new TagNodeString(id));
        node.Add("Lock", new TagNodeString());
        node.Add("x", new TagNodeInt(pos.x));
        node.Add("y", new TagNodeInt(pos.y));
        node.Add("z", new TagNodeInt(pos.z));
        return node;
    }

    Mesh GetMesh(byte blockData)
    {
        if (!itemMeshDict.ContainsKey(0))
        {
            itemMeshDict[0] = Resources.Load<Mesh>("Meshes/entity/chest/all");
        }
        return itemMeshDict[0];
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
        if (!itemMaterialDict.ContainsKey(0))
        {
            itemMaterialDict[0] = Resources.Load<Material>("Meshes/entity/chest/chest_mat");
        }
        return itemMaterialDict[0];
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
