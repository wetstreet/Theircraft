using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceItem
{
    public byte count;
    public short damage;
    public string id;
    public byte slot;

    public FurnaceItem() { }

    public FurnaceItem(string _id)
    {
        count = 1;
        damage = 0;
        id = _id;
        slot = 2;
    }

    public FurnaceItem(TagNodeCompound item)
    {
        count = item["Count"].ToTagByte();
        damage = item["Damage"].ToTagShort();
        id = item["id"].ToTagString();
        slot = item["Slot"].ToTagByte();
    }
}

public class FurnaceData
{
    public FurnaceItem source; // slot 0
    public FurnaceItem fuel; // slot 1
    public FurnaceItem result; // slot 2
    public short burnTime;
    public short cookTime;
    public short cookTimeTotal;

    public FurnaceData(TagNodeCompound node)
    {
        burnTime = node["BurnTime"].ToTagShort();
        cookTime = node["CookTime"].ToTagShort();
        cookTimeTotal = node["CookTimeTotal"].ToTagShort();
        var items = node["Items"].ToTagList();
        foreach (TagNodeCompound item in items)
        {
            var slot = item["Slot"] as TagNodeByte;
            if (slot == 0)
                source = new FurnaceItem(item);
            else if (slot == 1)
                fuel = new FurnaceItem(item);
            else if (slot == 2)
                result = new FurnaceItem(item);
        }
    }

    public void Tick()
    {
        if (burnTime > 0) // has fuel
        {
            burnTime--;

            if (source != null)
            {
                NBTObject sourceItem = NBTGeneratorManager.GetObjectGenerator(source.id);
                if (result == null || sourceItem.smeltResult == result.id)
                {
                    cookTime++;
                    if (cookTime == cookTimeTotal) // done one smelting
                    {
                        cookTime = 0;

                        if (result == null)
                        {
                            if (sourceItem.smeltResult != null)
                            {
                                result = new FurnaceItem(sourceItem.smeltResult);
                            }
                        }
                        else
                        {
                            result.count++;
                        }

                        source.count--;
                        if (source.count == 0)
                        {
                            source = null;
                        }
                        FurnaceUI.Refresh();
                    }
                }
            }
        }
        else if (fuel != null && fuel.count > 0 && source != null && source.count > 0)
        {
            NBTObject sourceItem = NBTGeneratorManager.GetObjectGenerator(source.id);
            if (sourceItem.smeltResult != null)
            {
                NBTObject fuelItem = NBTGeneratorManager.GetObjectGenerator(fuel.id);
                burnTime = fuelItem.burningTime;
                cookTimeTotal = sourceItem.cookTimeTotal;
                fuel.count--;
                if (fuel.count == 0)
                {
                    fuel = null;
                }
                FurnaceUI.Refresh();
            }
        }
    }

    public void Serialize(TagNodeCompound node)
    {
        node["BurnTime"] = (TagNodeShort)burnTime;
        node["CookTime"] = (TagNodeShort)cookTime;
        node["CookTimeTotal"] = (TagNodeShort)cookTimeTotal;
        var Items = node["Items"].ToTagList();

        Items.Clear();
        Items.ChangeValueType(TagType.TAG_COMPOUND);
        if (source != null)
        {
            TagNodeCompound item = new TagNodeCompound();
            item.Add("Count", (TagNodeByte)source.count);
            item.Add("Damage", (TagNodeShort)source.damage);
            item.Add("id", (TagNodeString)source.id);
            item.Add("Slot", (TagNodeByte)0);
            Items.Add(item);
        }
        if (fuel != null)
        {
            TagNodeCompound item = new TagNodeCompound();
            item.Add("Count", (TagNodeByte)fuel.count);
            item.Add("Damage", (TagNodeShort)fuel.damage);
            item.Add("id", (TagNodeString)fuel.id);
            item.Add("Slot", (TagNodeByte)1);
            Items.Add(item);
        }
        if (result != null)
        {
            TagNodeCompound item = new TagNodeCompound();
            item.Add("Count", (TagNodeByte)result.count);
            item.Add("Damage", (TagNodeShort)result.damage);
            item.Add("id", (TagNodeString)result.id);
            item.Add("Slot", (TagNodeByte)2);
            Items.Add(item);
        }
    }
}

public class NBTFurnace : NBTBlock
{
    public override string name => "Furnace";
    public override string id => "minecraft:furnace";

    public override float hardness => 3.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.RockI;
    public override SoundMaterial soundMaterial => SoundMaterial.Stone;

    public override string frontName => "furnace_front_off";
    public override string backName => "furnace_side";
    public override string leftName => "furnace_side";
    public override string rightName => "furnace_side";
    public override string topName => "furnace_top";
    public override string bottomName => "furnace_top";

    public override string GetBreakEffectTexture(byte data)
    {
        return "furnace_front_off";
    }

    public override byte GetDropItemData(byte data) { return 0; }

    public override bool canInteract => true;
    public override void OnRightClick()
    {
        FurnaceUI.Show(WireFrameHelper.pos);
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;

        ca.pos = pos;
        ca.worldPos.Set(pos.x + chunk.x * 16, pos.y, pos.z + chunk.z * 16);
        ca.blockData = blockData;

        InitBlockAttributes(chunk, ref ca);

        if (ca.front.exist == 0)
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (blockData == 2)
                fa.uv = TextureArrayManager.GetUVByName("furnace_front_off");
            else
                fa.uv = TextureArrayManager.GetUVByName("furnace_side");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.right.exist == 0)
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (blockData == 5)
                fa.uv = TextureArrayManager.GetUVByName("furnace_front_off");
            else
                fa.uv = TextureArrayManager.GetUVByName("furnace_side");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.left.exist == 0)
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (blockData == 4)
                fa.uv = TextureArrayManager.GetUVByName("furnace_front_off");
            else
                fa.uv = TextureArrayManager.GetUVByName("furnace_side");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.back.exist == 0)
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (blockData == 3)
                fa.uv = TextureArrayManager.GetUVByName("furnace_front_off");
            else
                fa.uv = TextureArrayManager.GetUVByName("furnace_side");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.top.exist == 0)
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (ca.bottom.exist == 0)
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }

    public override void OnAddBlock(RaycastHit hit)
    {
        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte type = NBTGeneratorManager.id2type[id];
        byte data = CalcBlockDirection(pos, 3, 4, 2, 5);

        NBTHelper.SetBlockData(pos, type, data);

        NBTChunk chunk = NBTHelper.GetChunk(pos);
        TagNodeCompound node = CreateEmptyFurnaceNode(pos);
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

    TagNodeCompound CreateEmptyFurnaceNode(Vector3Int pos)
    {
        TagNodeCompound node = new TagNodeCompound();
        TagNodeList items = new TagNodeList(TagType.TAG_COMPOUND);
        node.Add("Items", items);
        node.Add("BurnTime", new TagNodeShort(0));
        node.Add("CookTime", new TagNodeShort(0));
        node.Add("CookTimeTotal", new TagNodeShort(0));
        node.Add("id", new TagNodeString(id));
        node.Add("Lock", new TagNodeString());
        node.Add("x", new TagNodeInt(pos.x));
        node.Add("y", new TagNodeInt(pos.y));
        node.Add("z", new TagNodeInt(pos.z));
        return node;
    }
}