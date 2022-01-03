using UnityEngine;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Enter Game Scene");
        SettingsPanel.Init();
        ChunkRefresher.Init();
        ChunkPool.Init();
        ItemSelectPanel.Show();
        ChatPanel.ShowChatPanel();
        InventorySystem.Init();
        GameModeManager.Init();

        // load chunk here
        ChunkChecker.Init();

        PlayerController.Init();
        LocalNavMeshBuilder.Init();
        ChunkRefresher.ForceRefreshAll();
    }

    void Update()
    {
        if (PlayerController.isInitialized)
        {
            ChunkChecker.Update();
            ChunkRefresher.Update();
        }

        Texture2D day = Resources.Load<Texture2D>("GUI/Day");
        Shader.SetGlobalTexture("_DayLightTexture", day);
        Texture2D night = Resources.Load<Texture2D>("GUI/Night");
        Shader.SetGlobalTexture("_NightLightTexture", night);
    }

    private void OnDestroy()
    {
        ChunkPool.Uninit();
        InputManager.Destroy();
        UISystem.DestroyUIRoot();
        PlayerController.Destroy();
        NBTHelper.Uninit();
    }
}
