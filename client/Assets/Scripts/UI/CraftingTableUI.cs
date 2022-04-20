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

    protected override bool checkCraft { get { return true; } }

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

    protected override void HideInternal() { Hide(); }

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
            InitItem(i, craftGrid);
        }

        InitItem(resultIndex, "CraftingResult/result");
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
            RefreshItem(i);
        }
    }

    protected override bool IsIndexValid(int index)
    {
        return indexList.Contains(index);
    }
}