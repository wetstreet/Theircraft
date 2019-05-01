using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static void PlayClickSound()
    {
        AkSoundEngine.PostEvent("UI_Click", Camera.main.gameObject);
    }
}
