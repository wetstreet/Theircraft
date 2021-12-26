using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    static MainMenu instance;

    public static void Show()
    {
        instance = UISystem.InstantiateUI("MainMenu").GetComponent<MainMenu>();
    }

    public static void Close()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Utilities.SetClickCallback(transform, "ButtonSingle", OnClickSingle);
        Utilities.SetClickCallback(transform, "ButtonQuit", OnClickQuit);
        //Utilities.SetClickCallback(transform, "ButtonClear", OnClickClear);
        Utilities.SetClickCallback(transform, "ButtonLanguage", OnClickLanguage);
        
        TextMeshProUGUI text = transform.Find("version").GetComponent<TextMeshProUGUI>();
        text.text = "Theircraft " + Application.version;
    }

    void OnClickSingle()
    {
        SelectWorldUI.Show();
    }

    void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnClickClear()
    {
        DeleteWorldUI.Show();
    }

    void OnClickLanguage()
    {
        if (SettingsPanel.Language == Language.English)
        {
            SettingsPanel.Language = Language.Chinese;
            LocalizationManager.Init();
        }
        else
        {
            SettingsPanel.Language = Language.English;
            LocalizationManager.Init();
        }
    }
}
