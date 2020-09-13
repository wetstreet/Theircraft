using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGameObject : MonoBehaviour
{
    static int capacity = 8192;
    public List<Vector3> vertices = new List<Vector3>(capacity);
    public List<Color> colors = new List<Color>(capacity);
    public List<Vector2> uv1 = new List<Vector2>(capacity);
    public List<Vector2> uv2 = new List<Vector2>(capacity);
    public List<List<int>> trianglesList = new List<List<int>>();
    public List<Material> materialList = new List<Material>();

    public Mesh mesh;

    public static NBTGameObject Create(string name, Transform parent)
    {

        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshCollider>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<NavMeshSourceTag>();
        go.layer = LayerMask.NameToLayer("Chunk");
        NBTGameObject nbtGO = go.AddComponent<NBTGameObject>();
        nbtGO.mesh = new Mesh();
        nbtGO.mesh.name = name;
        return nbtGO;
    }

    public void Clear()
    {
        vertices.Clear();
        colors.Clear();
        uv1.Clear();
        uv2.Clear();
        trianglesList.Clear();
    }

    public void Refresh()
    {
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv1);
        mesh.subMeshCount = trianglesList.Count;
        for (int i = 0; i < trianglesList.Count; i++)
        {
            mesh.SetTriangles(trianglesList[i], i);
        }
        mesh.RecalculateNormals();
        GetComponent<MeshRenderer>().sharedMaterials = materialList.ToArray();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

}
