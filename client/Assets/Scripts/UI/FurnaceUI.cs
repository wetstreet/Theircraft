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

    FurnaceData furnaceData;

    void UpdateData()
    {
        if (furnaceData.source != null)
        {
            InventorySystem.items[oreIndex].id = furnaceData.source.id;
            InventorySystem.items[oreIndex].damage = furnaceData.source.damage;
            InventorySystem.items[oreIndex].count = furnaceData.source.count;
        }
        else
            InventorySystem.items[oreIndex].id = null;

        if (furnaceData.fuel != null)
        {
            InventorySystem.items[fuelIndex].id = furnaceData.fuel.id;
            InventorySystem.items[fuelIndex].damage = furnaceData.fuel.damage;
            InventorySystem.items[fuelIndex].count = furnaceData.fuel.count;
        }
        else
            InventorySystem.items[fuelIndex].id = null;

        if (furnaceData.result != null)
        {
            InventorySystem.items[resultIndex].id = furnaceData.result.id;
            InventorySystem.items[resultIndex].damage = furnaceData.result.damage;
            InventorySystem.items[resultIndex].count = furnaceData.result.count;
        }
        else
            InventorySystem.items[resultIndex].id = null;
    }

    void SyncData()
    {
        if (InventorySystem.items[oreIndex].id != null)
        {
            if (furnaceData.source == null)
                furnaceData.source = new FurnaceItem();
            furnaceData.source.id = InventorySystem.items[oreIndex].id;
            furnaceData.source.damage = InventorySystem.items[oreIndex].damage;
            furnaceData.source.count = InventorySystem.items[oreIndex].count;
        }
        else
        {
            furnaceData.source = null;
            furnaceData.cookTime = 0;
        }

        if (InventorySystem.items[fuelIndex].id != null)
        {
            if (furnaceData.fuel == null)
                furnaceData.fuel = new FurnaceItem();
            furnaceData.fuel.id = InventorySystem.items[fuelIndex].id;
            furnaceData.fuel.damage = InventorySystem.items[fuelIndex].damage;
            furnaceData.fuel.count = InventorySystem.items[fuelIndex].count;
        }
        else
            furnaceData.fuel = null;

        if (InventorySystem.items[resultIndex].id != null)
        {
            if (furnaceData.result == null)
                furnaceData.result = new FurnaceItem();
            furnaceData.result.id = InventorySystem.items[resultIndex].id;
            furnaceData.result.damage = InventorySystem.items[resultIndex].damage;
            furnaceData.result.count = InventorySystem.items[resultIndex].count;
        }
        else
            furnaceData.result = null;
    }

    void InitData()
    {
        NBTChunk chunk = NBTHelper.GetChunk(pos);
        if (chunk != null && chunk.furnaceDict.ContainsKey(pos))
        {
            furnaceData = chunk.furnaceDict[pos];
            UpdateData();
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

    protected override bool IsIDValid(int index, string id)
    {
        if (index == fuelIndex)
        {
            NBTObject obj = NBTGeneratorManager.GetObjectGenerator(id);
            return obj.burningTime != -1;
        }
        return true;
    }

    protected override void OnLeftMouseClick()
    {
        base.OnLeftMouseClick();
        SyncData();
    }

    protected override void OnRightMouseClick()
    {
        base.OnRightMouseClick();
        SyncData();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        float burnt = (1600 - furnaceData.burnTime) / 1600.0f;
        fire.padding = new Vector4(0, 0, 0, burnt * 14);
        float progress = 0;
        if (furnaceData.cookTimeTotal != 0)
        {
            progress = furnaceData.cookTime / (float)furnaceData.cookTimeTotal;
        }
        //Debug.Log("burntime=" + furnaceData.burnTime + ",cooktime=" + furnaceData.cookTime);
        arrow.padding = new Vector4(0, 0, 24 * (1 - progress), 0);
    }

    public static void Refresh()
    {
        if (Instance)
        {
            Instance.UpdateData();
            Instance.RefreshUI();
        }
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