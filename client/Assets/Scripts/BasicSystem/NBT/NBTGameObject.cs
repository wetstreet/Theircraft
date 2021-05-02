using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static MeshGenerator;

public class NBTGameObject : MonoBehaviour
{
    public NativeArray<Vertex> vertexArray;
    public NativeArray<ushort> triangleArray;

    public ushort vertexCount = 0;
    public ushort triangleCount = 0;

    public Mesh mesh;

    bool isCollidable;

    static VertexAttributeDescriptor[] vertexAttributes;

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
        vertexCount = 0;
        triangleCount = 0;
        //vertexList.Clear();
        //triangles.Clear();
    }

    private void Awake()
    {
        vertexArray = new NativeArray<Vertex>(65536, Allocator.Persistent);
        triangleArray = new NativeArray<ushort>(65536, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        vertexArray.Dispose();
        triangleArray.Dispose();
    }

    public void Refresh()
    {
        mesh.Clear();

        if (vertexAttributes == null)
        {
            vertexAttributes = new[] {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            };
        }

        mesh.SetVertexBufferParams(vertexCount, vertexAttributes);
        mesh.SetVertexBufferData(vertexArray, 0, 0, vertexCount);

        mesh.SetIndexBufferParams(triangleCount, IndexFormat.UInt16);
        print("triangleCount=" + triangleCount);
        mesh.SetIndexBufferData(triangleArray, 0, 0, triangleCount);

        mesh.subMeshCount = 1;
        SubMeshDescriptor desc = new SubMeshDescriptor();
        desc.indexStart = 0;
        desc.indexCount = triangleCount;
        desc.baseVertex = 0;
        desc.topology = MeshTopology.Triangles;
        mesh.SetSubMesh(0, desc);

        Vector3[] positions = new Vector3[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            Vertex vert = vertexArray[i];
            positions[i] = vert.pos;
        }
        Bounds bounds = GeometryUtility.CalculateBounds(positions, Matrix4x4.identity);
        mesh.bounds = bounds;

        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Array", TextureArrayManager.GetArray());
        GetComponent<MeshFilter>().sharedMesh = mesh;
        if (isCollidable)
        {
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

}
