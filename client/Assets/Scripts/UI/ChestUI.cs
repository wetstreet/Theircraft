using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChestUI : InventoryUI
{
    Transform chestGrid;

    static ChestUI Instance;

    public TagNodeList Items;

    static Animator animator;

    public static void Show(Vector3Int pos)
    {
        if (Instance != null)
        {
            Instance.InitData(pos);
            Instance.gameObject.SetActive(true);
            Instance.RefreshUI();
            Instance.RefreshGrabItem();
        }
        else
        {
            Instance = UISystem.InstantiateUI("ChestUI").GetComponent<ChestUI>();
            Instance.InitData(pos);
        }
        animator = GetChestAnimator(pos);

        SoundManager.Play2DSound("Player_Chest_Open");
        animator?.Play("chest_open");

        InputManager.enabled = false;
        PlayerController.LockCursor(false);
    }

    static Animator GetChestAnimator(Vector3Int pos)
    {
        NBTChunk chunk = NBTHelper.GetChunk(pos);
        Vector3Int localPos = new Vector3Int(pos.x - chunk.x * 16, pos.y, pos.z - chunk.z * 16);
        GameObject go = chunk.GetTileEntityObj(localPos);
        if (go)
        {
            Animator animator = go.GetComponent<Animator>();
            return animator;
        }
        return null;
    }

    public static void Hide()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }

        if (InventorySystem.grabItem.id != null)
        {
            InventorySystem.DropGrabItem();
        }

        SoundManager.Play2DSound("Player_Chest_Close");
        animator?.Play("chest_close");

        InputManager.enabled = true;
        PlayerController.LockCursor(true);

        SaveData();
    }

    protected override void HideInternal() { Hide(); }

    static void SaveData()
    {
        // save to chest
        Instance.Items.Clear();
        Instance.Items.ChangeValueType(TagType.TAG_COMPOUND);

        int count = 0;
        for (int i = 46; i <= 72; i++)
        {
            InventoryItem item = InventorySystem.items[i];
            if (item.id != null)
            {
                TagNodeCompound serializeItem = new TagNodeCompound();
                serializeItem.Add("Count", (TagNodeByte)item.count);
                serializeItem.Add("Damage", (TagNodeShort)item.damage);
                serializeItem.Add("id", (TagNodeString)item.id);
                serializeItem.Add("Slot", (TagNodeByte)(i - 46));
                Instance.Items.Insert(count, serializeItem);

                InventorySystem.items[i].id = null;
                InventorySystem.items[i].damage = 0;
                InventorySystem.items[i].count = 0;
            }
        }
    }

    protected override void InitComponents()
    {
        base.InitComponents();

        chestGrid = transform.Find("ChestGrid");
    }

    void InitData(Vector3Int pos)
    {
        NBTChunk chunk = NBTHelper.GetChunk(pos);
        if (chunk != null && chunk.tileEntityDict.ContainsKey(pos))
        {
            Items = (TagNodeList)chunk.tileEntityDict[pos]["Items"];
            foreach (TagNodeCompound item in Items)
            {
                byte slot = item["Slot"] as TagNodeByte;
                InventorySystem.items[slot + 46].id = item["id"] as TagNodeString;
                InventorySystem.items[slot + 46].damage = item["Damage"] as TagNodeShort;
                InventorySystem.items[slot + 46].count = item["Count"] as TagNodeByte;
            }
        }
    }

    protected override void InitGrid()
    {
        base.InitGrid();

        for (int i = 46; i <= 72; i++)
        {
            InitItem(i, chestGrid);
        }
    }

    protected override void RefreshUI()
    {
        base.RefreshUI();

        for (int i = 46; i <= 72; i++)
        {
            RefreshItem(i);
        }
    }

    protected override bool IsIndexValid(int index)
    {
        return index >= 46 && index <= 72;
    }
}