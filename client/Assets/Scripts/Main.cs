using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    // Use this for initialization
    void Start ()
    {
        SoundManager.Init();
        LocalizationManager.Init();
        NBTGeneratorManager.Init();
        TextureArrayManager.Init();
        NBTGeneratorManager.AfterTexutreInit();
        CraftingSystem.Init();

        SceneManager.LoadScene("LoginScene");
    }
}
