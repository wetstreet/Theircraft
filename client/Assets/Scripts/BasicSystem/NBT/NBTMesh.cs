using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTMesh
{
    public Vector3[] vertexArray;
    public Color32[] colorArray;
    public Vector2[] uvArray;
    public Vector2[] uv2Array;
    public Vector3[] normalArray;
    public int[] triangleArray;
    int vCount;
    bool finished = false;
    public int vertexCount
    {
        get
        {
            return vCount;
        }
        set
        {
            if (finished)
            {
                Debug.LogError("add face after finish!");
            }
            vCount = value;
        }
    }
    public int triangleCount;

    public Mesh mesh;

    public NBTMesh(int size)
    {
        mesh = new Mesh();

        vertexArray = new Vector3[size];
        colorArray = new Color32[size];
        uvArray = new Vector2[size];
        uv2Array = new Vector2[size];
        normalArray = new Vector3[size];
        triangleArray = new int[size];
    }

    public void Dispose()
    {
        vertexArray = null;
        colorArray = null;
        uvArray = null;
        uv2Array = null;
        normalArray = null;
        triangleArray = null;
    }

    public void Clear()
    {
        finished = false;

        System.Array.Clear(vertexArray, 0, vertexArray.Length);
        System.Array.Clear(colorArray, 0, colorArray.Length);
        System.Array.Clear(uvArray, 0, uvArray.Length);
        System.Array.Clear(normalArray, 0, normalArray.Length);
        System.Array.Clear(triangleArray, 0, triangleArray.Length);

        vertexCount = 0;
        triangleCount = 0;
    }

    public void Refresh()
    {
        mesh.Clear();
        finished = true;

        //Debug.Log("vertexCount=" + vertexCount + ",vertexArray=" + vertexArray.Length + ",colorArray=" + colorArray.Length
        //    + ",uvArray=" + uvArray.Length + ",uv2Array=" + uv2Array.Length + ",normalArray=" + normalArray.Length
        //    + ",triangleArray=" + triangleArray.Length + ",triangleCount=" + triangleCount);

        if (triangleCount % 3 != 0)
            Debug.LogError("triangleCount is not multiple of 3, triangleCount=" + triangleCount);

        mesh.SetVertices(vertexArray, 0, vertexCount);
        mesh.SetColors(colorArray, 0, vertexCount);
        mesh.SetUVs(0, uvArray, 0, vertexCount);
        mesh.SetUVs(1, uv2Array, 0, vertexCount);
        mesh.SetNormals(normalArray, 0, vertexCount);
        mesh.SetTriangles(triangleArray, 0, triangleCount, 0, true);
    }
}
