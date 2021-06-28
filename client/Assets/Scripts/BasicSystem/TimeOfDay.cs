using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    // 0 = sunrise
    // 6000 = highnoon
    // 12000 = sunset
    // 18000 = midnight
    // 24000 = sunrise
    float dayTime = 0;

    [Range(0, 24)]
    public float time = 6;
    public Gradient lightColor;

    public bool debug = false;
    public Color skylightcolor = Color.white;

    private void Update()
    {
        if (debug)
        {
            Shader.SetGlobalColor("_SkyLightColor", skylightcolor);
        }
        else
        {
            Shader.SetGlobalColor("_SkyLightColor", lightColor.Evaluate(time / 24));
        }
    }
}
