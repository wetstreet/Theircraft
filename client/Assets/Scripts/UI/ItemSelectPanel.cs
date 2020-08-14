using UnityEngine;
using UnityEngine.UI;
using protocol.cs_theircraft;
using System.Collections.Generic;
using protocol.cs_enum;
using TMPro;

public class ItemSelectPanel : MonoBehaviour
{
    static uint curIndex;
    public static CSBlockType curBlockType { get { return dataList[curIndex]; } }

    static SlotItem[] itemList = new SlotItem[9];
    public static CSBlockType[] dataList = new CSBlockType[9];
    public static int[] countList = new int[9];

    public static void Init(uint index, List<CSItem> items)
    {
        curIndex = index;
        for (int i = 0; i < 9; i++)
        {
            dataList[i] = items[i].Type;
            countList[i] = (int)items[i].Count;
        }
    }

    public static void AddItem(CSBlockType type, int count)
    {
        uint firstEmpty = 0;
        bool hasEmpty = false;
        for (uint i = 0; i < 9; i++)
        {
            if (dataList[i] == type)
            {
                SetSlotItem(i, type, count);
                return;
            }
            else if (dataList[i] == CSBlockType.None && !hasEmpty)
            {
                hasEmpty = true;
                firstEmpty = i;
            }
        }
        if (hasEmpty)
        {
            SetSlotItem(firstEmpty, type, count);
        }
    }
    
    public static void DropCurItem(int count = 1)
    {
        if (dataList[curIndex] != CSBlockType.None)
        {
            Item.CreatePlayerDropItem(dataList[curIndex]);
            int left = countList[curIndex] - count;
            if (left <= 0)
            {
                SetSlotItem(curIndex, CSBlockType.None);
            }
            else
            {
                SetSlotItem(curIndex, dataList[curIndex], -1);
            }
        }
    }

    public static void SetSlotItem(uint index, CSBlockType type, int count = 1)
    {
        if (dataList[index] != type)
        {
            countList[index] = 0;
        }
        dataList[index] = type;
        countList[index] = type == CSBlockType.None ? 0 : countList[index] + count;
        CSHeroChangeSelectItemReq req = new CSHeroChangeSelectItemReq
        {
            Index = index,
            Item = type,
            Count = (uint)countList[index],
        };
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_HERO_CHANGE_SELECT_ITEM_REQ, req);
        instance.RefreshUI();
    }

    struct SlotItem
    {
        public Image icon;
        public GameObject select;
        public TextMeshProUGUI count;
    }

    public static ItemSelectPanel instance;
    public static void Show()
    {
        instance = UISystem.InstantiateUI("ItemSelectPanel").GetComponent<ItemSelectPanel>();
    }
    
    void Init()
    {
        Transform grid = transform.Find("container/grid");
        Transform unit = grid.Find("item");
        unit.gameObject.SetActive(false);
        for (int i = 0; i < 9; i++)
        {
            Transform trans = Instantiate(unit);
            trans.SetParent(grid, false);
            trans.localScale = Vector3.one;
            trans.gameObject.SetActive(true);
            SlotItem item = new SlotItem
            {
                icon = trans.Find("icon").GetComponent<Image>(),
                select = trans.Find("select").gameObject,
                count = trans.Find("Text").GetComponent<TextMeshProUGUI>(),
            };
            item.icon.gameObject.SetActive(false);
            item.select.SetActive(false);
            itemList[i] = item;
        }
    }

    uint lastIndex = 0;
    void RefreshUI()
    {
        for (int i = 0; i < 9; i++)
        {
            CSBlockType blockType = dataList[i];
            if (blockType == CSBlockType.None)
            {
                itemList[i].icon.gameObject.SetActive(false);
                itemList[i].count.gameObject.SetActive(false);
            }
            else
            {
                itemList[i].icon.sprite = BlockIconHelper.GetIcon(blockType);
                itemList[i].icon.gameObject.SetActive(true);
                if (countList[i] > 1)
                {
                    itemList[i].count.gameObject.SetActive(true);
                    itemList[i].count.text = countList[i].ToString();
                }
                else
                {
                    itemList[i].count.gameObject.SetActive(false);
                }
            }
            itemList[i].select.SetActive(i == curIndex);
        }

        if (lastIndex != curIndex)
        {
            lastIndex = curIndex;
            HeroChangeSelectIndexReq(curIndex);
        }
        if (curBlockType != CSBlockType.None)
        {
            PlayerController.ShowBlock(curBlockType);
        }
        else
        {
            PlayerController.ShowHand();
        }
    }

    void HeroChangeSelectIndexReq(uint index)
    {
        CSHeroChangeSelectIndexReq req = new CSHeroChangeSelectIndexReq
        {
            Index = index
        };
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_HERO_CHANGE_SELECT_INDEX_REQ, req);
    }

    // Use this for initialization
    void Start () {
        Init();

        RefreshUI();
    }

    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { curIndex = 0; }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { curIndex = 1; }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { curIndex = 2; }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { curIndex = 3; }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { curIndex = 4; }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { curIndex = 5; }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { curIndex = 6; }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) { curIndex = 7; }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) { curIndex = 8; }

        float mouseMove = Input.GetAxis("Mouse ScrollWheel");
        if (mouseMove < 0)
        {
            if (curIndex == 8)
            {
                curIndex = 0;
            }
            else
            {
                curIndex++;
            }
        }
        else if (mouseMove > 0)
        {
            if (curIndex == 0)
            {
                curIndex = 8;
            }
            else
            {
                curIndex--;
            }
        }

        if (lastIndex != curIndex)
        {
            RefreshUI();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
