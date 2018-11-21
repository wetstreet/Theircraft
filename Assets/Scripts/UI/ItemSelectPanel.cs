using UnityEngine;
using UnityEngine.UI;
using Theircraft;

public class ItemSelectPanel : MonoBehaviour
{
    public static int curIndex;
    public static BlockType curBlockType { get { return dataList[curIndex]; } }

    static Item[] itemList = new Item[9];
    static BlockType[] dataList = new BlockType[9];

    struct Item
    {
        public Image icon;
        public GameObject select;
    }

    public static void Show()
    {
        UISystem.InstantiateUI("ItemSelectPanel");
    }

    public static void SetSlotItem(int slotID, BlockType blockType)
    {
        dataList[slotID] = blockType;
        string iconPath = BlockTexture.type2icon[blockType];
        itemList[slotID].icon.sprite = Resources.Load<Sprite>(iconPath);
        itemList[slotID].icon.gameObject.SetActive(true);
    }

    void Init()
    {
        Transform grid = transform.Find("grid");
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

    void RefreshUI()
    {
        for (int i = 0; i < 9; i++)
        {
            itemList[i].select.SetActive(i == curIndex);
        }
    }

    // Use this for initialization
    void Start () {
        Init();

        SetSlotItem(0, BlockType.Grass);
        SetSlotItem(1, BlockType.Tnt);
        SetSlotItem(2, BlockType.Brick);
        SetSlotItem(3, BlockType.Furnace);
        SetSlotItem(4, BlockType.HayBlock);

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

        RefreshUI();
    }
}
