using UnityEngine;
using UnityEngine.SceneManagement;
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
        //transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(OnClickClose);
        Utilities.SetClickCallback(transform, "btn_close", OnClickClose);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
