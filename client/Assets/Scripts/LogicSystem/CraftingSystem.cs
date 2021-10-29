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
        public string type;
        public string group;
        public List<string> pattern;
        public Dictionary<string, List<Item>> key;
        public Item result;
    }

    static Recipe ParseRecipe(string json)
    {
        Recipe recipe = new Recipe();

        JObject root = JObject.Parse(json);

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
            recipe.key = new Dictionary<string, List<Item>>();

            JObject key = JObject.Parse(root["key"].ToString());
            foreach (JToken v in key.Children())
            {
                if (v.First.Type == JTokenType.Array)
                {
                    recipe.key[v.Path] = v.First.ToObject<List<Item>>();
                }
                else if (v.First.Type == JTokenType.Object)
                {
                    recipe.key[v.Path] = new List<Item> { v.First.ToObject<Item>() };
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
            name2recipe[json.name] = ParseRecipe(json.text); ;
        }
    }

    protected static int resultIndex = 45;

    public static void CraftItems()
    {
        if (InventorySystem.items[resultIndex].id == null) return;

        foreach (int i in SurvivalInventory.indexList)
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

        InventorySystem.grabItem.id = InventorySystem.items[resultIndex].id;
        InventorySystem.grabItem.damage = InventorySystem.items[resultIndex].damage;
        InventorySystem.grabItem.count = InventorySystem.items[resultIndex].count;

        CheckCanCraft();
    }

    static bool CheckRecipe(Recipe recipe, string firstRow, string secondRow, string firstID, int firstData)
    {
        if (recipe.pattern.Count == 1)
        {
            if (recipe.pattern[0] == firstRow)
            {
                foreach (Item item in recipe.key["#"])
                {
                    if (item.item == firstID && item.data == firstData)
                    {
                        return true;
                    }
                }
            }
        }
        else if (recipe.pattern.Count == 2)
        {
            if (recipe.pattern[0] == firstRow && recipe.pattern[1] == secondRow)
            {
                foreach (Item item in recipe.key["#"])
                {
                    if (item.item == firstID && item.data == firstData)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static void CheckCanCraft()
    {
        string firstRow = "";
        string secondRow = "";

        InventoryItem upperLeft = InventorySystem.items[36];
        InventoryItem upperRight = InventorySystem.items[37];
        InventoryItem bottomLeft = InventorySystem.items[39];
        InventoryItem bottomRight = InventorySystem.items[40];

        int count = 0;
        if (upperLeft.id != null) count++;
        if (upperRight.id != null) count++;
        if (bottomLeft.id != null) count++;
        if (bottomRight.id != null) count++;

        string firstID = null;
        int firstData = 0;

        if (count == 1)
        {
            firstRow = "#";
            if (upperLeft.id != null)
            {
                firstID = upperLeft.id;
                firstData = upperLeft.damage;
            }
            else if (upperRight.id != null)
            {
                firstID = upperRight.id;
                firstData = upperRight.damage;
            }
            else if (bottomLeft.id != null)
            {
                firstID = bottomLeft.id;
                firstData = bottomLeft.damage;
            }
            else if (bottomRight.id != null)
            {
                firstID = bottomRight.id;
                firstData = bottomRight.damage;
            }
        }
        else if (count == 2)
        {
            if (upperLeft.id != null && upperRight.id != null)
            {
                firstRow = "##";
                firstID = upperLeft.id;
                firstData = upperLeft.damage;
            }
            else if (bottomLeft.id != null && bottomRight.id != null)
            {
                firstRow = "##";
                firstID = upperLeft.id;
                firstData = upperLeft.damage;
            }
            else if (upperLeft.id != null && bottomLeft.id != null)
            {
                firstRow =  "#";
                secondRow = "#";
                firstID = upperLeft.id;
                firstData = upperLeft.damage;
            }
            else if (upperRight.id != null && bottomRight.id != null)
            {
                firstRow =  "#";
                secondRow = "#";
                firstID = upperRight.id;
                firstData = upperRight.damage;
            }
        }
        else if (count == 3)
        {
            if (upperLeft.id == null)
            {
                firstRow =  " #";
                secondRow = "##";
                firstID = upperRight.id;
                firstData = upperRight.damage;
            }
            else if (upperRight.id == null)
            {
                firstRow =  "# ";
                secondRow = "##";
                firstID = upperLeft.id;
                firstData = upperLeft.damage;
            }
            else if (bottomLeft.id == null)
            {
                firstRow =  "##";
                secondRow = " #";
                firstID = upperLeft.id;
                firstData = upperLeft.damage;
            }
            else if (bottomRight.id == null)
            {
                firstRow =  "##";
                secondRow = "# ";
                firstID = upperLeft.id;
                firstData = upperLeft.damage;
            }
        }
        else if (count == 4)
        {
            firstRow =  "##";
            secondRow = "##";
            firstID = upperLeft.id;
            firstData = upperLeft.damage;
        }

        string[] recipeNames = new string[] { "stick", "crafting_table" };
        Recipe matchRecipe = null;
        bool canCraft = false;
        foreach (string recipeName in recipeNames)
        {
            Recipe recipe = name2recipe[recipeName];
            canCraft = CheckRecipe(recipe, firstRow, secondRow, firstID, firstData);
            if (canCraft)
            {
                matchRecipe = recipe;
                break;
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
