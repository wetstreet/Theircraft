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
    TextMeshProUGUI fullscreenLabel;
    TextMeshProUGUI graphicsLabel;
    TextMeshProUGUI vsyncLabel;
    Slider framerateSlider;
    TextMeshProUGUI framerateLabel;

    static readonly string MASTER_VOLUME_KEY = "MASTER_VOLUME_KEY";
    static readonly string RENDER_DISTANCE_KEY = "RENDER_DISTANCE_KEY";
    static readonly string FRAME_RATE_INDEX_KEY = "FRAME_RATE_INDEX_KEY";
    static readonly string VSYNC_KEY = "VSYNC_KEY";
    static readonly string GRAPHICS_LEVEL_KEY = "GRAPHICS_LEVEL_KEY";

    public static int MasterVolume
    {
        get { return PlayerPrefs.GetInt(MASTER_VOLUME_KEY, 100); }
        set { PlayerPrefs.SetInt(MASTER_VOLUME_KEY, value); }
    }

    static readonly int MinChunkView = 1;
    static readonly int MaxChunkView = 15;

    public static int RenderDistance
    {
        get { return Mathf.Clamp(PlayerPrefs.GetInt(RENDER_DISTANCE_KEY, 8), MinChunkView, MaxChunkView); }
        set { PlayerPrefs.SetInt(RENDER_DISTANCE_KEY, value); }
    }

    public static int FrameRateIndex
    {
        get { return PlayerPrefs.GetInt(FRAME_RATE_INDEX_KEY, 26); }
        set { PlayerPrefs.SetInt(FRAME_RATE_INDEX_KEY, value); }
    }

    public static int VSync
    {
        get { return PlayerPrefs.GetInt(VSYNC_KEY, 0); }
        set { PlayerPrefs.SetInt(VSYNC_KEY, value); }
    }

    public static int GraphicsLevel
    {
        get { return PlayerPrefs.GetInt(GRAPHICS_LEVEL_KEY, 2); }
        set { PlayerPrefs.SetInt(GRAPHICS_LEVEL_KEY, value); }
    }

    public static int FrameRate
    {
        get
        {
            return FrameRateIndex == 26 ? -1 : FrameRateIndex * 10;
        }
    }

    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
            Instance.RefreshFullscreenLabel();
        }
        else
        {
            Instance = UISystem.InstantiateUI("SettingsPanel").GetComponent<SettingsPanel>();
        }

        InputManager.enabled = false;
        PlayerController.LockCursor(false);
    }

    void OnClickClose()
    {
        Hide();
    }

    public static void Hide()
    {
        InputManager.enabled = true;
        PlayerController.LockCursor(true);
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
    }

    public static void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Instance != null && Instance.gameObject.activeSelf)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        HandleInput();
    }

    void OnClickQuit()
    {
        LocalServer.SaveData();
        SceneManager.LoadScene("LoginScene");
    }

    public static void Init()
    {
        RefreshGraphicsLevel();
        RefreshFrameRate();
        RefreshVSync();
    }

    // Use this for initialization
    void Start ()
    {
        transform.Find("btn_quit").GetComponent<Button>().onClick.AddListener(OnClickQuit);
        Utilities.SetClickCallback(transform, "btn_close", OnClickClose);
        renderDistanceSlider = transform.Find("grid/slider_chunkview").GetComponent<Slider>();
        renderDistanceSlider.minValue = MinChunkView;
        renderDistanceSlider.maxValue = MaxChunkView;
        renderDistanceSlider.onValueChanged.AddListener(OnRenderDistanceChange);
        sliderLabel = transform.Find("grid/slider_chunkview/slider_label").GetComponent<TextMeshProUGUI>();
        masterVolumeSlider = transform.Find("grid/slider_master_volume").GetComponent<Slider>();
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChange);
        masterVolumeLabel = transform.Find("grid/slider_master_volume/label_master_volume").GetComponent<TextMeshProUGUI>();

        framerateSlider = transform.Find("grid/slider_framerate").GetComponent<Slider>();
        framerateSlider.onValueChanged.AddListener(OnFrameRateChange);
        framerateLabel = transform.Find("grid/slider_framerate/slider_label").GetComponent<TextMeshProUGUI>();
        framerateSlider.value = FrameRateIndex;
        RefreshFrameRateLabel();

        Utilities.SetClickCallback(transform, "grid/btn_fullscreen", OnClickFullscreen);
        fullscreenLabel = transform.Find("grid/btn_fullscreen/label").GetComponent<TextMeshProUGUI>();
        RefreshFullscreenLabel();

        Utilities.SetClickCallback(transform, "grid/btn_vsync", OnClickVSync);
        vsyncLabel = transform.Find("grid/btn_vsync/label").GetComponent<TextMeshProUGUI>();
        RefreshVSyncLabel();

        Utilities.SetClickCallback(transform, "grid/btn_graphics", OnClickGraphics);
        graphicsLabel = transform.Find("grid/btn_graphics/label").GetComponent<TextMeshProUGUI>();
        RefreshGraphicsLabel();

        RefreshRenderDistanceLabel();
        renderDistanceSlider.value = RenderDistance;

        RefreshMasterVolumeLabel();
        masterVolumeSlider.value = MasterVolume;
    }

    void RefreshFullscreenLabel()
    {
        if (Screen.fullScreen)
        {
            fullscreenLabel.text = "Fullscreen: On";
        }
        else
        {
            fullscreenLabel.text = "Fullscreen: Off";
        }
    }

    string KEY_WINDOWED_WIDTH = "KEY_WINDOWED_WIDTH";
    string KEY_WINDOWED_HEIGHT = "KEY_WINDOWED_HEIGHT";
    void OnClickFullscreen()
    {
#if !UNITY_EDITOR
        if (!Screen.fullScreen)
        {
            PlayerPrefs.SetInt(KEY_WINDOWED_WIDTH, Screen.width);
            PlayerPrefs.SetInt(KEY_WINDOWED_HEIGHT, Screen.height);
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            fullscreenLabel.text = "Fullscreen: On";
        }
        else
        {
            int width = PlayerPrefs.GetInt(KEY_WINDOWED_WIDTH, 800);
            int height = PlayerPrefs.GetInt(KEY_WINDOWED_HEIGHT, 600);
            Screen.SetResolution(width, height, false);
            fullscreenLabel.text = "Fullscreen: Off";
        }
#endif
    }

    void RefreshGraphicsLabel()
    {
        int quality = GraphicsLevel;
        if (quality == 0)
        {
            graphicsLabel.text = "Graphics: Fast";
        }
        else if (quality == 1)
        {
            graphicsLabel.text = "Graphics: Good";
        }
        else if (quality == 2)
        {
            graphicsLabel.text = "Graphics: Fancy";
        }
    }

    public static void RefreshGraphicsLevel()
    {
        QualitySettings.SetQualityLevel(GraphicsLevel);
    }

    void OnClickGraphics()
    {
        GraphicsLevel = (GraphicsLevel + 1) % 3;
        RefreshGraphicsLevel();
        RefreshGraphicsLabel();
        RefreshVSync();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void OnFrameRateChange(float value)
    {
        FrameRateIndex = (int)value;
        RefreshFrameRate();
        RefreshFrameRateLabel();
    }

    void RefreshFrameRateLabel()
    {
        if (FrameRate == -1)
        {
            framerateLabel.text = $"Max Framerate: Unlimited";
        }
        else
        {
            framerateLabel.text = $"Max Framerate: {FrameRate}";
        }
    }

    public static void RefreshFrameRate()
    {
        Application.targetFrameRate = FrameRate;
    }

    public static void RefreshVSync()
    {
        QualitySettings.vSyncCount = VSync;
    }

    void RefreshVSyncLabel()
    {
        int vsync = VSync;
        if (vsync == 0)
        {
            vsyncLabel.text = "Use VSync: Off";
        }
        else
        {
            vsyncLabel.text = "Use VSync: On";
        }
    }

    void OnClickVSync()
    {
        VSync = VSync == 1 ? 0 : 1;
        RefreshVSync();
        RefreshVSyncLabel();
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
        {
            masterVolumeLabel.text = $"Master Volume: OFF";
        }
        else
        {
            masterVolumeLabel.text = $"Master Volume: {MasterVolume}%";
        }
    }
}
