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
}
