using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{

    public static InventoryItem[] craftItem = new InventoryItem[9];

    public static InventoryItem resultItem;

    struct Recipe
    {
        public NBTObject grid;
    };
}
