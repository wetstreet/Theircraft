using UnityEngine;
using UnityEngine.UI;
using protocol.cs_theircraft;
using System.Collections.Generic;

public class ItemSelectPanel : MonoBehaviour
{
    public static uint curIndex;
    public static CSBlockType curBlockType { get { return dataList[curIndex]; } }

    static Item[] itemList = new Item[9];
    public static CSBlockType[] dataList = new CSBlockType[9]
    {
        CSBlockType.GrassBlock,
        CSBlockType.Brick,
        CSBlockType.BrickStairs,
        CSBlockType.BrickWall,
        CSBlockType.Furnace,
        CSBlockType.HayBlock,
        CSBlockType.None,
        CSBlockType.None,
        CSBlockType.None
    };

    struct Item
    {
        public Image icon;
        public GameObject select;
    }

    public static ItemSelectPanel instance;
    public static void Show()
    {
        instance = UISystem.InstantiateUI("ItemSelectPanel").GetComponent<ItemSelectPanel>();
    }

    public static Dictionary<CSBlockType, string> type2icon = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.GrassBlock, "grass" },
        {CSBlockType.Dirt, "dirt" },
        {CSBlockType.BrickStairs, "Brick_Stairs" },
        {CSBlockType.Brick, "brick" },
        {CSBlockType.Furnace, "furnace" },
        {CSBlockType.HayBlock, "hayblock" },
        {CSBlockType.Stone, "stone" },
        {CSBlockType.Torch, "torch" },
        {CSBlockType.BrickWall, "Brick_Wall" },
    };

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
    }

    uint lastIndex = 0;
    public void RefreshUI()
    {
        for (int i = 0; i < 9; i++)
        {
            CSBlockType blockType = dataList[i];
            if (blockType == CSBlockType.None)
            {
                itemList[i].icon.gameObject.SetActive(false);
            }
            else
            {
                string iconPath = type2icon[blockType];
                itemList[i].icon.sprite = Resources.Load<Sprite>("GUI/CubeBlock/" + iconPath);
                itemList[i].icon.gameObject.SetActive(true);
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
        NetworkManager.SendPkgToServer(protocol.cs_enum.ENUM_CMD.CS_HERO_CHANGE_SELECT_INDEX_REQ, req);
    }

    // Use this for initialization
    void Start () {
        Init();

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
}
