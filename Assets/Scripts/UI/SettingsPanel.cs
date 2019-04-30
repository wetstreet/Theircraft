using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {

    public static SettingsPanel Instance;

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
        AkSoundEngine.PostEvent("UI_Click", gameObject);
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
        Application.Quit();
    }

	// Use this for initialization
	void Start ()
    {
        transform.Find("btn_quit").GetComponent<Button>().onClick.AddListener(OnClickQuit);
        transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(OnClickClose);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
