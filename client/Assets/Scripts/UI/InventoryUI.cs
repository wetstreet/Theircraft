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
    public GameObject holdItemBlock;

    protected struct SlotItem
    {
        public RawImage icon;
        public RawImage highlight;
        public GameObject select;
        public TextMeshProUGUI count;
        public GameObject blockObj;
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
    // 100-101 is furnace ore and fuel
    // 102-146 is creative inventory(45)
    protected SlotItem[] items = new SlotItem[147];

    protected static int resultIndex = 45;
    protected static int oreIndex = 100;
    protected static int fuelIndex = 101;

    protected virtual bool checkCraft {  get { return false; } }

    protected void InitItem(int i, Transform parent)
    {
        Transform trans = Instantiate(unit);
        trans.name = i.ToString();
        trans.SetParent(parent, false);
        trans.localScale = Vector3.one;
        trans.gameObject.SetActive(true);

        items[i].highlight = trans.Find("highlight").GetComponent<RawImage>();
        items[i].icon = trans.GetComponent<RawImage>();
        items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        items[i].blockObj = trans.Find("block").gameObject;
    }

    protected void InitItem(int i, string path)
    {
        Transform trans = transform.Find(path);
        trans.name = i.ToString();
        items[i].highlight = trans.Find("highlight").GetComponent<RawImage>();
        items[i].icon = trans.GetComponent<RawImage>();
        items[i].count = trans.Find("text").GetComponent<TextMeshProUGUI>();
        items[i].blockObj = trans.Find("block").gameObject;
    }

    protected virtual void InitGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            InitItem(i, selectGrid);
        }
        for (int i = 9; i < 36; i++)
        {
            InitItem(i, grid);
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
        holdItemBlock = transform.Find("holdItem/block").gameObject;
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
    protected void RefreshItem(int i)
    {
        InventoryItem item = InventorySystem.items[i];
        if (item.id != null)
        {
            NBTObject generator = NBTGeneratorManager.GetObjectGenerator(item.id);
            bool isBlock = generator.useBlockOnUI;
            items[i].blockObj.SetActive(isBlock);
            items[i].icon.enabled = !isBlock;

            if (isBlock)
            {
                NBTBlock blockGenerator = generator as NBTBlock;
                byte data = (byte)item.damage;
                items[i].blockObj.GetComponent<MeshFilter>().sharedMesh = blockGenerator.GetItemMesh(data);
                items[i].blockObj.GetComponent<MeshRenderer>().sharedMaterial = blockGenerator.GetItemMaterial(data);
            }
            else
            {
                items[i].icon.texture = BlockIconHelper.GetIcon(item.id, item.damage);
            }

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
            items[i].blockObj.SetActive(false);
        }
        items[i].highlight.color = i == highlightIndex ? highlightColor : Color.clear;
    }

    protected virtual void RefreshUI()
    {
        for (int i = 0; i < 36; i++)
        {
            RefreshItem(i);
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
        if (Input.GetMouseButton(1))
        {
            OnRightMousePressed();
        }
        if (Input.GetMouseButtonUp(1))
        {
            travelledIndices.Clear();
        }
    }

    protected virtual bool IsIndexValid(int index)
    {
        return false;
    }

    protected virtual bool IsIDValid(int index, string id)
    {
        return true;
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
        if ((highlightIndex >= 0 && highlightIndex < 36) || IsIndexValid(highlightIndex))
        {
            if (InventorySystem.grabItem.id != null &&
                InventorySystem.items[highlightIndex].id != null &&
                InventorySystem.grabItem.id == InventorySystem.items[highlightIndex].id &&
                InventorySystem.grabItem.damage == InventorySystem.items[highlightIndex].damage)
            {
                InventorySystem.PutItems(highlightIndex, checkCraft);
            }
            else if (InventorySystem.grabItem.id == null || IsIDValid(highlightIndex, InventorySystem.grabItem.id))
            {
                InventorySystem.MouseGrabItem(highlightIndex, checkCraft);
            }
            RefreshGrabItem();
            RefreshUI();
            ItemSelectPanel.instance.RefreshUI();
        }

        if (highlightIndex == resultIndex)
        {
            if (InventorySystem.grabItem.id == null ||
                (InventorySystem.grabItem.id == InventorySystem.items[resultIndex].id &&
                InventorySystem.grabItem.damage == InventorySystem.items[resultIndex].damage))
            {
                CraftingSystem.CraftItems();
                RefreshGrabItem();
                RefreshUI();
            }
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
        if ((highlightIndex >= 0 && highlightIndex < 36) || IsIndexValid(highlightIndex))
        {
            if (InventorySystem.grabItem.id != null)
            {
                travelledIndices.Add(highlightIndex);
                lastIndex = highlightIndex;

                if (InventorySystem.items[highlightIndex].id != null)
                {
                    if (InventorySystem.grabItem.id == InventorySystem.items[highlightIndex].id &&
                        InventorySystem.grabItem.damage == InventorySystem.items[highlightIndex].damage)
                        InventorySystem.PutOneItem(highlightIndex, checkCraft);
                    else
                        InventorySystem.MouseGrabItem(highlightIndex, checkCraft);
                }
                else
                {
                    InventorySystem.PutOneItem(highlightIndex, checkCraft);
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
        if (highlightIndex == resultIndex)
        {
            if (InventorySystem.grabItem.id == null)
            {
                InventorySystem.MouseGrabItem(highlightIndex, checkCraft);
                RefreshGrabItem();
                RefreshUI();
                ItemSelectPanel.instance.RefreshUI();
            }
        }
    }

    int lastIndex = -1;
    List<int> travelledIndices = new List<int>();
    protected virtual void OnRightMousePressed()
    {
        if (InventorySystem.grabItem.id != null && highlightIndex != -1)
        {
            bool canPut = InventorySystem.items[highlightIndex].id == null || InventorySystem.items[highlightIndex].id == InventorySystem.grabItem.id;
            if (lastIndex != highlightIndex && !travelledIndices.Contains(highlightIndex) && canPut)
            {
                lastIndex = highlightIndex;
                travelledIndices.Add(highlightIndex);

                InventorySystem.PutOneItem(highlightIndex, checkCraft);
                RefreshGrabItem();
                RefreshUI();
            }
        }
    }

    protected virtual void RefreshGrabItem()
    {
        if (InventorySystem.grabItem.id != null)
        {
            NBTObject generator = NBTGeneratorManager.GetObjectGenerator(InventorySystem.grabItem.id);
            bool isBlock = generator.useBlockOnUI;
            holdItemBlock.SetActive(isBlock);
            holdItemImage.enabled = !isBlock;

            if (isBlock)
            {
                NBTBlock blockGenerator = generator as NBTBlock;
                byte data = (byte)InventorySystem.grabItem.damage;
                holdItemBlock.GetComponent<MeshFilter>().sharedMesh = blockGenerator.GetItemMesh(data);
                holdItemBlock.GetComponent<MeshRenderer>().sharedMaterial = blockGenerator.GetItemMaterial(data);
            }
            else
            {
                holdItemImage.texture = BlockIconHelper.GetIcon(InventorySystem.grabItem.id, InventorySystem.grabItem.damage);
            }

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
            holdItemBlock.SetActive(false);
        }
    }

    protected virtual void HideInternal()
    {

    }

    protected virtual void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            HideInternal();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleMouseOperation();
        UpdateDesc();

        holdItemImage.rectTransform.anchoredPosition = Input.mousePosition / UISystem.scale;

        HandleKeyboardInput();
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
