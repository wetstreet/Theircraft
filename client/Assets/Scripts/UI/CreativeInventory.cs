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
    bool showDesc;
    int showIndex;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        descTrans = transform.Find("desc").GetComponent<RectTransform>();
        descLabel = descTrans.Find("text").GetComponent<TextMeshProUGUI>();

        grid = transform.Find("Scroll View/Viewport/Content");
        unit = grid.Find("unit");
        unit.gameObject.SetActive(false);

        RefreshUI();
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
    }
    
    void UpdateDesc()
    {
        if (showDesc)
        {
            if (!descTrans.gameObject.activeSelf)
            {
                descTrans.gameObject.SetActive(true);
            }
            descTrans.anchoredPosition = Input.mousePosition + offset;

            string name = blocks[showIndex].ToString();
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
                OnPointerCallback onPointerCallback = trans.GetComponent<OnPointerCallback>();
                onPointerCallback.index = i;
                onPointerCallback.pointerEnterCallback = (int index) =>
                {
                    showDesc = true;
                    showIndex = index;
                };
                onPointerCallback.pointerExitCallback = (int index) =>
                {
                    showDesc = false;
                    showIndex = index;
                };

                itemList.Add(item);
            }
            itemList[i].icon.sprite = Resources.Load<Sprite>("GUI/CubeBlock/" + iconPath);
        }
    }
}
