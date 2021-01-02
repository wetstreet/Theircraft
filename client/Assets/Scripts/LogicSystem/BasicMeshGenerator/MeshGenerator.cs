using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshGenerator : MonoBehaviour
{
    protected static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    protected static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    protected static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    protected static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    protected static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    protected static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    protected static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    protected static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    static int type;
    static Vector3Int pos;
    static List<Vertex> vertices;
    static List<int> triangles;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Array/New Mesh")]
#endif
    public static void Test()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Custom/TextureArrayShader"));
        mat.SetTexture("_Array", TextureArrayManager.GetArray());
        mr.material = mat;
        MeshFilter mf = go.GetComponent<MeshFilter>();
        mf.mesh = GetMesh();
    }
    
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct Vertex
    {
        public Vector4 pos;
        public Vector2 texcoord;
    }

    public static Mesh GetMesh()
    {
        vertices = new List<Vertex>();
        triangles = new List<int>();

        Mesh mesh = new Mesh();

        AddCube(Vector3Int.zero, 0);
        AddCube(Vector3Int.right, 5);

        var vertexCount = vertices.Count;

        mesh.SetVertexBufferParams(vertexCount, new[] {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
        });
        
        var verts = new NativeArray<Vertex>(vertexCount, Allocator.Temp);

        verts.CopyFrom(vertices.ToArray());

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);
        mesh.SetTriangles(triangles.ToArray(), 0);

        return mesh;
    }

    static void AddCube(Vector3Int pos, int type)
    {
        MeshGenerator.pos = pos;
        MeshGenerator.type = type;

        AddFrontFace();
        AddRightFace();
        AddLeftFace();
        AddBackFace();
        AddTopFace();
        AddBottomFace();
    }

    static Vector4 toVector4(Vector3 v3, float w)
    {
        return new Vector4(v3.x, v3.y, v3.z, w);
    }

    static void AddFace(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
    {
        vertices.Add(new Vertex { pos = toVector4(pos1 + pos, type), texcoord = Vector2.zero });
        vertices.Add(new Vertex { pos = toVector4(pos2 + pos, type), texcoord = Vector2.up });
        vertices.Add(new Vertex { pos = toVector4(pos3 + pos, type), texcoord = Vector2.one });
        vertices.Add(new Vertex { pos = toVector4(pos4 + pos, type), texcoord = Vector2.right });

        int startIndex = vertices.Count - 4;
        triangles.AddRange(new int[] {
            startIndex, startIndex + 1, startIndex + 2,
            startIndex, startIndex + 2, startIndex + 3
        });
    }

    static void AddFrontFace()
    {
        AddFace(nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight);
    }

    static void AddBackFace()
    {
        AddFace(farBottomRight, farTopRight, farTopLeft, farBottomLeft);
    }

    static void AddTopFace()
    {
        AddFace(farTopRight, nearTopRight, nearTopLeft, farTopLeft);
    }

    static void AddBottomFace()
    {
        AddFace(nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft);
    }

    static void AddLeftFace()
    {
        AddFace(farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft);
    }

    static void AddRightFace()
    {
        AddFace(nearBottomRight, nearTopRight, farTopRight, farBottomRight);
    }
}
