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

    public static InventoryItem[] items = new InventoryItem[36];

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

    public static void Increment(NBTObject generator, byte data, byte count)
    {
        // 优先加到已有的同类的里面
        for (int i = 0; i < 36; i++)
        {
            if (items[i].id == null) continue;

            if (!NBTGeneratorManager.id2type.ContainsKey(items[i].id))
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
}
