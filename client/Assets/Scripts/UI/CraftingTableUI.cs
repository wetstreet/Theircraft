using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingTableUI : InventoryUI
{
    Transform craftGrid;

    static CraftingTableUI Instance;

    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
            Instance.RefreshUI();
            Instance.RefreshGrabItem();
        }
        else
        {
            Instance = UISystem.InstantiateUI("CraftingTableUI").GetComponent<CraftingTableUI>();
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

        // clear crafting
        foreach (int i in indexList)
        {
            if (InventorySystem.items[i].id != null)
            {
                NBTObject obj = NBTGeneratorManager.GetObjectGenerator(InventorySystem.items[i].id);
                InventorySystem.Increment(obj, (byte)InventorySystem.items[i].damage, InventorySystem.items[i].count);
                InventorySystem.items[i].id = null;
                InventorySystem.items[i].damage = 0;
                InventorySystem.items[i].count = 0;
            }
        }
        InventorySystem.items[resultIndex].id = null;
        InventorySystem.items[resultIndex].damage = 0;
        InventorySystem.items[resultIndex].count = 0;

        ItemSelectPanel.instance.RefreshUI();
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

        craftGrid = transform.Find("CraftingGrid");
    }

    public static List<int> indexList = new List<int>() {
        36, 37, 38,
        39, 40, 41,
        42, 43, 44,
    };

    protected override void InitGrid()
    {
        base.InitGrid();

        foreach (int i in indexList)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.SetParent(craftGrid, false);
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);

            items[i].highlight = trans.Find("highlight").GetComponent<RawImage>();
            items[i].icon = trans.GetComponent<RawImage>();
            items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        }

        Transform resultTrans = transform.Find("CraftingResult/result");
        resultTrans.name = resultIndex.ToString();
        items[resultIndex].highlight = resultTrans.Find("highlight").GetComponent<RawImage>();
        items[resultIndex].icon = resultTrans.GetComponent<RawImage>();
        items[resultIndex].count = resultTrans.Find("text").GetComponent<TextMeshProUGUI>();
    }

    int[] refreshIndex = new int[] {
        36, 37, 38,
        39, 40, 41,
        42, 43, 44,
        45
    };
    protected override void RefreshUI()
    {
        base.RefreshUI();

        foreach (int i in refreshIndex)
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
            if (InventorySystem.grabItem.id == null ||
                (InventorySystem.grabItem.id == InventorySystem.items[resultIndex].id &&
                InventorySystem.grabItem.damage == InventorySystem.items[resultIndex].damage))
            {
                CraftingSystem.CraftItems();
                RefreshGrabItem();
                RefreshUI();
            }
        }
        if (indexList.Contains(highlightIndex))
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
        if (indexList.Contains(highlightIndex))
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