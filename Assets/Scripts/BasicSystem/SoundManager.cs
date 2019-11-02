using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;

    public static void Init()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/WwiseGlobal");
        GameObject obj = Instantiate(prefab);
        instance = obj.AddComponent<SoundManager>();

        AkBankManager.LoadBank("Main", false, false);
        SetMasterVolume();
    }

    public static void SetMasterVolume()
    {
        AkSoundEngine.SetRTPCValue("MainVolume", SettingsPanel.MasterVolume);
    }
    
    public static void PlayClickSound()
    {
        AkSoundEngine.PostEvent("UI_Click", Camera.main.gameObject);
    }
    
    static Dictionary<CSBlockType, string> type2material = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.Grass, "Grass" },
        {CSBlockType.Dirt, "Gravel" },
        {CSBlockType.Tnt, "Grass" },
        {CSBlockType.Brick, "Stone" },
        {CSBlockType.Furnace, "Stone" },
        {CSBlockType.HayBlock, "Grass" },
    };

    public static void PlayDigSound(CSBlockType type, GameObject gameObject)
    {
        string material = type2material[CSBlockType.Grass];
        if (type2material.ContainsKey(type))
        {
            material = type2material[type];
        }
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Dig", gameObject);
    }

    public static void PlayFootstepSound(CSBlockType type, GameObject gameObject)
    {
        string material = type2material[CSBlockType.Grass];
        if (type2material.ContainsKey(type))
        {
            material = type2material[type];
        }
        AkSoundEngine.SetSwitch("Materials", type2material[type], gameObject);
        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
    }
}
