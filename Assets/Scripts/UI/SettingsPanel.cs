using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {

    public static SettingsPanel Instance;
    Slider renderDistanceSlider;
    TextMeshProUGUI sliderLabel;
    Slider masterVolumeSlider;
    TextMeshProUGUI masterVolumeLabel;

    static readonly string MASTER_VOLUME_KEY = "MASTER_VOLUME_KEY";
    static readonly string RENDER_DISTANCE_KEY = "RENDER_DISTANCE_KEY";

    public static void Show()
    {
        if (Instance != null)
            Instance.gameObject.SetActive(true);
        else
            Instance = UISystem.InstantiateUI("SettingsPanel").GetComponent<SettingsPanel>();

        mergetestPlayerController.LockCursor(false);
    }

    void OnClickClose()
    {
        Hide();
    }

    public static void Hide()
    {
        mergetestPlayerController.LockCursor(true);
        if (Instance != null)
            Instance.gameObject.SetActive(false);

    }

    public static void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Instance != null && Instance.gameObject.activeSelf)
                Hide();
            else
                Show();
        }
    }

    void OnClickQuit()
    {
        SceneManager.LoadScene("LoginScene");
        MainMenu.Show();
    }

	// Use this for initialization
	void Start ()
    {
        //没做退出登陆，先屏蔽
        transform.Find("btn_quit").GetComponent<Button>().interactable = false;
        //transform.Find("btn_quit").GetComponent<Button>().onClick.AddListener(OnClickQuit);
        Utilities.SetClickCallback(transform, "btn_close", OnClickClose);
        renderDistanceSlider = transform.Find("slider_chunkview").GetComponent<Slider>();
        renderDistanceSlider.onValueChanged.AddListener(OnRenderDistanceChange);
        sliderLabel = transform.Find("slider_label").GetComponent<TextMeshProUGUI>();
        masterVolumeSlider = transform.Find("slider_master_volume").GetComponent<Slider>();
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChange);
        masterVolumeLabel = transform.Find("label_master_volume").GetComponent<TextMeshProUGUI>();

        RefreshRenderDistanceLabel();
        RefreshMasterVolumeLabel();

        int volume = PlayerPrefs.GetInt("MASTER_VOLUME_KEY");
        masterVolumeSlider.value = volume;
    }

    void RefreshRenderDistanceLabel()
    {
        sliderLabel.text = $"Render Distance: {DataCenter.renderDistance} chunks";
    }

    void OnRenderDistanceChange(float value)
    {
        DataCenter.renderDistance = (int)value;
        RefreshRenderDistanceLabel();
    }

    void OnMasterVolumeChange(float value)
    {
        PlayerPrefs.SetInt("MASTER_VOLUME_KEY", (int)value);
        SetMasterVolume();
        RefreshMasterVolumeLabel();
    }

    public static void SetMasterVolume()
    {
        int volume = PlayerPrefs.GetInt("MASTER_VOLUME_KEY");
        AkSoundEngine.SetRTPCValue("MainVolume", volume);
    }

    void RefreshMasterVolumeLabel()
    {
        int volume = PlayerPrefs.GetInt("MASTER_VOLUME_KEY");
        if (volume == 0)
            masterVolumeLabel.text = $"Master Volume: OFF";
        else
            masterVolumeLabel.text = $"Master Volume: {volume}%";
    }
}
