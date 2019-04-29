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

    // raycast的结果有精度问题，所以要加上精度补偿（每个轴的正负两个方向都试着取一下，最坏需要尝试2^3=8种情况）
    public static bool GetBlockPosByRaycast(Vector3 point, out Vector3Int result)
    {
        float precisionCompensation = 0.01f;
        Vector3Int pos = new Vector3Int();

        pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
        if (test.ContainBlock(pos))
        {
            result = pos;
            return true;
        }
        else
        {
            pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
            if (test.ContainBlock(pos))
            {
                result = pos;
                return true;
            }
            else
            {
                pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
                if (test.ContainBlock(pos))
                {
                    result = pos;
                    return true;
                }
                else
                {
                    pos.Set(Mathf.RoundToInt(point.x - precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
                    if (test.ContainBlock(pos))
                    {
                        result = pos;
                        return true;
                    }
                    else
                    {
                        pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
                        if (test.ContainBlock(pos))
                        {
                            result = pos;
                            return true;
                        }
                        else
                        {
                            pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y - precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
                            if (test.ContainBlock(pos))
                            {
                                result = pos;
                                return true;
                            }
                            else
                            {
                                pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z - precisionCompensation));
                                if (test.ContainBlock(pos))
                                {
                                    result = pos;
                                    return true;
                                }
                                else
                                {
                                    pos.Set(Mathf.RoundToInt(point.x + precisionCompensation), Mathf.RoundToInt(point.y + precisionCompensation), Mathf.RoundToInt(point.z + precisionCompensation));
                                    if (test.ContainBlock(pos))
                                    {
                                        result = pos;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        result = Vector3Int.zero;
        return false;
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
