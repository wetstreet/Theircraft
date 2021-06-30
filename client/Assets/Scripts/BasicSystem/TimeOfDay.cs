using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    [HideInInspector] public float tick;

    public bool elapse = true;

    public Gradient lightColor;
    public Gradient skyTopColor;
    public Gradient skyBottomColor;
    public float skyHeight = 1000;
    public float skyTransition = 1000;

    private void Start()
    {
        tick = 6000;
    }

    private void Update()
    {
        if (elapse)
        {
            tick += Time.deltaTime * 20;

            if (tick > 24000)
                tick -= 24000;
        }
        float time01 = tick / 24000;

        Shader.SetGlobalColor("_SkyLightColor", lightColor.Evaluate(time01));
        Shader.SetGlobalColor("_SkyTopColor", skyTopColor.Evaluate(time01).linear);
        Shader.SetGlobalColor("_SkyBottomColor", skyBottomColor.Evaluate(time01).linear);
        Shader.SetGlobalFloat("_SkyHeight", skyHeight);
        Shader.SetGlobalFloat("_SkyTransition", skyTransition);
    }
}
