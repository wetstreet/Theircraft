using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static MeshGenerator;

public class NBTGameObject : MonoBehaviour
{
    static int capacity = 8192;
    public List<Vector3> vertices = new List<Vector3>(capacity);
    public List<Color> colors = new List<Color>(capacity);
    public List<Vector2> uv1 = new List<Vector2>(capacity);
    public List<Vector2> uv2 = new List<Vector2>(capacity);
    public List<List<int>> trianglesList = new List<List<int>>();
    public List<Material> materialList = new List<Material>();

    public List<Vertex> vertexList = new List<Vertex>();
    public List<int> triangles = new List<int>();

    public Mesh mesh;

    bool isCollidable;

    public static NBTGameObject Create(string name, Transform parent, int layer, bool isCollidable = true)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        go.AddComponent<MeshFilter>();
        go.layer = layer;
        if (isCollidable)
        {
            go.AddComponent<MeshCollider>();
        }

        string matPath = "Materials/block";
        if (layer == 12)
        {
            matPath = "Materials/plant";
        }
        go.AddComponent<MeshRenderer>().material = Resources.Load<Material>(matPath);
        go.AddComponent<NavMeshSourceTag>();
        NBTGameObject nbtGO = go.AddComponent<NBTGameObject>();
        nbtGO.mesh = new Mesh();
        nbtGO.mesh.name = name;
        nbtGO.isCollidable = isCollidable;
        return nbtGO;
    }

    public void Clear()
    {
        vertices.Clear();
        //colors.Clear();
        //uv1.Clear();
        //uv2.Clear();
        triangles.Clear();
        //materialList.Clear();
    }

    public void Refresh()
    {
        mesh.Clear();
        //mesh.SetVertices(vertices);
        //mesh.SetUVs(0, uv1);
        //mesh.subMeshCount = trianglesList.Count;
        //for (int i = 0; i < trianglesList.Count; i++)
        //{
        //    mesh.SetTriangles(trianglesList[i], i);
        //}
        //mesh.RecalculateNormals();

        var vertexCount = vertexList.Count;

        mesh.SetVertexBufferParams(vertexCount, new[] {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
        });

        var verts = new NativeArray<Vertex>(vertexCount, Allocator.Temp);

        verts.CopyFrom(vertexList.ToArray());

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.RecalculateBounds();

        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Array", TextureArrayManager.GetArray());
        //GetComponent<MeshRenderer>().sharedMaterials = materialList.ToArray();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        if (isCollidable)
        {
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

}
