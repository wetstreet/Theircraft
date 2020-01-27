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

    public static int MasterVolume
    {
        get { return PlayerPrefs.GetInt(MASTER_VOLUME_KEY, 100); }
        set { PlayerPrefs.SetInt(MASTER_VOLUME_KEY, value); }
    }

    static readonly int MinChunkView = 2;
    static readonly int MaxChunkView = 15;

    public static int RenderDistance
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt(RENDER_DISTANCE_KEY, 3), MinChunkView, MaxChunkView); }
        set { PlayerPrefs.SetInt(RENDER_DISTANCE_KEY, value); }
    }

    public static void Show()
    {
        if (Instance != null)
            Instance.gameObject.SetActive(true);
        else
            Instance = UISystem.InstantiateUI("SettingsPanel").GetComponent<SettingsPanel>();

        PlayerController.LockCursor(false);
    }

    void OnClickClose()
    {
        Hide();
    }

    public static void Hide()
    {
        PlayerController.LockCursor(true);
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
        renderDistanceSlider.minValue = MinChunkView;
        renderDistanceSlider.maxValue = MaxChunkView;
        renderDistanceSlider.onValueChanged.AddListener(OnRenderDistanceChange);
        sliderLabel = transform.Find("slider_label").GetComponent<TextMeshProUGUI>();
        masterVolumeSlider = transform.Find("slider_master_volume").GetComponent<Slider>();
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChange);
        masterVolumeLabel = transform.Find("label_master_volume").GetComponent<TextMeshProUGUI>();

        RefreshRenderDistanceLabel();
        renderDistanceSlider.value = RenderDistance;

        RefreshMasterVolumeLabel();
        masterVolumeSlider.value = MasterVolume;
    }

    void RefreshRenderDistanceLabel()
    {
        sliderLabel.text = $"Render Distance: {RenderDistance} chunks";
    }

    void OnRenderDistanceChange(float value)
    {
        RenderDistance = (int)value;
        RefreshRenderDistanceLabel();
    }

    void OnMasterVolumeChange(float value)
    {
        MasterVolume = (int)value;
        SoundManager.SetMasterVolume();
        RefreshMasterVolumeLabel();
    }

    void RefreshMasterVolumeLabel()
    {
        if (MasterVolume == 0)
            masterVolumeLabel.text = $"Master Volume: OFF";
        else
            masterVolumeLabel.text = $"Master Volume: {MasterVolume}%";
    }
}
