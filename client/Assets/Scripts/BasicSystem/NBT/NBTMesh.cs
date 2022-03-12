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
    public int vertexCount;
    public int triangleCount;

    public int[] triangleIndexes = new int[16];
    public int subMeshCount;

    public Mesh mesh;

    public NBTMesh(int size)
    {
        mesh = new Mesh();

        vertexArray = new Vector3[size];
        colorArray = new Color32[size];
        uvArray = new Vector2[size];
        uv2Array = new Vector2[size];
        normalArray = new Vector3[size];
        int triangleSize = Mathf.FloorToInt(size * 1.5f);
        triangleArray = new int[triangleSize];
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

        mesh.SetVertices(vertexArray, 0, vertexCount);
        mesh.SetColors(colorArray, 0, vertexCount);
        mesh.SetUVs(0, uvArray, 0, vertexCount);
        mesh.SetUVs(1, uv2Array, 0, vertexCount);
        mesh.SetNormals(normalArray, 0, vertexCount);

        if (subMeshCount == 0)
        {
            // for items
            mesh.SetTriangles(triangleArray, 0, triangleCount, 0, true);
        }
        else
        {
            // for chunks
            mesh.subMeshCount = subMeshCount;

            for (int i = 0; i < subMeshCount; i++)
            {
                int start = i == 0 ? 0 : triangleIndexes[i - 1];
                mesh.SetTriangles(triangleArray, start, triangleIndexes[i] - start, i);
            }
        }
    }
}
