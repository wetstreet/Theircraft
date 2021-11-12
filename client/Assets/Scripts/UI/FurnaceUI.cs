using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FurnaceUI : InventoryUI
{
    static FurnaceUI Instance;

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
            Instance = UISystem.InstantiateUI("FurnaceUI").GetComponent<FurnaceUI>();
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
    }

    public static List<int> indexList = new List<int>() {
        oreIndex,
        fuelIndex
    };

    protected override void InitGrid()
    {
        base.InitGrid();

        Transform oreTrans = transform.Find("Ore");
        oreTrans.name = oreIndex.ToString();
        items[oreIndex].highlight = oreTrans.Find("highlight").GetComponent<RawImage>();
        items[oreIndex].icon = oreTrans.GetComponent<RawImage>();
        items[oreIndex].count = oreTrans.Find("text").GetComponent<TextMeshProUGUI>();

        Transform fuelTrans = transform.Find("Fuel");
        fuelTrans.name = fuelIndex.ToString();
        items[fuelIndex].highlight = fuelTrans.Find("highlight").GetComponent<RawImage>();
        items[fuelIndex].icon = fuelTrans.GetComponent<RawImage>();
        items[fuelIndex].count = fuelTrans.Find("text").GetComponent<TextMeshProUGUI>();

        Transform resultTrans = transform.Find("CraftingResult/result");
        resultTrans.name = resultIndex.ToString();
        items[resultIndex].highlight = resultTrans.Find("highlight").GetComponent<RawImage>();
        items[resultIndex].icon = resultTrans.GetComponent<RawImage>();
        items[resultIndex].count = resultTrans.Find("text").GetComponent<TextMeshProUGUI>();
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
                Debug.Log("craft,grabItem=" + InventorySystem.grabItem.id + ",result=" + InventorySystem.items[resultIndex].id);
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
            else if (InventorySystem.items[highlightIndex].id != null && InventorySystem.items[highlightIndex].count > 1)
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