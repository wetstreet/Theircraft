using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ItemSelectPanel : MonoBehaviour
{
    struct SlotItem
    {
        public RawImage icon;
        public GameObject select;
        public TextMeshProUGUI count;
        public RawImage damage;
        public GameObject damageObj;
        public GameObject blockObj;
    }

    public static uint curIndex;

    private SlotItem[] itemList = new SlotItem[9];
    public static int[] countList = new int[9];

    // health and food level
    struct SlotUnit
    {
        public Image full;
        public Image half;
    }

    SlotUnit[] heartList = new SlotUnit[10];
    SlotUnit[] meatList = new SlotUnit[10];

    TextMeshProUGUI level;
    Image exp;
    Transform survival;

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
                icon = trans.Find("icon").GetComponent<RawImage>(),
                select = trans.Find("select").gameObject,
                count = trans.Find("Text").GetComponent<TextMeshProUGUI>(),
                damageObj = trans.Find("damage_bg").gameObject,
                damage = trans.Find("damage_bg/damage").GetComponent<RawImage>(),
                blockObj = trans.Find("block").gameObject
            };
            item.icon.gameObject.SetActive(false);
            item.select.SetActive(false);
            item.damageObj.SetActive(false);
            itemList[i] = item;
        }

        survival = transform.Find("container/survival");

        Transform heartGrid = survival.Find("health_grid");
        Transform heartUnit = heartGrid.Find("heart_bg_unit");
        for (int i = 0; i < 10; i++)
        {
            Transform heartTrans = Instantiate(heartUnit);
            heartTrans.SetParent(heartGrid, false);
            heartTrans.localScale = Vector3.one;
            SlotUnit heart = new SlotUnit
            {
                full = heartTrans.Find("heart").GetComponent<Image>(),
                half = heartTrans.Find("heart_half").GetComponent<Image>(),
            };
            heartList[i] = heart;
        }
        heartUnit.gameObject.SetActive(false);

        Transform meatGrid = survival.Find("meat_grid");
        Transform meatUnit = meatGrid.Find("meat_bg_unit");
        for (int i = 0; i < 10; i++)
        {
            Transform meatTrans = Instantiate(meatUnit);
            meatTrans.SetParent(meatGrid, false);
            meatTrans.localScale = Vector3.one;
            SlotUnit meat = new SlotUnit
            {
                full = meatTrans.Find("meat").GetComponent<Image>(),
                half = meatTrans.Find("meat_half").GetComponent<Image>(),
            };
            meatList[i] = meat;
        }
        meatUnit.gameObject.SetActive(false);

        level = survival.Find("level").GetComponent<TextMeshProUGUI>();
        exp = survival.Find("exp_bg/exp").GetComponent<Image>();
    }

    public void RefreshStatus()
    {
        if (GameModeManager.isCreative)
        {
            survival.gameObject.SetActive(false);
            return;
        }
        else
        {
            survival.gameObject.SetActive(true);
        }

        float heart = Mathf.Clamp(PlayerController.instance.Health, 0, 20) / 2.0f;
        for (int i = 0; i < 10; i++)
        {
            heartList[i].full.gameObject.SetActive(false);
            heartList[i].half.gameObject.SetActive(false);
        }
        for (int i = 0; i < (int)heart; i++)
        {
            heartList[i].full.gameObject.SetActive(true);
        }
        float fraction = heart - (int)heart;
        if (fraction >= 0.5f)
        {
            heartList[(int)heart].half.gameObject.SetActive(true);
        }

        float meat = Mathf.Clamp(PlayerController.instance.foodLevel, 0, 20) / 2.0f;
        for (int i = 0; i < 10; i++)
        {
            meatList[i].full.gameObject.SetActive(false);
            meatList[i].half.gameObject.SetActive(false);
        }
        for (int i = 0; i < (int)meat; i++)
        {
            meatList[i].full.gameObject.SetActive(true);
        }
        float meatFraction = meat - (int)meat;
        if (meatFraction >= 0.5f)
        {
            meatList[(int)meat].half.gameObject.SetActive(true);
        }
    }

    uint lastIndex = 0;
    public void RefreshUI()
    {
        for (int i = 0; i < 9; i++)
        {
            InventoryItem item = InventorySystem.items[i];
            if (item.id == null)
            {
                itemList[i].icon.gameObject.SetActive(false);
                itemList[i].count.gameObject.SetActive(false);
                itemList[i].damageObj.SetActive(false);
                itemList[i].blockObj.SetActive(false);
            }
            else
            {

                NBTObject generator = NBTGeneratorManager.GetObjectGenerator(item.id);
                bool isBlock = generator.useBlockOnUI;
                itemList[i].blockObj.SetActive(isBlock);
                itemList[i].icon.gameObject.SetActive(!isBlock);
                if (isBlock)
                {
                    NBTBlock blockGenerator = generator as NBTBlock;
                    byte data = (byte)item.damage;
                    itemList[i].blockObj.GetComponent<MeshFilter>().sharedMesh = blockGenerator.GetItemMesh(data);
                    itemList[i].blockObj.GetComponent<MeshRenderer>().sharedMaterial = blockGenerator.GetItemMaterial(data);
                    itemList[i].damageObj.SetActive(false);
                }
                else
                {
                    itemList[i].icon.texture = BlockIconHelper.GetIcon(item.id, item.damage);

                    NBTItem nbtItem = generator as NBTItem;
                    if (nbtItem == null || nbtItem.durability == -1 || item.damage == 0)
                    {
                        itemList[i].damageObj.SetActive(false);
                    }
                    else
                    {
                        itemList[i].damageObj.SetActive(true);
                        float process = (nbtItem.durability - item.damage) / (float)nbtItem.durability;
                        itemList[i].damage.rectTransform.sizeDelta = new Vector2(13 * process, 1);
                        itemList[i].damage.color = Color.Lerp(Color.red, Color.green, process);
                    }
                }

                if (item.count > 1)
                {
                    itemList[i].count.gameObject.SetActive(true);
                    itemList[i].count.text = item.count.ToString();
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
            //HeroChangeSelectIndexReq(curIndex);
        }

        InventoryItem curItem = InventorySystem.items[curIndex];
        if (curItem.id != null)
        {
            NBTObject generator = NBTGeneratorManager.GetObjectGenerator(curItem.id);
            if (generator != null)
            {
                PlayerController.ShowBlock(generator, curItem.damage);
            }
            else
            {
                PlayerController.ShowHand();
            }
        }
        else
        {
            PlayerController.ShowHand();
        }
    }

    //void HeroChangeSelectIndexReq(uint index)
    //{
    //    CSHeroChangeSelectIndexReq req = new CSHeroChangeSelectIndexReq
    //    {
    //        Index = index
    //    };
    //    NetworkManager.SendPkgToServer(ENUM_CMD.CS_HERO_CHANGE_SELECT_INDEX_REQ, req);
    //}

    // Use this for initialization
    void Start () {
        Init();

        RefreshUI();
        RefreshStatus();
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
}
