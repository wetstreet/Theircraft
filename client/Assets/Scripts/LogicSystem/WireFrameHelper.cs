using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WireFrameHelper : MonoBehaviour
{
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/WireframeShader");
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

    public static bool render = false;
    public static Vector3Int pos;
    public static Vector3 hitPos;
    public static byte type;
    public static byte data;

    private void OnRenderObject()
    {
        if (!render || UniversalRenderPipeline.currentRenderingCamera != Camera.main)
        {
            return;
        }

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        // Draw lines
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(type);

        float topOffset = generator.topOffset;
        float bottomOffset = generator.bottomOffset;
        float leftOffset = generator.useRadius ? -generator.radius : generator.leftOffset;
        float rightOffset = generator.useRadius ? generator.radius : generator.rightOffset;
        float frontOffset = generator.useRadius ? -generator.radius : generator.frontOffset;
        float backOffset = generator.useRadius ? generator.radius : generator.backOffset;


        // bottom lines
        GL.Vertex3(pos.x + leftOffset, pos.y + bottomOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + bottomOffset, pos.z + frontOffset);

        GL.Vertex3(pos.x + leftOffset, pos.y + bottomOffset, pos.z + backOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + bottomOffset, pos.z + backOffset);

        GL.Vertex3(pos.x + leftOffset, pos.y + bottomOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + leftOffset, pos.y + bottomOffset, pos.z + backOffset);

        GL.Vertex3(pos.x + rightOffset, pos.y + bottomOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + bottomOffset, pos.z + backOffset);

        // top lines
        GL.Vertex3(pos.x + leftOffset, pos.y + topOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + topOffset, pos.z + frontOffset);

        GL.Vertex3(pos.x + leftOffset, pos.y + topOffset, pos.z + backOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + topOffset, pos.z + backOffset);

        GL.Vertex3(pos.x + leftOffset, pos.y + topOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + leftOffset, pos.y + topOffset, pos.z + backOffset);

        GL.Vertex3(pos.x + rightOffset, pos.y + topOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + topOffset, pos.z + backOffset);

        // vertical lines
        GL.Vertex3(pos.x + rightOffset, pos.y + topOffset, pos.z + backOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + bottomOffset, pos.z + backOffset);

        GL.Vertex3(pos.x + rightOffset, pos.y + topOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + rightOffset, pos.y + bottomOffset, pos.z + frontOffset);

        GL.Vertex3(pos.x + leftOffset, pos.y + topOffset, pos.z + backOffset);
        GL.Vertex3(pos.x + leftOffset, pos.y + bottomOffset, pos.z + backOffset);

        GL.Vertex3(pos.x + leftOffset, pos.y + topOffset, pos.z + frontOffset);
        GL.Vertex3(pos.x + leftOffset, pos.y + bottomOffset, pos.z + frontOffset);

        GL.End();
    }
}
