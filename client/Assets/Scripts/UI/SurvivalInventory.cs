using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SurvivalInventory : InventoryUI
{
    Transform head;
    Transform headAnchor;

    Transform craftGrid;

    static SurvivalInventory Instance;

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
            Instance = UISystem.InstantiateUI("SurvivalInventory").GetComponent<SurvivalInventory>();
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

    protected override void InitGrid()
    {
        base.InitGrid();

        for (int i = 36; i < 40; i++)
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

        Transform resultTrans = transform.Find("CraftingResult/40");
        items[40].highlight = resultTrans.Find("highlight").GetComponent<RawImage>();
        items[40].icon = resultTrans.GetComponent<RawImage>();
        items[40].count = resultTrans.Find("text").GetComponent<TextMeshProUGUI>();
    }

    protected override void RefreshUI()
    {
        base.RefreshUI();

        for (int i = 36; i < 41; i++)
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

        if (highlightIndex == 40)
        {
            if (InventorySystem.grabItem.id == null)
            {
                InventorySystem.CraftItems();
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        if (highlightIndex >= 36 && highlightIndex < 40)
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

        if (highlightIndex == 40)
        {
            if (InventorySystem.grabItem.id == null)
            {
                InventorySystem.MouseGrabItem(highlightIndex);
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        if (highlightIndex >= 36 && highlightIndex < 40)
        {
            if (InventorySystem.grabItem.id != null)
            {
                if (InventorySystem.items[highlightIndex].id != null)
                {
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
            else
            {
                if (InventorySystem.items[highlightIndex].id != null)
                {
                    InventorySystem.SplitHalf(highlightIndex);
                    RefreshGrabItem();
                    RefreshUI();
                    ItemSelectPanel.instance.RefreshUI();
                }
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        HandleInputUpdate();

        //headAnchor.LookAt(Input.mousePosition);
        //Vector3 headPos = UISystem.camera.WorldToScreenPoint(head.position);
        //Vector3 dir = Input.mousePosition - headPos;
        //Debug.Log("dir=" + dir);
    }
}
