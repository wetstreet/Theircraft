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
}
