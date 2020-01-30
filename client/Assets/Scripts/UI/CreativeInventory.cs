using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreativeInventory : MonoBehaviour
{
    CSBlockType[] blocks = new CSBlockType[8]
    {
        CSBlockType.Stone,
        CSBlockType.GrassBlock,
        CSBlockType.Brick,
        CSBlockType.BrickStairs,
        CSBlockType.BrickWall,
        CSBlockType.Furnace,
        CSBlockType.HayBlock,
        CSBlockType.Torch
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
    CSBlockType showSelectType;

    Item[] selectItems = new Item[9];

    static CreativeInventory Instance;
    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
            Instance.RefreshUI();
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
            if (Instance != null && Instance.gameObject.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    private void OnEnable()
    {
        showDesc = false;
        showSelectDesc = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        descTrans = transform.Find("desc").GetComponent<RectTransform>();
        descLabel = descTrans.Find("text").GetComponent<TextMeshProUGUI>();
        transform.Find("mask").GetComponent<OnPointerCallback>().pointerDownCallback = OnClickMask;

        grid = transform.Find("Scroll View/Viewport/Content");
        unit = grid.Find("unit");
        holdItemImage = transform.Find("holdItem").GetComponent<Image>();
        unit.gameObject.SetActive(false);

        RefreshUI();
        InitSelectPanel();
        RefreshSelectPanel();
    }

    void InitSelectPanel()
    {
        Transform selectPanel = transform.Find("selectPanel");
        for (int i = 0; i < 9; i++)
        {
            Item item = new Item();
            Transform trans = selectPanel.GetChild(i);
            item.icon = trans.GetComponent<Image>();
            OnPointerCallback callbacks = trans.GetComponent<OnPointerCallback>();
            callbacks.index = i;
            callbacks.pointerEnterCallback = (int index) =>
            {
                showSelectDesc = true;
                showSelectIndex = index;
            };
            callbacks.pointerExitCallback = (int index) =>
            {
                showSelectDesc = false;
            };
            callbacks.pointerDownCallback = (int index) =>
            {
                OnClickSelectItem(index);
            };
            selectItems[i] = item;
        }
    }

    // Update is called once per frame
    static Vector3 offset = new Vector3(16, 0, 0);
    void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
        UpdateDesc();

        if (holdItem || holdSelectItem)
        {
            holdItemImage.rectTransform.anchoredPosition = Input.mousePosition;
        }
    }
    
    void UpdateDesc()
    {
        if ((showDesc || (showSelectDesc && ItemSelectPanel.dataList[showSelectIndex] != CSBlockType.None)) && !holdItem && !holdSelectItem)
        {
            if (!descTrans.gameObject.activeSelf)
            {
                descTrans.gameObject.SetActive(true);
            }
            descTrans.anchoredPosition = Input.mousePosition + offset;

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
            descTrans.sizeDelta = new Vector2(Mathf.CeilToInt(descLabel.renderedWidth) + 16, 32);
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
            }
            else
            {
                string iconPath = ItemSelectPanel.type2icon[type];
                selectItems[i].icon.sprite = Resources.Load<Sprite>("GUI/CubeBlock/" + iconPath);
                selectItems[i].icon.color = Color.white;
            }
        }
    }

    struct Item
    {
        public Image icon;
        public GameObject select;
    }
    List<Item> itemList = new List<Item>();
    void RefreshUI()
    {
        for(int i = 0; i < blocks.Length; i++)
        {
            CSBlockType blockType = blocks[i];
            string iconPath = ItemSelectPanel.type2icon[blockType];
            if (i >= itemList.Count)
            {
                Transform trans = Instantiate(unit);
                trans.parent = grid;
                trans.localScale = Vector3.one;
                trans.gameObject.SetActive(true);

                Item item = new Item();
                item.icon = trans.GetComponent<Image>();
                OnPointerCallback callbacks = trans.GetComponent<OnPointerCallback>();
                callbacks.index = i;
                callbacks.pointerEnterCallback = (int index) =>
                {
                    showDesc = true;
                    showIndex = index;
                };
                callbacks.pointerExitCallback = (int index) =>
                {
                    showDesc = false;
                };
                callbacks.pointerDownCallback = (int index) =>
                {
                    OnItemClick(index);
                };

                itemList.Add(item);
            }
            itemList[i].icon.sprite = Resources.Load<Sprite>("GUI/CubeBlock/" + iconPath);
        }
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
            holdItemImage.sprite = itemList[showIndex].icon.sprite;
            holdItemImage.gameObject.SetActive(true);
        }
    }

    void OnClickMask(int index)
    {
        //Debug.Log("onclickmask");
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
    }

    void OnClickSelectItem(int index)
    {
        //Debug.Log("OnClickSelectItem,index=" + index);
        if (holdItem)
        {
            holdItem = false;
            holdItemImage.gameObject.SetActive(false);

            ItemSelectPanel.dataList[index] = blocks[showIndex];
            ItemSelectPanel.instance.RefreshUI();
            RefreshSelectPanel();
        }
        else if (holdSelectItem)
        {
            holdSelectItem = false;
            holdItemImage.gameObject.SetActive(false);

            ItemSelectPanel.dataList[index] = showSelectType;
            ItemSelectPanel.instance.RefreshUI();
            RefreshSelectPanel();
        }
        else if (ItemSelectPanel.dataList[index] != CSBlockType.None)
        {
            holdSelectItem = true;
            showSelectType = ItemSelectPanel.dataList[index];
            holdItemImage.sprite = selectItems[showSelectIndex].icon.sprite;
            holdItemImage.gameObject.SetActive(true);

            ItemSelectPanel.dataList[index] = CSBlockType.None;
            ItemSelectPanel.instance.RefreshUI();
            RefreshSelectPanel();
        }
    }
}
