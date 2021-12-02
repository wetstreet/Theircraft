using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTMesh
{
    public NativeArray<Vertex> vertexArray;
    public NativeArray<ushort> triangleArray;
    Vector3[] positions;
    public ushort vertexCount = 0;
    public ushort triangleCount = 0;

    public Mesh mesh;

    public NBTMesh(int size)
    {
        vertexArray = new NativeArray<Vertex>(size, Allocator.Persistent);
        triangleArray = new NativeArray<ushort>(size, Allocator.Persistent);

        mesh = new Mesh();
        positions = new Vector3[size];
    }

    public void Dispose()
    {
        vertexArray.Dispose();
        triangleArray.Dispose();
    }

    public void Clear()
    {
        vertexCount = 0;
        triangleCount = 0;
    }

    static VertexAttributeDescriptor[] vertexAttributes;
    SubMeshDescriptor desc = new SubMeshDescriptor
    {
        indexStart = 0,
        baseVertex = 0,
        topology = MeshTopology.Triangles,
    };

    public void Refresh()
    {
        mesh.Clear();

        if (vertexCount > 0)
        {
            if (vertexAttributes == null)
            {
                vertexAttributes = new[] {
                    new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4, 0),
                    new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
                    new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4, 0),
                    new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 4, 0),
                };
            }

            mesh.SetVertexBufferParams(vertexCount, vertexAttributes);
            try
            {
                mesh.SetVertexBufferData(vertexArray, 0, 0, vertexCount, 0);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message + "\nlength=" + vertexArray.Length + ",vertexcount=" + vertexCount + ",bytes=" + (14 * 4 * vertexCount));
            }

            mesh.SetIndexBufferParams(triangleCount, IndexFormat.UInt16);
            try
            {
                mesh.SetIndexBufferData(triangleArray, 0, 0, triangleCount);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message + "\nlength=" + triangleArray.Length + ",triangleCount=" + triangleCount + ",vertexcount=" + vertexCount);
                throw e;
            }

            mesh.subMeshCount = 1;
            desc.indexCount = triangleCount;
            mesh.SetSubMesh(0, desc);

            for (int i = 0; i < vertexCount; i++)
            {
                Vertex vert = vertexArray[i];
                positions[i] = vert.pos;
            }
            for (int i = vertexCount; i < positions.Length; i++)
            {
                positions[i] = Vector3.zero;
            }
            Bounds bounds = GeometryUtility.CalculateBounds(positions, Matrix4x4.identity);
            mesh.bounds = bounds;
        }
    }
}
