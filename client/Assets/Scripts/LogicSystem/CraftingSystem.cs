using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public class Item
    {
        public string item;
        public byte data;
        public byte count = 1;
    }

    public class Recipe
    {
        public string name;
        public string type;
        public string group;
        public List<string> pattern;
        public Dictionary<char, List<Item>> key;
        public Item result;
    }

    static Recipe ParseRecipe(TextAsset json)
    {
        Recipe recipe = new Recipe();

        JObject root = JObject.Parse(json.text);
        recipe.name = json.name;
        recipe.type = root["type"].ToString();

        if (root.ContainsKey("group"))
        {
            recipe.group = root["group"].ToString();
        }

        if (root.ContainsKey("pattern"))
        {
            JArray pattern = JArray.Parse(root["pattern"].ToString());
            recipe.pattern = pattern.ToObject<List<string>>();
        }

        if (root.ContainsKey("key"))
        {
            recipe.key = new Dictionary<char, List<Item>>();

            JObject key = JObject.Parse(root["key"].ToString());
            foreach (JToken v in key.Children())
            {
                if (v.First.Type == JTokenType.Array)
                {
                    recipe.key[v.Path[0]] = v.First.ToObject<List<Item>>();
                }
                else if (v.First.Type == JTokenType.Object)
                {
                    recipe.key[v.Path[0]] = new List<Item> { v.First.ToObject<Item>() };
                }
            }
        }

        if (root.ContainsKey("result"))
        {
            JObject result = JObject.Parse(root["result"].ToString());
            recipe.result = result.ToObject<Item>();
        }

        return recipe;
    }

    static Dictionary<string, Recipe> name2recipe = new Dictionary<string, Recipe>();

    public static void Init()
    {
        UnityEngine.Object[] jsons = Resources.LoadAll("Recipes");
        foreach (TextAsset json in jsons)
        {
            name2recipe[json.name] = ParseRecipe(json); ;
        }
    }

    protected static int resultIndex = 45;

    static string[] recipeNames = new string[] {
        "oak_planks",
        "birch_planks",
        "spruce_planks",
        "jungle_planks",
        "acacia_planks",
        "dark_oak_planks",

        "oak_stairs",
        "birch_stairs",
        "spruce_stairs",
        "jungle_stairs",
        "acacia_stairs",
        "dark_oak_stairs",

        "oak_wooden_slab",
        "birch_wooden_slab",
        "spruce_wooden_slab",
        "jungle_wooden_slab",
        "acacia_wooden_slab",
        "dark_oak_wooden_slab",

        "wooden_pickaxe",
        "stone_pickaxe",
        "iron_pickaxe",
        "golden_pickaxe",
        "diamond_pickaxe",

        "wooden_axe",
        "stone_axe",
        "iron_axe",
        "golden_axe",
        "diamond_axe",

        "wooden_sword",
        "stone_sword",
        "iron_sword",
        "golden_sword",
        "diamond_sword",

        "leather_helmet",
        "iron_helmet",
        "golden_helmet",
        "diamond_helmet",

        "leather_boots",
        "iron_boots",
        "golden_boots",
        "diamond_boots",

        "leather_chestplate",
        "iron_chestplate",
        "golden_chestplate",
        "diamond_chestplate",

        "leather_leggings",
        "iron_leggings",
        "golden_leggings",
        "diamond_leggings",

        "iron_block",
        "iron_ingot_from_block",

        "torch",

        "furnace",
        "stick",
        "crafting_table",

        "chest",

        "wooden_door",
    };


    public static void CraftItems()
    {
        if (InventorySystem.items[resultIndex].id == null) return;

        foreach (int i in CraftingTableUI.indexList)
        {
            if (InventorySystem.items[i].count > 0)
            {
                InventorySystem.items[i].count--;

                if (InventorySystem.items[i].count == 0)
                {
                    InventorySystem.items[i].id = null;
                    InventorySystem.items[i].damage = 0;
                }
            }
        }

        if (InventorySystem.grabItem.id == InventorySystem.items[resultIndex].id &&
            InventorySystem.grabItem.damage == InventorySystem.items[resultIndex].damage)
        {
            InventorySystem.grabItem.count += InventorySystem.items[resultIndex].count;
        }
        else
        {
            InventorySystem.grabItem.id = InventorySystem.items[resultIndex].id;
            InventorySystem.grabItem.damage = InventorySystem.items[resultIndex].damage;
            InventorySystem.grabItem.count = InventorySystem.items[resultIndex].count;
        }

        CheckCanCraft();
    }

    static bool CheckRecipe(Recipe recipe, InventoryItem[,] grid)
    {
        int row = recipe.pattern.Count;
        int column = recipe.pattern[0].Length;

        int gridColumn = grid.GetUpperBound(0) + 1;
        int gridRow = grid.GetUpperBound(1) + 1;
        if (row != gridRow || column != gridColumn)
        {
            return false;
        }

        Item[,] recipeGrid = new Item[column, row];
        for (int j = 0; j < row; j++)
        {
            for (int i = 0; i < column; i++)
            {
                char key = recipe.pattern[j][i];
                if (key != ' ')
                {
                    bool keyMatch = false;

                    foreach (Item item in recipe.key[key])
                    {
                        recipeGrid[i, j] = item;
                        if (recipeGrid[i, j].item == grid[i, j].id && recipeGrid[i, j].data == grid[i, j].damage)
                        {
                            keyMatch = true;
                            break;
                        }
                    }
                    if (!keyMatch)
                    {
                        return false;
                    }
                }
                else
                {
                    if (grid[i, j].id != null)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    static InventoryItem[,] grid = new InventoryItem[3, 3];
    static char[,] pattern = new char[3, 3];
    public static void CheckCanCraft()
    {
        grid[0, 0] = InventorySystem.items[36];
        grid[1, 0] = InventorySystem.items[37];
        grid[2, 0] = InventorySystem.items[38];
        grid[0, 1] = InventorySystem.items[39];
        grid[1, 1] = InventorySystem.items[40];
        grid[2, 1] = InventorySystem.items[41];
        grid[0, 2] = InventorySystem.items[42];
        grid[1, 2] = InventorySystem.items[43];
        grid[2, 2] = InventorySystem.items[44];

        // calc bounding box
        int minX = 2;
        int maxX = 0;
        int minY = 2;
        int maxY = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[i, j].id != null)
                {
                    if (i < minX)
                    {
                        minX = i;
                    }
                    if (i > maxX)
                    {
                        maxX = i;
                    }
                    if (j < minY)
                    {
                        minY = j;
                    }
                    if (j > maxY)
                    {
                        maxY = j;
                    }
                }
            }
        }

        Recipe matchRecipe = null;
        bool canCraft = false;

        // trim spaces
        int row = Mathf.Max(maxY - minY + 1, 0);
        int column = Mathf.Max(maxX - minX + 1, 0);
        if (row != 0 || column != 0)
        {
            InventoryItem[,] trimedGrid = new InventoryItem[column, row];
            for (int i = minY; i <= maxY; i++)
            {
                for (int j = minX; j <= maxX; j++)
                {
                    trimedGrid[j - minX, i - minY] = grid[j, i];
                }
            }

            // compare
            foreach (string recipeName in recipeNames)
            {
                try
                {
                    Recipe recipe = name2recipe[recipeName];
                    canCraft = CheckRecipe(recipe, trimedGrid);
                    if (canCraft)
                    {
                        matchRecipe = recipe;
                        break;
                    }
                }
                catch
                {
                    Debug.Log("no recipe,name=" + recipeName);
                }
            }
        }

        if (canCraft)
        {
            InventorySystem.items[resultIndex].id = matchRecipe.result.item;
            InventorySystem.items[resultIndex].damage = matchRecipe.result.data;
            InventorySystem.items[resultIndex].count = matchRecipe.result.count;
        }
        else
        {
            InventorySystem.items[resultIndex].id = null;
            InventorySystem.items[resultIndex].damage = 0;
            InventorySystem.items[resultIndex].count = 0;
        }
    }

}
