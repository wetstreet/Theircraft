using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FurnaceUI : InventoryUI
{
    static FurnaceUI Instance;

    public Vector3Int pos;

    RectMask2D fire;
    RectMask2D arrow;

    public static void Show(Vector3Int pos)
    {
        if (Instance != null)
        {
            Instance.pos = pos;
            Instance.gameObject.SetActive(true);
            Instance.InitData();
            Instance.RefreshUI();
            Instance.RefreshGrabItem();
        }
        else
        {
            Instance = UISystem.InstantiateUI("FurnaceUI").GetComponent<FurnaceUI>();
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

        ItemSelectPanel.instance.RefreshUI();
    }

    protected override void HideInternal() { Hide(); }

    private void OnDisable()
    {
        ClearData();
    }

    protected override void InitComponents()
    {
        base.InitComponents();
    }

    public static List<int> indexList = new List<int>() {
        oreIndex,
        fuelIndex
    };

    protected override void InitGrid()
    {
        base.InitGrid();

        arrow = transform.Find("bg/arrow").GetComponent<RectMask2D>();
        fire = transform.Find("bg/fire").GetComponent<RectMask2D>();

        InitItem(oreIndex, "Ore");
        InitItem(fuelIndex, "Fuel");
        InitItem(resultIndex, "CraftingResult/result");
    }

    static short burnTime;
    static short cookTime;
    static short cookTimeTotal;

    void InitData()
    {
        NBTChunk chunk = NBTHelper.GetChunk(pos);
        if (chunk != null && chunk.tileEntityDict.ContainsKey(pos))
        {
            TagNodeCompound furnace = chunk.tileEntityDict[pos];

            burnTime = (TagNodeShort)furnace["BurnTime"];
            cookTime = (TagNodeShort)furnace["CookTime"];
            cookTimeTotal = (TagNodeShort)furnace["CookTimeTotal"];

            TagNodeList Items = (TagNodeList)furnace["Items"];
            foreach (TagNodeCompound item in Items)
            {
                int index = -1;
                byte slot = (TagNodeByte)item["Slot"];
                switch (slot)
                {
                    case 0:
                        index = oreIndex;
                        break;
                    case 1:
                        index = fuelIndex;
                        break;
                    case 2:
                        index = resultIndex;
                        break;
                }
                if (index != -1)
                {
                    InventorySystem.items[index].id = item["id"] as TagNodeString;
                    InventorySystem.items[index].damage = item["Damage"] as TagNodeShort;
                    InventorySystem.items[index].count = item["Count"] as TagNodeByte;
                }
            }
        }
    }

    void ClearData()
    {
        foreach (int index in refreshIndex)
        {
            InventorySystem.items[index].id = null;
            InventorySystem.items[index].damage = 0;
            InventorySystem.items[index].count = 0;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        float burnt = (1600 - burnTime) / 1600.0f;
        fire.padding = new Vector4(0, 0, 0, burnt * 14);
        float progress = cookTime / 200.0f;
        arrow.padding = new Vector4(0, 0, 24 * (1 - progress), 0);
    }

    int[] refreshIndex = new int[] {
        resultIndex,
        oreIndex,
        fuelIndex
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