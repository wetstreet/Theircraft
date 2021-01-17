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
    int holdItemCount;

    struct SlotItem
    {
        public Image icon;
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
        }
        UpdateDesc();
    }

    void HandleMouseOperation()
    {
        //PointerEventData ped = new PointerEventData(null) { position = Input.mousePosition };
        //List<RaycastResult> results = new List<RaycastResult>();
        //gr.Raycast(ped, results);
        //showDesc = false;
        //showSelectDesc = false;
        //foreach (RaycastResult result in results)
        //{
        //    if (result.gameObject.transform.parent == grid.transform)
        //    {
        //        int curIndex = int.Parse(result.gameObject.name) + step * 9;
        //        if (curIndex < blocks.Length)
        //        {
        //            showDesc = true;
        //            if (showIndex != curIndex && !holdItem)
        //            {
        //                showIndex = curIndex;
        //            }
        //            if (Input.GetKeyDown(KeyCode.Mouse0))
        //            {
        //                OnItemClick(curIndex);
        //            }
        //        }
        //        break;
        //    }
        //    else if (result.gameObject.transform.parent == selectPanel)
        //    {
        //        showSelectDesc = true;
        //        int curIndex = int.Parse(result.gameObject.name);
        //        if (showSelectIndex != curIndex)
        //        {
        //            showSelectIndex = curIndex;
        //        }
        //        if (Input.GetKeyDown(KeyCode.Mouse0))
        //        {
        //            OnClickSelectItem(curIndex);
        //        }
        //        break;
        //    }
        //    else if (result.gameObject == mask)
        //    {
        //        if (Input.GetKeyDown(KeyCode.Mouse0))
        //        {
        //            OnClickMask();
        //        }
        //    }
        //}
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
        if ((showDesc || (showSelectDesc && ItemSelectPanel.dataList[showSelectIndex] != 0)) && !holdItem && !holdSelectItem)
        {
            if (!descTrans.gameObject.activeSelf)
            {
                descTrans.gameObject.SetActive(true);
            }
            descTrans.anchoredPosition = Input.mousePosition / UISystem.scale + offset;

            string name = "";
            //if (showDesc)
            //{
            //    CSBlockType type = blocks[showIndex];
            //    name = LocalizationManager.GetBlockName(type);
            //}
            //else if (showSelectDesc)
            //{
            //    CSBlockType type = ItemSelectPanel.dataList[showSelectIndex];
            //    name = LocalizationManager.GetBlockName(type);
            //}
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
