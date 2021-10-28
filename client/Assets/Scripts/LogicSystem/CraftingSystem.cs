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

    [UnityEditor.MenuItem("Assets/RecipeTest")]
    public static void Init()
    {
        UnityEngine.Object[] jsons = Resources.LoadAll("Recipes");
        foreach (TextAsset json in jsons)
        {
            Debug.Log(json.name + "," + json);
            name2recipe[json.name] = ParseRecipe(json.text); ;
        }

        //TextAsset json = UnityEditor.Selection.activeObject as TextAsset;
        //name2recipe[json.name] = ParseRecipe(json.text);

        //Recipe recipe = JsonConvert.DeserializeObject<Recipe>(json.text);
        //Debug.Log(json);
        //Debug.Log(recipe.type);
        //Debug.Log(recipe.group);
        //foreach (string p in recipe.pattern)
        //    Debug.Log(p);
        //foreach (KeyValuePair<string, Item> pair in recipe.key)
        //{
        //    Debug.Log(pair.Key);
        //    Debug.Log(pair.Value.item);
        //    Debug.Log(pair.Value.data);
        //}
        //Debug.Log(recipe.result.item);
        //Debug.Log(recipe.result.data);
        //Debug.Log(recipe.result.count);
    }


}
