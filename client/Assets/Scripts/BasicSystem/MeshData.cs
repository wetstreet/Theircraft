using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public Vector2[] uv;
    public Vector3[] normals;
    public int[] triangles;
    public Mesh mesh;

    public MeshData(Mesh mesh)
    {
        vertices = mesh.vertices;
        uv = mesh.uv;
        normals = mesh.normals;
        triangles = mesh.triangles;
        this.mesh = mesh;
    }
}
