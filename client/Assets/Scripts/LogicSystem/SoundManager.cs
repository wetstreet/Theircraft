using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static readonly string Material_Grass = "Grass";
    static readonly string Material_Gravel = "Gravel";
    static readonly string Material_Sand = "Sand";
    static readonly string Material_Stone = "Stone";
    static readonly string Material_Wood = "Wood";

    static Dictionary<CSBlockType, string> type2material = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.Dirt, Material_Gravel },
        {CSBlockType.Brick, Material_Stone },
        {CSBlockType.BrickStairs, Material_Stone },
        {CSBlockType.Furnace, Material_Stone },
        {CSBlockType.BrickWall, Material_Stone },
        {CSBlockType.Stone, Material_Stone },
        {CSBlockType.DoubleStoneSlab, Material_Stone },
        {CSBlockType.Torch, Material_Wood },
        {CSBlockType.OakLog, Material_Wood },
        {CSBlockType.OakPlanks, Material_Wood },
        {CSBlockType.RedSand, Material_Sand },
    };

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

    public static void PlayDigSound(CSBlockType type, GameObject gameObject)
    {
        string material = Material_Grass;
        if (type2material.ContainsKey(type))
        {
            material = type2material[type];
        }
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Dig", gameObject);
    }

    public static void PlayFootstepSound(CSBlockType type, GameObject gameObject)
    {
        string material = Material_Grass;
        if (type2material.ContainsKey(type))
        {
            material = type2material[type];
        }
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
    }
}
