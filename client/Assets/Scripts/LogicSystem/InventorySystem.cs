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
    public static InventoryItem[] items = new InventoryItem[41];

    public static InventoryItem grabItem;

    static NBTObject UpperLeft
    {
        get
        {
            if (items[36].id != null)
                return NBTGeneratorManager.GetObjectGenerator(items[36].id);
            return null;
        }
    }
    static NBTObject UpperRight
    {
        get
        {
            if (items[37].id != null)
                return NBTGeneratorManager.GetObjectGenerator(items[37].id);
            return null;
        }
    }
    static NBTObject BottomLeft
    {
        get
        {
            if (items[38].id != null)
                return NBTGeneratorManager.GetObjectGenerator(items[38].id);
            return null;
        }
    }
    static NBTObject BottomRight
    {
        get
        {
            if (items[39].id != null)
                return NBTGeneratorManager.GetObjectGenerator(items[39].id);
            return null;
        }
    }

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

            items[slot].id = id;
            items[slot].count = count;
            items[slot].damage = damage;
        }
    }

    public static void MouseGrabItem(int index)
    {
        InventoryItem tempItem = grabItem;
        grabItem = items[index];
        items[index] = tempItem;

        CheckCanCraft();
    }

    public static void PutOneItem(int index)
    {
        items[index].id = grabItem.id;
        items[index].damage = grabItem.damage;
        items[index].count++;

        grabItem.count--;
        if (grabItem.count == 0)
        {
            grabItem.id = null;
            grabItem.damage = 0;
        }

        CheckCanCraft();
    }

    public static void PutItems(int index)
    {
        // todo: max stack count
        items[index].count += grabItem.count;

        grabItem.id = null;
        grabItem.damage = 0;
        grabItem.count = 0;

        CheckCanCraft();
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
        grabItem.id = null;
        grabItem.damage = 0;
        grabItem.count = 0;
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

    public static void CraftItems()
    {
        if (items[40].id == null) return;

        for (int i = 36; i < 40; i++)
        {
            if (items[i].count > 0)
            {
                items[i].count--;

                if (items[i].count == 0)
                {
                    items[i].id = null;
                    items[i].damage = 0;
                }
            }
        }

        grabItem.id = items[40].id;
        grabItem.damage = items[40].damage;
        grabItem.count = items[40].count;

        CheckCanCraft();
    }

    static void CheckCanCraft()
    {
        items[40].id = null;
        items[40].damage = 0;
        items[40].count = 0;

        if (UpperLeft is NBTPlanks && UpperRight is NBTPlanks && BottomLeft is NBTPlanks && BottomRight is NBTPlanks)
        {
            items[40].id = "minecraft:crafting_table";
            items[40].damage = 0;
            items[40].count = 1;
        }
        else if ((UpperLeft is NBTPlanks && UpperRight is null && BottomLeft is NBTPlanks && BottomRight is null) ||
            (UpperLeft is null && BottomLeft is NBTPlanks && UpperRight is null && BottomRight is NBTPlanks))
        {
            items[40].id = "minecraft:stick";
            items[40].damage = 0;
            items[40].count = 4;
        }
    }
}
