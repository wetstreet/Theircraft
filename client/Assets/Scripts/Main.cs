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
        CraftingSystem.Init();

        SceneManager.LoadScene("LoginScene");
    }
}
