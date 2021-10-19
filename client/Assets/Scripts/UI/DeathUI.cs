using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
    static DeathUI Instance;
    public static void Show()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(true);
        }
        else
        {
            Instance = UISystem.InstantiateUI("DeathUI").GetComponent<DeathUI>();
        }
        InputManager.enabled = false;
        PlayerController.LockCursor(false);
    }

    public static void Hide()
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(false);
        }
        InputManager.enabled = true;
        PlayerController.LockCursor(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;

        Utilities.SetClickCallback(transform, "ButtonRespawn", OnClickRespawn);
        Utilities.SetClickCallback(transform, "ButtonTitleScreen", OnClickTitleScreen);
    }

    void OnClickRespawn()
    {
        PlayerController.instance.Respawn();

        Hide();
    }

    void OnClickTitleScreen()
    {
        SettingsPanel.OnClickQuit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
