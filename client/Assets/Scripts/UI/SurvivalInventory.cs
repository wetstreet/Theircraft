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
    Transform craftGrid;
    Transform unit;
    RectTransform descTrans;
    TextMeshProUGUI descLabel;

    RawImage holdItemImage;
    TextMeshProUGUI holdItemCount;

    Transform head;
    Transform headAnchor;

    struct SlotItem
    {
        public RawImage icon;
        public RawImage highlight;
        public GameObject select;
        public TextMeshProUGUI count;
    }
    
    SlotItem[] items = new SlotItem[41];

    static SurvivalInventory Instance;

    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
            Instance.RefreshUI();
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

    void InitGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.parent = selectGrid;
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);
            
            items[i].highlight = trans.Find("highlight").GetComponent<RawImage>();
            items[i].icon = trans.GetComponent<RawImage>();
            items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        }
        for (int i = 9; i < 36; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.parent = grid;
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);

            items[i].highlight = trans.Find("highlight").GetComponent<RawImage>();
            items[i].icon = trans.GetComponent<RawImage>();
            items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        }

        for (int i = 36; i < 40; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.parent = craftGrid;
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
        craftGrid = transform.Find("CraftingGrid");
        unit = transform.Find("unit");
        unit.gameObject.SetActive(false);
        holdItemImage = transform.Find("holdItem").GetComponent<RawImage>();
        holdItemCount = transform.Find("holdItem/text").GetComponent<TextMeshProUGUI>();

        head = transform.Find("meshParent/steve/Move/Body/Head");
        headAnchor = transform.Find("meshParent/RotateHelper");

        InitGrid();
        RefreshUI();
        RefreshGrabItem();
    }

    Color highlightColor = new Color(1, 1, 1, 0.2f);
    void RefreshUI()
    {
        for (int i = 0; i < 41; i++)
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

        if (Input.GetMouseButtonDown(0))
        {
            OnLeftMouseClick();
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnRightMouseClick();
        }
    }

    void OnLeftMouseClick()
    {
        if (highlightIndex == -1)
        {
            if (InventorySystem.grabItem.id != null)
            {
                NBTObject generator = NBTGeneratorManager.GetObjectGenerator(InventorySystem.grabItem.id);
                Item.CreatePlayerDropItem(generator, (byte)InventorySystem.grabItem.damage, InventorySystem.grabItem.count);
                InventorySystem.DropGrabItem();

                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        else if (highlightIndex == 40)
        {
            if (InventorySystem.grabItem.id == null)
            {
                InventorySystem.CraftItems();
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        else
        {
            if (InventorySystem.grabItem.id != null && InventorySystem.items[highlightIndex].id != null && InventorySystem.grabItem.id == InventorySystem.items[highlightIndex].id)
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

    void OnRightMouseClick()
    {
        if (highlightIndex == -1)
        {
            if (InventorySystem.grabItem.id != null)
            {
                NBTObject generator = NBTGeneratorManager.GetObjectGenerator(InventorySystem.grabItem.id);
                Item.CreatePlayerDropItem(generator, (byte)InventorySystem.grabItem.damage, InventorySystem.grabItem.count);
                InventorySystem.DropGrabItem();

                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        else if (highlightIndex == 40)
        {
            if (InventorySystem.grabItem.id == null)
            {
                InventorySystem.MouseGrabItem(highlightIndex);
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        else
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

    void RefreshGrabItem()
    {
        if (InventorySystem.grabItem.id != null)
        {
            holdItemImage.enabled = true;
            holdItemImage.texture = BlockIconHelper.GetIcon(InventorySystem.grabItem.id, InventorySystem.grabItem.damage);

            if (InventorySystem.grabItem.count > 1)
            {
                holdItemCount.enabled = true;
                holdItemCount.text = InventorySystem.grabItem.count.ToString();
            }
            else
            {
                holdItemCount.enabled = false;
            }
        }
        else
        {
            holdItemImage.enabled = false;
            holdItemCount.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseOperation();
        HandleInputUpdate();
        UpdateDesc();

        holdItemImage.rectTransform.anchoredPosition = Input.mousePosition / UISystem.scale;

        //headAnchor.LookAt(Input.mousePosition);
        //Vector3 headPos = UISystem.camera.WorldToScreenPoint(head.position);
        //Vector3 dir = Input.mousePosition - headPos;
        //Debug.Log("dir=" + dir);
    }

    static Vector3 offset = new Vector3(8, 0, 0);
    void UpdateDesc()
    {
        if (highlightIndex != -1 && InventorySystem.grabItem.id == null)
        {
            InventoryItem item = InventorySystem.items[highlightIndex];

            if (item.id != null)
            {
                if (!descTrans.gameObject.activeSelf)
                {
                    descTrans.gameObject.SetActive(true);
                }
                descTrans.anchoredPosition = Input.mousePosition / UISystem.scale + offset;

                NBTObject generator = NBTGeneratorManager.GetObjectGenerator(item.id);
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
        else
        {
            if (descTrans.gameObject.activeSelf)
            {
                descTrans.gameObject.SetActive(false);
            }
        }
    }
}
