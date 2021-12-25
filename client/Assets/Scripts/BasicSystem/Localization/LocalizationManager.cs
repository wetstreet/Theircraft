using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum Language
{
    English,
    Chinese,
}

public class LocalizationManager
{
    static Dictionary<string, string> langDict = new Dictionary<string, string>();

    static List<LocalizationReplacer> replacerList = new List<LocalizationReplacer>();

    static Dictionary<Language, string> lang2path = new Dictionary<Language, string>()
    {
        { Language.English, "Languages/en_us"},
        { Language.Chinese, "Languages/zh_cn"},
    };
    
    public static void Init()
    {
        langDict.Clear();
        
        Language currentLanguage = SettingsPanel.Language;
        string path = lang2path[currentLanguage];
        TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
        string language = textAsset.text;
        string[] lines = language.Split('\n');
        foreach (string line in lines)
        {
            string[] item = line.Split('=');
            if (item.Length == 2)
            {
                string key = item[0];
                string value = item[1].TrimEnd();
                if (langDict.ContainsKey(key))
                {
                    langDict[key] = value;
                }
                else
                {
                    langDict.Add(key, value);
                }
            }
        }

        Debug.Log("Init Language Done! current language=" + currentLanguage);
        RefreshAll();
    }

    public static void Add(LocalizationReplacer replacer)
    {
        replacerList.Add(replacer);
    }

    public static void Remove(LocalizationReplacer replacer)
    {
        replacerList.Remove(replacer);
    }

    static void RefreshAll()
    {
        foreach (LocalizationReplacer replacer in replacerList)
        {
            replacer.Refresh();
        }
    }

    public static string GetText(string key)
    {
        if (langDict.ContainsKey(key))
        {
            return langDict[key];
        }
        return null;
    }
}
