using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireFrameHelper : MonoBehaviour
{

    public bool render = false;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    float unit = 0.502f;

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (!render)
            return;
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        // Draw lines
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        GL.Vertex3(-unit, -unit, -unit);
        GL.Vertex3(unit, -unit, -unit);

        GL.Vertex3(-unit, -unit, unit);
        GL.Vertex3(unit, -unit, unit);

        GL.Vertex3(-unit, unit, -unit);
        GL.Vertex3(unit, unit, -unit);

        GL.Vertex3(-unit, unit, unit);
        GL.Vertex3(unit, unit, unit);

        GL.Vertex3(-unit, -unit, -unit);
        GL.Vertex3(-unit, -unit, unit);

        GL.Vertex3(-unit, unit, -unit);
        GL.Vertex3(-unit, unit, unit);

        GL.Vertex3(unit, -unit, -unit);
        GL.Vertex3(unit, -unit, unit);

        GL.Vertex3(unit, unit, -unit);
        GL.Vertex3(unit, unit, unit);

        //竖杠
        GL.Vertex3(unit, unit, unit);
        GL.Vertex3(unit, -unit, unit);

        GL.Vertex3(unit, unit, -unit);
        GL.Vertex3(unit, -unit, -unit);

        GL.Vertex3(-unit, unit, unit);
        GL.Vertex3(-unit, -unit, unit);

        GL.Vertex3(-unit, unit, -unit);
        GL.Vertex3(-unit, -unit, -unit);

        GL.End();
    }
}
