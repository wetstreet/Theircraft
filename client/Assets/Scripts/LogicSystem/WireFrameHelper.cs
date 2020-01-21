using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireFrameHelper : MonoBehaviour
{
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

    float unit = 0.5f;

    public static bool render = false;
    public static Vector3Int pos;

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (!render || Camera.current.tag == "HandCamera")
            return;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);
        
        // Draw lines
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        CSBlockType type = ChunkManager.GetBlockType(pos.x, pos.y, pos.z);

        GL.Vertex3(pos.x - unit, pos.y - unit, pos.z - unit);
        GL.Vertex3(pos.x + unit, pos.y - unit, pos.z - unit);

        GL.Vertex3(pos.x - unit, pos.y - unit, pos.z + unit);
        GL.Vertex3(pos.x + unit, pos.y - unit, pos.z + unit);

        GL.Vertex3(pos.x - unit, pos.y + unit, pos.z - unit);
        GL.Vertex3(pos.x + unit, pos.y + unit, pos.z - unit);

        GL.Vertex3(pos.x - unit, pos.y + unit, pos.z + unit);
        GL.Vertex3(pos.x + unit, pos.y + unit, pos.z + unit);

        GL.Vertex3(pos.x - unit, pos.y - unit, pos.z - unit);
        GL.Vertex3(pos.x - unit, pos.y - unit, pos.z + unit);

        GL.Vertex3(pos.x - unit, pos.y + unit, pos.z - unit);
        GL.Vertex3(pos.x - unit, pos.y + unit, pos.z + unit);

        GL.Vertex3(pos.x + unit, pos.y - unit, pos.z - unit);
        GL.Vertex3(pos.x + unit, pos.y - unit, pos.z + unit);

        GL.Vertex3(pos.x + unit, pos.y + unit, pos.z - unit);
        GL.Vertex3(pos.x + unit, pos.y + unit, pos.z + unit);

        //竖杠
        GL.Vertex3(pos.x + unit, pos.y + unit, pos.z + unit);
        GL.Vertex3(pos.x + unit, pos.y - unit, pos.z + unit);

        GL.Vertex3(pos.x + unit, pos.y + unit, pos.z - unit);
        GL.Vertex3(pos.x + unit, pos.y - unit, pos.z - unit);

        GL.Vertex3(pos.x - unit, pos.y + unit, pos.z + unit);
        GL.Vertex3(pos.x - unit, pos.y - unit, pos.z + unit);

        GL.Vertex3(pos.x - unit, pos.y + unit, pos.z - unit);
        GL.Vertex3(pos.x - unit, pos.y - unit, pos.z - unit);

        GL.End();
    }
}
