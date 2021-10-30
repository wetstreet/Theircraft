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

    public Vector3Int pos;
    public TagNodeList Items;

    public static void Show(Vector3Int pos)
    {
        if (Instance != null)
        {
            Instance.pos = pos;
            Instance.InitData();
            Instance.gameObject.SetActive(true);
            Instance.RefreshUI();
            Instance.RefreshGrabItem();
        }
        else
        {
            Instance = UISystem.InstantiateUI("ChestUI").GetComponent<ChestUI>();
            Instance.pos = pos;
            Instance.InitData();
        }

        InputManager.enabled = false;
        PlayerController.LockCursor(false);
    }

    public static void Hide()
    {
        InputManager.enabled = true;
        PlayerController.LockCursor(true);
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }

        if (InventorySystem.grabItem.id != null)
        {
            InventorySystem.DropGrabItem();
        }

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

    void HandleInputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            Hide();
        }
    }

    protected override void InitComponents()
    {
        base.InitComponents();

        chestGrid = transform.Find("ChestGrid");
    }

    protected void InitData()
    {
        NBTChunk chunk = NBTHelper.GetChunk(pos);
        foreach (TagNodeCompound node in chunk.TileEntities)
        {
            int x = node["x"] as TagNodeInt;
            int y = node["y"] as TagNodeInt;
            int z = node["z"] as TagNodeInt;

            if (x == pos.x && y == pos.y && z == pos.z)
            {
                Items = node["Items"] as TagNodeList;
                foreach (TagNodeCompound item in Items)
                {
                    byte slot = item["Slot"] as TagNodeByte;
                    InventorySystem.items[slot + 46].id = item["id"] as TagNodeString;
                    InventorySystem.items[slot + 46].damage = item["Damage"] as TagNodeShort;
                    InventorySystem.items[slot + 46].count = item["Count"] as TagNodeByte;
                }
                break;
            }
        }
    }

    protected override void InitGrid()
    {
        base.InitGrid();

        for (int i = 46; i <= 72; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.SetParent(chestGrid, false);
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);

            items[i].highlight = trans.Find("highlight").GetComponent<RawImage>();
            items[i].icon = trans.GetComponent<RawImage>();
            items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        }
    }

    protected override void RefreshUI()
    {
        base.RefreshUI();

        for (int i = 46; i <= 72; i++)
        {
            InventoryItem item = InventorySystem.items[i];
            if (item.id != null)
            {
                items[i].icon.enabled = true;
                items[i].icon.texture = BlockIconHelper.GetIcon(item.id, item.damage);
                if (item.count > 1)
                {
                    items[i].count.enabled = true;
                    items[i].count.text = item.count.ToString();
                }
                else
                {
                    items[i].count.enabled = false;
                }
            }
            else
            {
                items[i].icon.enabled = false;
                items[i].count.enabled = false;
            }
            items[i].highlight.color = i == highlightIndex ? highlightColor : Color.clear;
        }
    }

    protected override void OnLeftMouseClick()
    {
        base.OnLeftMouseClick();

        if (highlightIndex == resultIndex)
        {
            if (InventorySystem.grabItem.id == null)
            {
                CraftingSystem.CraftItems();
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        if (highlightIndex >= 46 && highlightIndex <= 72)
        {
            if (InventorySystem.grabItem.id != null &&
                InventorySystem.items[highlightIndex].id != null &&
                InventorySystem.grabItem.id == InventorySystem.items[highlightIndex].id &&
                InventorySystem.grabItem.damage == InventorySystem.items[highlightIndex].damage)
            {
                InventorySystem.PutItems(highlightIndex);
            }
            else
            {
                InventorySystem.MouseGrabItem(highlightIndex);
            }
            RefreshGrabItem();
            RefreshUI();
            ItemSelectPanel.instance.RefreshUI();
        }
    }

    protected override void OnRightMouseClick()
    {
        base.OnRightMouseClick();

        if (highlightIndex == resultIndex)
        {
            if (InventorySystem.grabItem.id == null)
            {
                InventorySystem.MouseGrabItem(highlightIndex);
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        if (highlightIndex >= 46 && highlightIndex <= 72)
        {
            if (InventorySystem.grabItem.id != null)
            {
                if (InventorySystem.items[highlightIndex].id != null)
                {
                    if (InventorySystem.grabItem.id == InventorySystem.items[highlightIndex].id &&
                        InventorySystem.grabItem.damage == InventorySystem.items[highlightIndex].damage)
                        InventorySystem.PutOneItem(highlightIndex);
                    else
                        InventorySystem.MouseGrabItem(highlightIndex);
                }
                else
                {
                    InventorySystem.PutOneItem(highlightIndex);
                }
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
            else if (InventorySystem.items[highlightIndex].id != null)
            {
                InventorySystem.SplitHalf(highlightIndex);
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        HandleInputUpdate();
    }
}