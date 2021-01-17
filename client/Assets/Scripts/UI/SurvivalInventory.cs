using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SurvivalInventory : MonoBehaviour
{

    Transform grid;
    Transform selectGrid;
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

    byte holdItemType;
    byte holdItemData;
    int holdItemCount;

    struct SlotItem
    {
        public Image icon;
        public Image highlight;
        public GameObject select;
        public TextMeshProUGUI count;
    }
    
    SlotItem[] items = new SlotItem[36];

    static SurvivalInventory Instance;

    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
            Instance.RefreshUI();
            Instance.RefreshSelectPanel();
        }
        else
        {
            Instance = UISystem.InstantiateUI("SurvivalInventory").GetComponent<SurvivalInventory>();
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

    void InitGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.parent = selectGrid;
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);
            
            items[i].highlight = trans.Find("highlight").GetComponent<Image>();
            items[i].icon = trans.GetComponent<Image>();
            items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        }
        for (int i = 9; i < 36; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.parent = grid;
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);

            items[i].highlight = trans.Find("highlight").GetComponent<Image>();
            items[i].icon = trans.GetComponent<Image>();
            items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
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
        
        grid = transform.Find("BagGrid");
        selectGrid = transform.Find("SelectGrid");
        unit = transform.Find("unit");
        unit.gameObject.SetActive(false);
        holdItemImage = transform.Find("holdItem").GetComponent<Image>();

        InitGrid();
        RefreshUI();
        RefreshSelectPanel();
    }

    Color highlightColor = new Color(1, 1, 1, 0.2f);
    void RefreshUI()
    {
        for (int i = 0; i < 36; i++)
        {
            InventoryItem item = InventorySystem.items[i];
            if (item.id != null)
            {
                items[i].icon.enabled = true;
                items[i].icon.sprite = BlockIconHelper.GetIcon(item.id, item.damage);
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
        UpdateDesc();
    }

    int lastHighlightIndex = -1;
    int highlightIndex = -1;
    void HandleMouseOperation()
    {
        PointerEventData ped = new PointerEventData(null) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        if (results.Count > 0 && int.TryParse(results[0].gameObject.transform.parent.name, out int slot))
        {
            highlightIndex = slot;
        }
        else
        {
            highlightIndex = -1;
        }
        if (lastHighlightIndex != highlightIndex)
        {
            lastHighlightIndex = highlightIndex;
            RefreshUI();
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
        if (highlightIndex != -1 && InventorySystem.items[highlightIndex].id != null && !holdItem && !holdSelectItem)
        {
            if (!descTrans.gameObject.activeSelf)
            {
                descTrans.gameObject.SetActive(true);
            }
            descTrans.anchoredPosition = Input.mousePosition / UISystem.scale + offset;

            InventoryItem item = InventorySystem.items[highlightIndex];
            NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(item.id);
            if (generator == null)
            {
                descLabel.text = item.id;
            }
            else
            {
                descLabel.text = generator.GetNameByData(item.damage);
            }
            descLabel.Rebuild(CanvasUpdate.PreRender);
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
        //for (int i = 0; i < 9; i++)
        //{
        //    CSBlockType type = ItemSelectPanel.dataList[i];
        //    if (type == CSBlockType.None)
        //    {
        //        selectItems[i].icon.color = Color.clear;
        //        selectItems[i].count.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        selectItems[i].icon.sprite = BlockIconHelper.GetIcon(type);
        //        selectItems[i].icon.color = Color.white;
        //        if (ItemSelectPanel.countList[i] > 1)
        //        {
        //            selectItems[i].count.gameObject.SetActive(true);
        //            selectItems[i].count.text = ItemSelectPanel.countList[i].ToString();
        //        }
        //        else
        //        {
        //            selectItems[i].count.gameObject.SetActive(false);
        //        }
        //    }
        //}
    }
}
