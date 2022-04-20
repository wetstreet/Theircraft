using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryItem
{
    public string id;
    public short damage;
    public byte count;
}

public class InventorySystem
{
    // 0-35 is bag(36)
    // 36-44 is crafting(9)
    // ----------
    // |36 37 38|
    // |39 40 41|
    // |42 43 44|
    // ----------
    // 45 is craft result(1)
    // 46-72 is small chest(27)
    // 46-99 is big chest(54)
    // 100-101 is furnace ore and fuel
    // 102-146 is creative inventory(45)
    public static InventoryItem[] items = new InventoryItem[147];

    public static InventoryItem grabItem;

    public static void Init()
    {
        TagNodeCompound playerData = NBTHelper.GetPlayerData();
        TagNodeList Inventory = playerData["Inventory"] as TagNodeList;

        for (int i = 0; i < Inventory.Count; i++)
        {
            TagNodeCompound item = Inventory[i] as TagNodeCompound;
            byte slot = item["Slot"] as TagNodeByte;
            byte count = item["Count"] as TagNodeByte;
            short damage = item["Damage"] as TagNodeShort;
            string id = item["id"] as TagNodeString;

            if (slot < 36)
            {
                items[slot].id = id;
                items[slot].count = count;
                items[slot].damage = damage;
            }
        }
    }

    public static void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameModeManager.isCreative)
                CreativeInventory.Show();
            else
                SurvivalInventory.Show();
        }
    }

    public static void MouseGrabItem(int index, bool checkCraft)
    {
        InventoryItem tempItem = grabItem;
        grabItem = items[index];
        items[index] = tempItem;

        if (checkCraft)
        {
            CraftingSystem.CheckCanCraft();
        }
    }

    public static void PutOneItem(int index, bool checkCraft)
    {
        items[index].id = grabItem.id;
        items[index].damage = grabItem.damage;
        items[index].count++;

        grabItem.count--;
        if (grabItem.count == 0)
        {
            ClearGrabItem();
        }

        if (checkCraft)
        {
            CraftingSystem.CheckCanCraft();
        }
    }

    public static void ClearGrabItem()
    {
        grabItem.id = null;
        grabItem.damage = 0;
        grabItem.count = 0;
    }

    public static void PutItems(int index, bool checkCraft)
    {
        // todo: max stack count
        items[index].count += grabItem.count;

        ClearGrabItem();

        if (checkCraft)
        {
            CraftingSystem.CheckCanCraft();
        }
    }

    public static void SplitHalf(int index)
    {
        byte half = (byte)Mathf.CeilToInt(items[index].count / 2f);
        items[index].count -= half;

        grabItem.id = items[index].id;
        grabItem.damage = items[index].damage;
        grabItem.count = half;
    }

    public static void DropGrabItem()
    {
        NBTObject generator = NBTGeneratorManager.GetObjectGenerator(grabItem.id);
        Item.CreatePlayerDropItem(generator, (byte)grabItem.damage, grabItem.count);

        ClearGrabItem();
    }

    public static void Increment(NBTObject generator, byte data, byte count)
    {
        // 优先加到已有的同类的里面
        for (int i = 0; i < 36; i++)
        {
            if (items[i].id == null) continue;

            if (NBTGeneratorManager.GetObjectGenerator(items[i].id) == null)
            {
                Debug.Log("cannot get type,slot=" + i + ",id=" + items[i].id);
                continue;
            }

            string id = items[i].id;
            short d = items[i].damage;
            byte c = items[i].count;
            if (id == generator.id && d == data && c < generator.maxStackCount)
            {
                int diff = generator.maxStackCount - c;

                if (count > diff)
                {
                    items[i].count = generator.maxStackCount;
                    count -= (byte)diff;
                }
                else
                {
                    items[i].count += count;
                    return;
                }

            }
        }
        //已有的放不下，需要放在空格内
        for (int i = 0; i < 36; i++)
        {
            if (items[i].id == null)
            {
                items[i].id = generator.id;
                items[i].damage = data;

                if (count > generator.maxStackCount)
                {
                    items[i].count = generator.maxStackCount;
                    count -= generator.maxStackCount;
                }
                else
                {
                    items[i].count = count;
                    return;
                }
            }
        }
    }

    public static void DecrementCurrent()
    {
        uint slot = ItemSelectPanel.curIndex;
        items[slot].count--;
        if (items[slot].count == 0)
        {
            items[slot].id = null;
            items[slot].damage = 0;
        }
    }

    public static void SaveData(TagNodeList Inventory)
    {
        Inventory.Clear();
        Inventory.ChangeValueType(TagType.TAG_COMPOUND);

        int count = 0;
        for (int i = 0; i < 36; i++)
        {
            InventoryItem item = items[i];
            if (item.id != null)
            {
                TagNodeCompound serializeItem = new TagNodeCompound();
                serializeItem.Add("Count", (TagNodeByte)item.count);
                serializeItem.Add("Damage", (TagNodeShort)item.damage);
                serializeItem.Add("id", (TagNodeString)item.id);
                serializeItem.Add("Slot", (TagNodeByte)i);
                Inventory.Insert(count, serializeItem);
            }
        }
    }

}
