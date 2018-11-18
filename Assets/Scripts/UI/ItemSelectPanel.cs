using UnityEngine;
using UnityEngine.UI;

public class ItemSelectPanel : MonoBehaviour
{
    public static int curIndex;
    public static BlockType curBlockType;

    Item[] itemList = new Item[9];
    BlockType[] dataList = new BlockType[9];

    struct Item
    {
        public Image icon;
        public GameObject select;
    }

    public static void Show()
    {
        UISystem.InstantiateUI("ItemSelectPanel");
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
        
        dataList[0] = BlockType.Grass;
        itemList[0].icon.sprite = Resources.Load<Sprite>("grass");
        itemList[0].icon.gameObject.SetActive(true);
        dataList[1] = BlockType.Tnt;
        itemList[1].icon.sprite = Resources.Load<Sprite>("tnt");
        itemList[1].icon.gameObject.SetActive(true);
        for (int i = 2; i < 9; i++)
        {
            dataList[i] = BlockType.None;
        }

        curIndex = 0;
        curBlockType = dataList[0];
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
        RefreshUI();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            curIndex = 0;
            curBlockType = dataList[0];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            curIndex = 1;
            curBlockType = dataList[1];
        }
        RefreshUI();
    }
}
