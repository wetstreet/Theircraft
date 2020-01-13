using UnityEngine;
using UnityEngine.UI;
using protocol.cs_theircraft;
using System.Collections.Generic;

public class ItemSelectPanel : MonoBehaviour
{
    public static int curIndex;
    public static CSBlockType curBlockType { get { return dataList[curIndex]; } }

    static Item[] itemList = new Item[9];
    static CSBlockType[] dataList = new CSBlockType[9];

    struct Item
    {
        public Image icon;
        public GameObject select;
    }

    public static void Show()
    {
        UISystem.InstantiateUI("ItemSelectPanel");
    }

    static Dictionary<CSBlockType, string> type2icon = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.GrassBlock, "grass" },
        {CSBlockType.Dirt, "dirt" },
        {CSBlockType.Tnt, "tnt" },
        {CSBlockType.Brick, "brick" },
        {CSBlockType.Furnace, "furnace" },
        {CSBlockType.HayBlock, "hayblock" },
        {CSBlockType.Stone, "stone" },
    };

    public static void SetSlotItem(int slotID, CSBlockType blockType)
    {
        dataList[slotID] = blockType;
        string iconPath = type2icon[blockType];
        itemList[slotID].icon.sprite = Resources.Load<Sprite>("GUI/CubeBlock/" + iconPath);
        itemList[slotID].icon.gameObject.SetActive(true);
    }

    void Init()
    {
        Transform grid = transform.Find("container/grid");
        for (int i = 0; i < grid.childCount; i++)
        {
            Transform trans = grid.GetChild(i);
            Item item = new Item();
            item.icon = trans.Find("icon").GetComponent<Image>();
            item.select = trans.Find("select").gameObject;
            item.icon.gameObject.SetActive(false);
            item.select.SetActive(false);
            itemList[i] = item;
        }

        curIndex = 0;
    }

    int lastIndex = 0;
    void RefreshUI()
    {
        for (int i = 0; i < 9; i++)
        {
            itemList[i].select.SetActive(i == curIndex);
        }
        if (lastIndex != curIndex)
        {
            lastIndex = curIndex;
            if (curBlockType != CSBlockType.None)
            {
                PlayerController.ShowBlock(curBlockType);
            }
            else
                PlayerController.ShowHand();
        }
    }

    // Use this for initialization
    void Start () {
        Init();

        SetSlotItem(0, CSBlockType.GrassBlock);
        SetSlotItem(1, CSBlockType.Tnt);
        SetSlotItem(2, CSBlockType.Brick);
        SetSlotItem(3, CSBlockType.Furnace);
        SetSlotItem(4, CSBlockType.HayBlock);
        SetSlotItem(5, CSBlockType.Stone);

        RefreshUI();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1)) { curIndex = 0; }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { curIndex = 1; }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { curIndex = 2; }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { curIndex = 3; }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { curIndex = 4; }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { curIndex = 5; }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { curIndex = 6; }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) { curIndex = 7; }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) { curIndex = 8; }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            curIndex++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            curIndex--;
        }
        if (curIndex == 9)
        {
            curIndex = 0;
        }
        if (curIndex == -1)
        {
            curIndex = 8;
        }

        RefreshUI();
    }
}
