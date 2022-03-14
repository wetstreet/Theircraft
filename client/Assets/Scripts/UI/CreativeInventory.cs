using Substrate.Nbt;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreativeInventory : InventoryUI
{
    Transform creativeGrid;
    Slider slider;

    struct CreativeItem
    {
        public string id;
        public short data;
    }

    CreativeItem[] itemList = new CreativeItem[]
    {
        new CreativeItem { id = "minecraft:stone", data = 0 },
        new CreativeItem { id = "minecraft:stone", data = 1 },
        new CreativeItem { id = "minecraft:stone", data = 2 },
        new CreativeItem { id = "minecraft:stone", data = 3 },
        new CreativeItem { id = "minecraft:stone", data = 4 },
        new CreativeItem { id = "minecraft:stone", data = 5 },
        new CreativeItem { id = "minecraft:stone", data = 6 },
        new CreativeItem { id = "minecraft:grass", data = 0 },
        new CreativeItem { id = "minecraft:dirt", data = 0 },
        new CreativeItem { id = "minecraft:cobblestone", data = 0 },
        new CreativeItem { id = "minecraft:planks", data = 0 },
        new CreativeItem { id = "minecraft:planks", data = 1 },
        new CreativeItem { id = "minecraft:planks", data = 2 },
        new CreativeItem { id = "minecraft:planks", data = 3 },
        new CreativeItem { id = "minecraft:planks", data = 4 },
        new CreativeItem { id = "minecraft:planks", data = 5 },
        new CreativeItem { id = "minecraft:bedrock", data = 0 },
        new CreativeItem { id = "minecraft:sand", data = 0 },
        new CreativeItem { id = "minecraft:gravel", data = 0 },
        new CreativeItem { id = "minecraft:gold_ore", data = 0 },
        new CreativeItem { id = "minecraft:iron_ore", data = 0 },
        new CreativeItem { id = "minecraft:coal_ore", data = 0 },
        new CreativeItem { id = "minecraft:log", data = 0 },
        new CreativeItem { id = "minecraft:log", data = 1 },
        new CreativeItem { id = "minecraft:log", data = 2 },
        new CreativeItem { id = "minecraft:log", data = 3 },
        new CreativeItem { id = "minecraft:glass", data = 0 },
        new CreativeItem { id = "minecraft:lapis_ore", data = 0 },
        new CreativeItem { id = "minecraft:sandstone", data = 0 },
        new CreativeItem { id = "minecraft:wool", data = 0 },
        new CreativeItem { id = "minecraft:wool", data = 1 },
        new CreativeItem { id = "minecraft:wool", data = 2 },
        new CreativeItem { id = "minecraft:wool", data = 3 },
        new CreativeItem { id = "minecraft:wool", data = 4 },
        new CreativeItem { id = "minecraft:wool", data = 5 },
        new CreativeItem { id = "minecraft:wool", data = 6 },
        new CreativeItem { id = "minecraft:wool", data = 7 },
        new CreativeItem { id = "minecraft:wool", data = 8 },
        new CreativeItem { id = "minecraft:wool", data = 9 },
        new CreativeItem { id = "minecraft:wool", data = 10 },
        new CreativeItem { id = "minecraft:wool", data = 11 },
        new CreativeItem { id = "minecraft:wool", data = 12 },
        new CreativeItem { id = "minecraft:wool", data = 13 },
        new CreativeItem { id = "minecraft:wool", data = 14 },
        new CreativeItem { id = "minecraft:wool", data = 15 },
        new CreativeItem { id = "minecraft:stone_slab", data = 0 },
        new CreativeItem { id = "minecraft:stone_slab", data = 1 },
        new CreativeItem { id = "minecraft:stone_slab", data = 3 },
        new CreativeItem { id = "minecraft:stone_slab", data = 4 },
        new CreativeItem { id = "minecraft:stone_slab", data = 5 },
        new CreativeItem { id = "minecraft:stone_slab", data = 6 },
        new CreativeItem { id = "minecraft:stone_slab", data = 7 },
        new CreativeItem { id = "minecraft:brick_block", data = 0 },
        new CreativeItem { id = "minecraft:bookshelf", data = 0 },
        new CreativeItem { id = "minecraft:mossy_cobblestone", data = 0 },
        new CreativeItem { id = "minecraft:obsidian", data = 0 },
        new CreativeItem { id = "minecraft:oak_stairs", data = 0 },
        new CreativeItem { id = "minecraft:diamond_ore", data = 0 },
        new CreativeItem { id = "minecraft:stone_stairs", data = 0 },
        new CreativeItem { id = "minecraft:redstone_ore", data = 0 },
        new CreativeItem { id = "minecraft:pumpkin", data = 0 },
        new CreativeItem { id = "minecraft:brick_stairs", data = 0 },
        new CreativeItem { id = "minecraft:wooden_slab", data = 0 },
        new CreativeItem { id = "minecraft:wooden_slab", data = 1 },
        new CreativeItem { id = "minecraft:wooden_slab", data = 2 },
        new CreativeItem { id = "minecraft:wooden_slab", data = 3 },
        new CreativeItem { id = "minecraft:wooden_slab", data = 4 },
        new CreativeItem { id = "minecraft:wooden_slab", data = 5 },
        new CreativeItem { id = "minecraft:emerald_ore", data = 0 },
        new CreativeItem { id = "minecraft:spruce_stairs", data = 0 },
        new CreativeItem { id = "minecraft:birch_stairs", data = 0 },
        new CreativeItem { id = "minecraft:jungle_stairs", data = 0 },
    };

    static CreativeInventory Instance;

    public static void Show()
    {
        if (Instance != null)
        {
            Instance.step = 0;
            Instance.slider.value = 0;
            Instance.RefreshData();
            Instance.gameObject.SetActive(true);
            Instance.RefreshUI();
        }
        else
        {
            Instance = UISystem.InstantiateUI("CreativeInventory").GetComponent<CreativeInventory>();
            Instance.RefreshData();
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

    protected override void InitComponents()
    {
        base.InitComponents();

        creativeGrid = transform.Find("CreativeGrid");

        slider = transform.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);

        step = 0;
        steps = 1 + Mathf.CeilToInt((itemList.Length - 45) / 9f);
        interval = 1f / steps;
    }

    protected override void InitGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            InitItem(i, selectGrid);
        }
        for (int i = 102; i < 147; i++)
        {
            InitItem(i, creativeGrid);
        }
    }

    void RefreshData()
    {
        for (int i = 102; i < 147; i++)
        {
            int index = i - 102 + step * 9;
            if (index < itemList.Length)
            {
                InventorySystem.items[i].id = itemList[index].id;
                InventorySystem.items[i].damage = itemList[index].data;
                InventorySystem.items[i].count = 1;
            }
            else
            {
                InventorySystem.items[i].id = null;
                InventorySystem.items[i].damage = 0;
                InventorySystem.items[i].count = 0;
            }
        }
    }

    protected override void RefreshUI()
    {
        for (int i = 0; i < 9; i++)
        {
            RefreshItem(i);
        }
        for (int i = 102; i < 147; i++)
        {
            RefreshItem(i);
        }
    }

    protected override void HandleKeyboardInput()
    {
        float mouseMove = Input.GetAxis("Mouse ScrollWheel");
        if (mouseMove != 0)
        {
            int sign = (int)Mathf.Sign(mouseMove);
            step = Mathf.Clamp(step - sign, 0, steps - 1);

            RefreshData();
            RefreshUI();
            slider.value = step / (steps - 1f);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            Hide();
        }
    }

    int steps;
    float interval;
    int step;
    void OnValueChanged(float value)
    {
        int newStep = Mathf.Min(Mathf.FloorToInt(value / interval), steps - 1);
        if (newStep != step)
        {
            step = newStep;
            RefreshData();
            RefreshUI();
        }
    }


    protected override void OnLeftMouseClick()
    {
        base.OnLeftMouseClick();


        if (highlightIndex >= 102 && highlightIndex < 147)
        {
            if (InventorySystem.grabItem.id != null)
            {
                if (InventorySystem.grabItem.damage == InventorySystem.items[highlightIndex].damage)
                {
                    InventorySystem.grabItem.count++;
                }
                else
                {
                    InventorySystem.ClearGrabItem();
                }
            }
            else
            {
                InventorySystem.grabItem = InventorySystem.items[highlightIndex];
            }
            RefreshGrabItem();
            RefreshUI();
            ItemSelectPanel.instance.RefreshUI();
        }
    }
}