using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public enum SoundMaterial
{
    Grass,
    Gravel,
    Sand,
    Stone,
    Wood,
    Glass,
    Snow,
}

public class SoundManager : MonoBehaviour
{
    static readonly string Material_Grass = "Grass";
    static readonly string Material_Gravel = "Gravel";
    static readonly string Material_Sand = "Sand";
    static readonly string Material_Stone = "Stone";
    static readonly string Material_Wood = "Wood";
    static readonly string Material_Glass = "Glass";

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

    public static void PlayPlaceSound(byte type, GameObject gameObject)
    {
        string material = NBTGeneratorManager.GetMeshGenerator(type).soundMaterial.ToString();
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Place", gameObject);
    }

    public static void PlayBreakSound(byte type, GameObject gameObject)
    {
        string material = NBTGeneratorManager.GetMeshGenerator(type).soundMaterial.ToString();
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Break", gameObject);
    }

    public static void PlayFootstepSound(byte type, GameObject gameObject)
    {
        string material = NBTGeneratorManager.GetMeshGenerator(type).soundMaterial.ToString();
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
    }

    public static void PlayPopSound()
    {
        AkSoundEngine.PostEvent("Player_Pop", Camera.main.gameObject);
    }

    public static void Play3DSound(string name, GameObject obj)
    {
        AkSoundEngine.PostEvent(name, obj);
    }
}
