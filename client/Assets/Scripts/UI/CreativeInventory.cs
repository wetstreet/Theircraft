using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreativeInventory : MonoBehaviour
{
    CSBlockType[] blocks = new CSBlockType[]
    {
        CSBlockType.Stone,
        CSBlockType.GrassBlock,
        CSBlockType.Dirt,
        CSBlockType.Cobblestone,
        CSBlockType.OakPlanks,
        CSBlockType.SpruceWoodPlanks,
        CSBlockType.BirchWoodPlanks,
        CSBlockType.JungleWoodPlanks,
        CSBlockType.AcaciaWoodPlanks,
        CSBlockType.DarkOakWoodPlanks,
        CSBlockType.OakSapling,
        CSBlockType.SpruceSapling,
        CSBlockType.BirchSapling,
        CSBlockType.JungleSapling,
        CSBlockType.AcaciaSapling,
        CSBlockType.DarkOakSapling,
        CSBlockType.BedRock,
        CSBlockType.Sand,
        CSBlockType.RedSand,
        CSBlockType.Gravel,
        CSBlockType.GoldOre,
        CSBlockType.IronOre,
        CSBlockType.CoalOre,
        CSBlockType.OakLog,
        CSBlockType.SpruceLog,
        CSBlockType.BirchLog,
        CSBlockType.JungleLog,
        CSBlockType.AcaciaLog,
        CSBlockType.DarkOakLog,
        CSBlockType.Brick,
        CSBlockType.BrickStairs,
        CSBlockType.BrickWall,
        CSBlockType.StoneBricks,
        CSBlockType.Furnace,
        CSBlockType.HayBlock,
        CSBlockType.Torch,
        CSBlockType.Cobweb,
        CSBlockType.Poppy,
        CSBlockType.Dandelion,
        CSBlockType.Grass,
        CSBlockType.Tnt,
        CSBlockType.OakLeaves,
        CSBlockType.SpruceLeaves,
        CSBlockType.BirchLeaves,
        CSBlockType.JungleLeaves,
        CSBlockType.AcaciaLeaves,
        CSBlockType.DarkOakLeaves,
        CSBlockType.DiamondOre,
        CSBlockType.EmeraldOre,
        CSBlockType.RedstoneOre,
        CSBlockType.CoalBlock,
        CSBlockType.IronBlock,
        CSBlockType.GoldBlock,
        CSBlockType.DiamondBlock,
        CSBlockType.EmeraldBlock,
        CSBlockType.RedstoneBlock,
        CSBlockType.OakWoodStairs,
        CSBlockType.CobblestoneStairs,
        CSBlockType.StoneBrickStairs,
        CSBlockType.NetherBrickStairs,
        CSBlockType.SpruceWoodStairs,
        CSBlockType.BirchWoodStairs,
        CSBlockType.JungleWoodStairs,
        CSBlockType.QuartzStairs,
        CSBlockType.CobblestoneWall,
        CSBlockType.Bookshelf,
        CSBlockType.MossyCobblestone,
        CSBlockType.MossyCobblestoneWall,
        CSBlockType.MossyStoneBricks,
        CSBlockType.MossyStoneBrickStairs,
        CSBlockType.MossyStoneBrickWall,
        CSBlockType.OakSlab,
        CSBlockType.SpruceSlab,
        CSBlockType.BirchSlab,
        CSBlockType.JungleSlab,
        CSBlockType.AcaciaSlab,
        CSBlockType.DarkOakSlab,
        CSBlockType.StoneSlab,
        CSBlockType.SmoothStoneSlab,
        CSBlockType.CobblestoneSlab,
        CSBlockType.MossyCobblestoneSlab,
        CSBlockType.StoneBrickSlab,
        CSBlockType.BrickSlab,
        CSBlockType.NetherBrickSlab,
        CSBlockType.QuartzSlab,
        CSBlockType.Glass,
        //CSBlockType.Ice,
        CSBlockType.PackedIce,
        //CSBlockType.Chest,
        CSBlockType.VerticalBrickSlab,
    };

    Transform grid;
    Transform unit;
    RectTransform descTrans;
    TextMeshProUGUI descLabel;
    bool holdItem = false;
    bool holdSelectItem = false;
    Image holdItemImage;
    bool showDesc;
    bool showSelectDesc;
    int showIndex;
    int showSelectIndex;
    CSBlockType holdItemType;
    int holdItemCount;
    Slider slider;

    struct SlotItem
    {
        public Image icon;
        public GameObject select;
        public TextMeshProUGUI count;
    }

    List<SlotItem> itemList = new List<SlotItem>();
    SlotItem[] selectItems = new SlotItem[9];

    static CreativeInventory Instance;

    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
            Instance.step = 0;
            Instance.slider.value = 0;
            Instance.RefreshUI();
            Instance.RefreshSelectPanel();
        }
        else
        {
            Instance = UISystem.InstantiateUI("CreativeInventory").GetComponent<CreativeInventory>();
        }

        InputManager.enabled = false;
        PlayerController.LockCursor(false);
    }

    void OnClickClose()
    {
        Hide();
    }

    public static void Hide()
    {
        InputManager.enabled = true;
        PlayerController.LockCursor(true);
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
    }

    public static void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Show();
        }
    }

    void HandleInputUpdate()
    {
        float mouseMove = Input.GetAxis("Mouse ScrollWheel");
        if (mouseMove != 0)
        {
            int sign = (int)Mathf.Sign(mouseMove);
            step = Mathf.Clamp(step - sign, 0, steps - 1);

            RefreshUI();
            slider.value = step / (steps - 1f);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            Hide();
        }
    }

    private void OnEnable()
    {
        showDesc = false;
        showSelectDesc = false;
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
            RefreshUI();
        }
    }

    void InitGrid()
    {
        for (int i = 0; i < 45; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.parent = grid;
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);

            SlotItem item = new SlotItem();
            item.icon = trans.GetComponent<Image>();
            itemList.Add(item);
        }
    }

    GraphicRaycaster gr;
    GameObject mask;
    // Start is called before the first frame update
    void Start()
    {
        mask = transform.Find("mask").gameObject;
        gr = gameObject.AddComponent<GraphicRaycaster>();
        descTrans = transform.Find("desc").GetComponent<RectTransform>();
        descLabel = descTrans.Find("text").GetComponent<TextMeshProUGUI>();

        slider = transform.Find("Slider").GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);

        step = 0;
        steps = 1 + Mathf.CeilToInt((blocks.Length - 45) / 9f);
        interval = 1f / steps;

        grid = transform.Find("BagGrid");
        unit = grid.Find("unit");
        holdItemImage = transform.Find("holdItem").GetComponent<Image>();
        unit.gameObject.SetActive(false);
        
        InitGrid();
        RefreshUI();
        InitSelectPanel();
        RefreshSelectPanel();
    }

    Transform selectPanel;
    void InitSelectPanel()
    {
        selectPanel = transform.Find("SelectGrid");
        Transform unit = selectPanel.Find("unit");
        unit.gameObject.SetActive(false);
        for (int i = 0; i < 9; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.gameObject.SetActive(true);
            trans.parent = selectPanel;
            trans.localScale = Vector3.one;

            SlotItem item = new SlotItem();
            item.icon = trans.GetComponent<Image>();
            item.count = trans.GetComponentInChildren<TextMeshProUGUI>();
            selectItems[i] = item;
        }
    }

    void HandleMouseOperation()
    {
        PointerEventData ped = new PointerEventData(null) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        showDesc = false;
        showSelectDesc = false;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.transform.parent == grid.transform)
            {
                int curIndex = int.Parse(result.gameObject.name) + step * 9;
                if (curIndex < blocks.Length)
                {
                    showDesc = true;
                    if (showIndex != curIndex && !holdItem)
                    {
                        showIndex = curIndex;
                    }
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        OnItemClick(curIndex);
                    }
                }
                break;
            }
            else if (result.gameObject.transform.parent == selectPanel)
            {
                showSelectDesc = true;
                int curIndex = int.Parse(result.gameObject.name);
                if (showSelectIndex != curIndex)
                {
                    showSelectIndex = curIndex;
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnClickSelectItem(curIndex);
                }
                break;
            }
            else if (result.gameObject == mask)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnClickMask();
                }
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        HandleMouseOperation();
        HandleInputUpdate();
        UpdateDesc();

        if (holdItem || holdSelectItem)
        {
            holdItemImage.rectTransform.anchoredPosition = Input.mousePosition / UISystem.scale;
        }
    }

    static Vector3 offset = new Vector3(8, 0, 0);
    void UpdateDesc()
    {
        if ((showDesc || (showSelectDesc && ItemSelectPanel.dataList[showSelectIndex] != CSBlockType.None)) && !holdItem && !holdSelectItem)
        {
            if (!descTrans.gameObject.activeSelf)
            {
                descTrans.gameObject.SetActive(true);
            }
            descTrans.anchoredPosition = Input.mousePosition / UISystem.scale + offset;

            string name = "";
            if (showDesc)
            {
                name = blocks[showIndex].ToString();
            }
            else if (showSelectDesc)
            {
                name = ItemSelectPanel.dataList[showSelectIndex].ToString();
            }
            descLabel.text = name;
            descTrans.sizeDelta = new Vector2(Mathf.CeilToInt(descLabel.renderedWidth) + 10, 16);
        }
        else
        {
            if (descTrans.gameObject.activeSelf)
            {
                descTrans.gameObject.SetActive(false);
            }
        }
    }

    void RefreshSelectPanel()
    {
        for (int i = 0; i < 9; i++)
        {
            CSBlockType type = ItemSelectPanel.dataList[i];
            if (type == CSBlockType.None)
            {
                selectItems[i].icon.color = Color.clear;
                selectItems[i].count.gameObject.SetActive(false);
            }
            else
            {
                selectItems[i].icon.sprite = BlockIconHelper.GetIcon(type);
                selectItems[i].icon.color = Color.white;
                if (ItemSelectPanel.countList[i] > 1)
                {
                    selectItems[i].count.gameObject.SetActive(true);
                    selectItems[i].count.text = ItemSelectPanel.countList[i].ToString();
                }
                else
                {
                    selectItems[i].count.gameObject.SetActive(false);
                }
            }
        }
    }

    void RefreshUI()
    {
        for (int i = 0; i < 45; i++)
        {
            int realIndex = i + step * 9;
            if (realIndex < blocks.Length)
            {
                itemList[i].icon.enabled = true;
                itemList[i].icon.sprite = BlockIconHelper.GetIcon(blocks[realIndex]);
            }
            else
            {
                itemList[i].icon.enabled = false;
            }
        }
        UpdateDesc();
    }

    void OnItemClick(int index)
    {
        //Debug.Log("click " + index);
        if (holdItem)
        {
            holdItem = false;
            holdItemImage.gameObject.SetActive(false);
        }
        else if (holdSelectItem)
        {
            holdSelectItem = false;
            holdItemImage.gameObject.SetActive(false);
        }
        else
        {
            holdItem = true;
            holdItemType = blocks[index];
            holdItemImage.sprite = BlockIconHelper.GetIcon(holdItemType);
            holdItemImage.gameObject.SetActive(true);
        }
    }

    void OnClickMask()
    {
        //Debug.Log("onclickmask");
        if (holdItem)
        {
            holdItem = false;
            Item.CreatePlayerDropItem(holdItemType, 1);
            holdItemImage.gameObject.SetActive(false);
        }
        else if (holdSelectItem)
        {
            holdSelectItem = false;
            Item.CreatePlayerDropItem(holdItemType, holdItemCount);
            holdItemImage.gameObject.SetActive(false);
        }
    }

    void OnClickSelectItem(int index)
    {
        //Debug.Log("OnClickSelectItem,index=" + index);
        if (holdItem)
        {
            holdItem = false;
            holdItemImage.gameObject.SetActive(false);

            ItemSelectPanel.SetSlotItem((uint)index, blocks[showIndex]);
            RefreshSelectPanel();
        }
        else if (holdSelectItem)
        {
            holdSelectItem = false;
            holdItemImage.gameObject.SetActive(false);

            ItemSelectPanel.SetSlotItem((uint)index, holdItemType, holdItemCount);
            RefreshSelectPanel();
        }
        else if (ItemSelectPanel.dataList[index] != CSBlockType.None)
        {
            holdSelectItem = true;
            holdItemType = ItemSelectPanel.dataList[index];
            holdItemCount = ItemSelectPanel.countList[index];
            holdItemImage.sprite = selectItems[showSelectIndex].icon.sprite;
            holdItemImage.gameObject.SetActive(true);
            
            ItemSelectPanel.SetSlotItem((uint)index, CSBlockType.None);
            RefreshSelectPanel();
        }
    }
}