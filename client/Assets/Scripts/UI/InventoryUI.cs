using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    protected Transform grid;
    protected Transform selectGrid;
    protected Transform unit;
    protected RectTransform descTrans;
    protected TextMeshProUGUI descLabel;

    protected RawImage holdItemImage;
    protected TextMeshProUGUI holdItemCount;

    protected struct SlotItem
    {
        public RawImage icon;
        public RawImage highlight;
        public GameObject select;
        public TextMeshProUGUI count;
    }

    // 0-35 is bag(36)
    // 36-44 is crafting(9)
    // ----------
    // |36 37 38|
    // |39 40 41|
    // |42 43 44|
    // ----------
    // 45 is craft result(1)
    // 46-72 is small chest(27)
    // 46-99 is big chest(54)
    protected SlotItem[] items = new SlotItem[99];

    protected static int resultIndex = 45;

    protected virtual void InitGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            Transform trans = Instantiate(unit);
            trans.name = i.ToString();
            trans.SetParent(selectGrid, false);
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
            trans.SetParent(grid, false);
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);

            items[i].highlight = trans.Find("highlight").GetComponent<RawImage>();
            items[i].icon = trans.GetComponent<RawImage>();
            items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        }
    }

    protected virtual void InitComponents()
    {
        mask = transform.Find("mask").gameObject;
        gr = gameObject.AddComponent<GraphicRaycaster>();
        descTrans = transform.Find("desc").GetComponent<RectTransform>();
        descLabel = descTrans.Find("text").GetComponent<TextMeshProUGUI>();

        grid = transform.Find("BagGrid");
        selectGrid = transform.Find("SelectGrid");
        unit = transform.Find("unit");
        unit.gameObject.SetActive(false);
        holdItemImage = transform.Find("holdItem").GetComponent<RawImage>();
        holdItemCount = transform.Find("holdItem/text").GetComponent<TextMeshProUGUI>();
    }

    GraphicRaycaster gr;
    GameObject mask;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitComponents();
        InitGrid();
        RefreshUI();
        RefreshGrabItem();
    }

    protected Color highlightColor = new Color(1, 1, 1, 0.2f);
    protected virtual void RefreshUI()
    {
        for (int i = 0; i < 36; i++)
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

    protected int lastHighlightIndex = -1;
    protected int highlightIndex = -1;
    protected bool inBG = false;
    protected virtual void HandleMouseOperation()
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
            inBG = results.Count > 0 && results[0].gameObject.name == "bg";
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

    protected virtual void OnLeftMouseClick()
    {
        if (highlightIndex == -1)
        {
            if (!inBG && InventorySystem.grabItem.id != null)
            {
                InventorySystem.DropGrabItem();
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        if (highlightIndex >= 0 && highlightIndex < 36)
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

    protected virtual void OnRightMouseClick()
    {
        if (highlightIndex == -1)
        {
            if (!inBG && InventorySystem.grabItem.id != null)
            {
                InventorySystem.DropGrabItem();
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
        if (highlightIndex >= 0 && highlightIndex < 36)
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
            else if (InventorySystem.items[highlightIndex].id != null)
            {
                InventorySystem.SplitHalf(highlightIndex);
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
    }

    protected virtual void RefreshGrabItem()
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
    protected virtual void Update()
    {
        HandleMouseOperation();
        UpdateDesc();

        holdItemImage.rectTransform.anchoredPosition = Input.mousePosition / UISystem.scale;
    }

    protected static Vector3 offset = new Vector3(8, 0, 0);
    protected virtual void UpdateDesc()
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
